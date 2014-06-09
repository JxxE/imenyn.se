using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Microsoft.Security.Application;
using Raven.Abstractions.Indexing;
using Raven.Abstractions.Util;
using Raven.Client;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Attributes;
using iMenyn.Data.Helpers;
using iMenyn.Data.Infrastructure.Index;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;

namespace iMenyn.Data.Concrete.Db
{
    public class RavenDbEnterprises : IDbEnterprises
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger _logger;

        public RavenDbEnterprises(IDocumentStore documentStore, ILogger logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        public void UpdateEnterprise(Enterprise enterprise)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(enterprise);
                session.SaveChanges();
                _logger.Info("Updated enterprise " + enterprise.Name, ", id: " + enterprise.Id);
            }
        }

        public bool UpdateEnterprise(string enterpriseId, Menu menu)
        {
            var updated = false;
            using (var session = _documentStore.OpenSession())
            {
                var enterprise = session.Load<Enterprise>(enterpriseId);

                try
                {
                    if (EnterpriseHelper.ValidEditableEnterprise(enterprise, session))
                    {
                        MenuHelper.ValidateMenu(menu, enterpriseId, session, _logger);
                        if (enterprise.IsNew || enterprise.OwnedByAccount)
                        {
                            var deletedProductsIds = GeneralHelper.CompareLists(enterprise.Menu.Categories.SelectMany(c => c.Products).ToList(), menu.Categories.SelectMany(c => c.Products).ToList());

                            if (deletedProductsIds.Count > 0)
                            {
                                var deletedProducts = session.Load<Product>(deletedProductsIds);
                                //Delete products that belongs to this enterprise
                                foreach (var deletedProduct in deletedProducts.Where(deletedProduct => deletedProduct.Enterprise == enterpriseId))
                                {
                                    session.Delete(deletedProduct);
                                }
                            }

                            enterprise.Menu = menu;
                            enterprise.LastUpdated = DateTime.Now;
                        }
                        else
                        {
                            var modifiedMenuInDb = session.Load<ModifiedMenu>(MenuHelper.GetModifiedMenuId(enterpriseId));

                            if (modifiedMenuInDb == null)
                            {
                                var modifiedMenu = new ModifiedMenu
                                {
                                    Id = MenuHelper.GetModifiedMenuId(enterpriseId),
                                    Menu = menu
                                };
                                session.Store(modifiedMenu);

                                enterprise.LockedFromEdit = true;
                                enterprise.ModifiedMenu = modifiedMenu.Id;
                            }

                        }

                        session.Store(enterprise);
                        session.SaveChanges();

                        updated = true;

                        _logger.Info("Updated menu settings for enterprise: " + enterprise.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex.Message, ex);
                    var failedEnterprise = new FailedEnterprise
                                               {
                                                   Enterprise = enterprise,
                                                   Menu = menu,
                                                   ErrorMessage = ex.Message
                                               };
                    session.Store(failedEnterprise);
                    session.SaveChanges();
                }
            }
            return updated;
        }

        public string CreateEnterprise(Enterprise enterprise)
        {
            using (var session = _documentStore.OpenSession())
            {
                if (enterprise == null)
                    return string.Empty;
                if (!string.IsNullOrEmpty(enterprise.Id))
                {
                    if (session.Load<Enterprise>(enterprise.Id) != null)
                        return string.Empty;
                }

                session.Store(enterprise);
                session.SaveChanges();
                _logger.Info(string.Format("New enterprise created: {0} ({1}), Code:[nBNbn5fgaq]", enterprise.Name, enterprise.Id));

                return enterprise.Id;
            }
        }

        public void DeleteEnterprise(string enterpriseId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var enterprise = session.Include<Enterprise>(e => e.Menu.Categories.Select(c => c.Products)).Load(enterpriseId);
                var enterpriseProductIds = new List<string>();
                if (enterprise.Menu != null && enterprise.Menu.Categories != null)
                    enterpriseProductIds = enterprise.Menu.Categories.SelectMany(c => c.Products).ToList();
                //Get products that belongs to this enterprise
                var productsInDb = session.Load<Product>(enterpriseProductIds).Where(p => p.Enterprise == enterpriseId);

                //Delete products
                foreach (var product in productsInDb)
                {
                    session.Delete(product);
                }

                //Delete modified menu
                if (enterprise.ModifiedMenu != null)
                {
                    var modifiedMenu = session.Load<ModifiedMenu>(enterprise.ModifiedMenu);
                    session.Delete(modifiedMenu);
                }

                session.Delete(enterprise);
                session.SaveChanges();
                _logger.Info(string.Format("Enterprise ({0}, {1}) was deleted with {2} products, Code:[45ouupl]", enterprise.Id, enterprise.Name, productsInDb.ToList().Count()));
            }
        }

        public IEnumerable<Enterprise> SearchEnterprises(string searchTerm, string location, string categorySearch)
        {
            using (var session = _documentStore.OpenSession())
            {
                var isTbSearch = !String.IsNullOrEmpty(searchTerm) && String.IsNullOrEmpty(categorySearch);
                var isCategorySearch = String.IsNullOrEmpty(searchTerm) && !String.IsNullOrEmpty(categorySearch);

                var query = session.Advanced.LuceneQuery<Enterprise, Enterprises>();


                if (isTbSearch || isCategorySearch)
                {
                    var searchTerms = new string[] { };

                    //Search made from textbox
                    if (isTbSearch)
                        searchTerms = searchTerm.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    //Category search from button-group
                    if (isCategorySearch)
                        searchTerms = categorySearch.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    searchTerms = searchTerms.Where(s => !string.IsNullOrEmpty(s.Trim())).ToArray();

                    if (searchTerms.Length > 0)
                    {
                        query = query.OpenSubclause();

                        for (var i = 0; i < searchTerms.Length; i++)
                        {
                            var searchQuery = RavenQuery.Escape(searchTerms[i], true, false);

                            if (i > 0)
                                query = query.OrElse();

                            query = query.WhereEquals(x => x.IsNew, false).AndAlso();

                            if (isTbSearch)
                            {
                                query = query.OpenSubclause();
                                query = query.WhereStartsWith(x => x.Name, searchQuery).OrElse();
                                query = query.Search(x => x.Name, searchQuery);

                                //var stateCode = GeneralHelper.GetCountyNameAndCodes().FirstOrDefault(c => c.Text.ToLower().Contains(searchQuery));
                                //if (stateCode != null)
                                //{
                                //    //County search
                                //    //query = query.WhereEquals(x => x.StateCode, stateCode.Value).OrElse();
                                //}

                                #region PostalCode search
                                int postalCode;
                                int.TryParse(searchTerm, out postalCode);
                                if (postalCode > 0)
                                {
                                    query = query.WhereBetweenOrEqual(x => x.PostalCode, postalCode - 500, postalCode + 500).OrElse();
                                }
                                #endregion

                                //Commune, sublocality, county m.m.
                                //query = query.WhereStartsWith(x => x.City, searchQuery);
                                query = query.CloseSubclause();

                            }
                            if (isCategorySearch)
                            {
                                if (categorySearch == "All")
                                {
                                    //query = query.WhereEquals(x => x.StateCode, location);
                                }
                                else
                                {
                                    //query = query.WhereEquals(x => x.StateCode, location).AndAlso();
                                    query = query.WhereIn("Categories", new[] { searchQuery });
                                }

                            }
                        }
                        query = query.CloseSubclause();

                        query = query.OrderBy(x => x.Name);

                        return query.Take(30).ToList();
                    }
                }

                return null;
            }
        }

        public IEnumerable<Enterprise> GetNearbyEnterprises(string lat,string lng)
        {
            using(var session = _documentStore.OpenSession())
            {
                _logger.Info("Searched nearby enterprises for lat: {0} and lng: {1}", lat, lng);

                return session.Advanced.LuceneQuery<Enterprise, Enterprises>()
                    .WhereEquals(x => x.IsNew, false)
                    .WithinRadiusOf("Coordinates", 30, double.Parse(lat, CultureInfo.InvariantCulture), double.Parse(lng, CultureInfo.InvariantCulture))
                    .SortByDistance();
            }
        } 

        //TODO
        public IEnumerable<Enterprise> CheckIfEnterpriseExists(string key, int postalCode)
        {
            using (var session = _documentStore.OpenSession())
            {
                var query = session.Advanced.LuceneQuery<Enterprise, Enterprises>();

                query = query.OpenSubclause();

                var searchQuery = RavenQuery.Escape(key, true, false);

                query = query.WhereEquals(x => x.PostalCode, postalCode).AndAlso();

                query = query.CloseSubclause();
                return query;
            }
        }

        //TODO
        public IEnumerable<Enterprise> GetAllEnterprises()
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<Enterprise>();
            }
        }

        //DONE 
        public IEnumerable<Enterprise> GetModifiedAndNewEnterprises()
        {
            using (var session = _documentStore.OpenSession())
            {
                var query = session.Advanced.LuceneQuery<Enterprise, Enterprises>();

                query.WhereEquals(e => e.IsNew, true);
                query.OrElse();
                query.WhereEquals(e => e.LockedFromEdit, true);

                return query.OrderByDescending(e => e.LastUpdated);
            }
        }


        public Enterprise GetEnterpriseById(string enterpriseId)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<Enterprise>(enterpriseId);
            }
        }

        public CompleteEnterpriseViewModel GetCompleteEnterprise(string enterpriseId, bool edit = false)
        {
            using (var session = _documentStore.OpenSession())
            {
                var viewModel = new CompleteEnterpriseViewModel();

                if (string.IsNullOrEmpty(enterpriseId))
                    return viewModel;

                // Load enterprise, include products
                var enterprise = session.Include<Enterprise>(e => e.Menu.Categories.Select(c => c.Products)).Load(enterpriseId);

                if (enterprise == null)
                    return viewModel;

                var menu = enterprise.Menu;

                var newProducts = new List<string>();
                var deletedProductIds = new List<string>();

                //Load the modified menu if: edit-mode, has a modified menu, is NOT new, is NOT owned by enterprise
                if (edit && !string.IsNullOrEmpty(enterprise.ModifiedMenu) && !enterprise.IsNew && !enterprise.OwnedByAccount)
                {
                    //Get the modified menu instead
                    var modifiedMenu = session.Include<Menu>(e => e.Categories.Select(c => c.Products)).Load<ModifiedMenu>(enterprise.ModifiedMenu);

                    //Get ids of all new products. Products that exist in the Modified-menu but not the original menu
                    newProducts = modifiedMenu.Menu.Categories.SelectMany(c => c.Products).Where(p => !enterprise.Menu.Categories.SelectMany(c => c.Products).Select(p1 => p1).Contains(p)).ToList();

                    //Get ids of all deleted products. Products that does NOT exist in the Modified-menu but not the original menu
                    deletedProductIds = enterprise.Menu.Categories.SelectMany(c => c.Products).Where(p => !modifiedMenu.Menu.Categories.SelectMany(c => c.Products).Select(p1 => p1).Contains(p)).ToList();

                    //Set modifiedMenu to menu
                    Mapper.CreateMap<ModifiedMenu, Menu>();
                    menu = Mapper.Map(modifiedMenu.Menu, menu);
                }
                var products = new List<Product>();
                // Create to viewmodel
                var categoriesViewModel = new List<ViewModelCategory>();
                if (menu != null)
                {
                    if (menu.Categories != null)
                    {
                        foreach (var p in menu.Categories.Select(category => session.Load<Product>(category.Products)))
                        {
                            products.AddRange(p);
                        }

                        Mapper.CreateMap<Product, ProductViewModel>();
                        foreach (var category in menu.Categories)
                        {
                            var categoryViewModel = new ViewModelCategory
                            {
                                Name = category.Name,
                                Id = category.Id,
                                EnterpriseId = enterpriseId,
                                Products = new List<ProductViewModel>()
                            };

                            foreach (var product in category.Products.Select(productForCategory => products.FirstOrDefault(p => p.Id == productForCategory)))
                            {

                                var p = ProductHelper.ModelToViewModel(product);
                                if (edit && p.UpdatedVersion != null)
                                {
                                    p.Updated = true;
                                    p.OriginalProduct = product;
                                    //p.UpdatedVersion = product.UpdatedVersion;
                                    Mapper.CreateMap<ProductUpdatedVersion, ProductViewModel>();
                                    Mapper.Map(product.UpdatedVersion, p);
                                }
                                if (edit)
                                {
                                    if (newProducts.Contains(p.Id))
                                    {
                                        p.New = true;
                                    }
                                }
                                p.Enterprise = enterpriseId;
                                p.CategoryId = category.Id;
                                categoryViewModel.Products.Add(p);
                            }



                            categoriesViewModel.Add(categoryViewModel);
                        }
                        if (edit)
                        {
                            //Add deleted products to viewmodel
                            var deletedProducts = session.Load<Product>(deletedProductIds);
                            var deletedProductsViewModel = ProductHelper.ModelToViewModel(deletedProducts.ToList());
                            foreach (var productViewModel in deletedProductsViewModel)
                            {
                                productViewModel.Deleted = true;
                            }
                            viewModel.DeletedProducts = deletedProductsViewModel;
                        }
                    }
                }
                // Add to viewmodel
                viewModel.ViewModelCategories = categoriesViewModel;

                viewModel.Enterprise = EnterpriseHelper.ModelToViewModel(enterprise);
                viewModel.Enterprise.DisplayCategories = EnterpriseHelper.GetDisplayCategories(enterprise.Categories);
                viewModel.Enterprise.DisplayStreet = string.Format("{0} {1}", enterprise.StreetRoute, enterprise.StreetNumber);

                if (!edit)
                {
                    viewModel.Enterprise.ModifiedMenu = null;
                }

                return viewModel;
            }
        }

        public IEnumerable<Enterprise> GetModifiedEnterprises()
        {
            using (var session = _documentStore.OpenSession())
            {
                var enterprises = session.Query<Enterprise>();
                var enterpriseIds = enterprises.Where(e => !string.IsNullOrEmpty(e.ModifiedMenu));
                return enterpriseIds;
            }
        }

        public Enterprise GetEnterpriseByUrlKey(string urlKey)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<Enterprise>("Enterprises-" + urlKey);
            }
        }

        public IEnumerable<Enterprise> GetEnterprisesByLocation(string stateCode, string city)
        {
            using (var session = _documentStore.OpenSession())
            {
                var query = session.Advanced.LuceneQuery<Enterprise, Enterprises>();

                query = query.WhereEquals(x => x.IsNew, false).AndAlso();

                if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(stateCode))
                {
                    //query = query.WhereEquals(x => x.StateCode, stateCode).AndAlso();
                    //query = query.WhereEquals(x => x.City, city);
                }
                else
                {
                    //query = query.WhereEquals(x => x.StateCode, stateCode);
                }


                return query.OrderBy(x => x.Name);
            }
        }

        public void SetModifiedMenuAsDefault(string enterpriseId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var enterprise = session.Load<Enterprise>(enterpriseId);
                var modifiedMenu = session.Include<ModifiedMenu>(m => m.Menu.Categories.SelectMany(p => p.Products)).Load(enterprise.ModifiedMenu);

                //Get deleted products. Products that exist in Original menu but NOT in the new Modified menu.
                var deletedProductsIds =
                    GeneralHelper.CompareLists(enterprise.Menu.Categories.SelectMany(c => c.Products).ToList(),
                                               modifiedMenu.Menu.Categories.SelectMany(c => c.Products).ToList());
                var productsToDelete = session.Load<Product>(deletedProductsIds);
                foreach (var productToDelete in productsToDelete)
                {
                    //Delete products
                    if (productToDelete.Id == enterpriseId)
                        session.Delete(productToDelete);
                    else
                        _logger.Warn(string.Format("Was about to delete product ({0}) that not belongs to enterprise ({1}) Code:[bvfgttls]", productToDelete.Id, enterpriseId));
                }
                if (productsToDelete.Any())
                    _logger.Info(string.Format("{0} products deleted for approved modified menu. Enterprise: {1}", productsToDelete.Count(), enterpriseId));

                //Set all updated versions to be the real versions
                var products = session.Load<Product>(modifiedMenu.Menu.Categories.SelectMany(p => p.Products));
                foreach (var p in products.Where(p => p.UpdatedVersion != null))
                {
                    var product = ProductHelper.AddUpdatedInfoToProduct(p);
                    product.UpdatedVersion = null;
                }
                if (products.Any())
                    _logger.Info(string.Format("Updated {0} products for approved modified menu. Enterprise: {1}", products.Count(), enterpriseId));

                enterprise.Menu = modifiedMenu.Menu;
                enterprise.ModifiedMenu = null;

                session.Delete(modifiedMenu);

                session.SaveChanges();
            }
        }


        public void DisapproveModifiedMenu(string enterpriseId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var enterprise = session.Load<Enterprise>(enterpriseId);

                var productsInOrginalMenu = enterprise.Menu.Categories.SelectMany(c => c.Products);

                var modifiedMenu = session.Include<ModifiedMenu>(m => m.Menu.Categories.SelectMany(p => p.Products)).Load(enterprise.ModifiedMenu);

                var productsInModifiedMenu = modifiedMenu.Menu.Categories.SelectMany(c => c.Products);

                //Get ids for new products in this modified menu. Products that exist in modified menu but NOT in original menu
                var IdsForNewProductsInModifiedMenu = GeneralHelper.CompareLists(productsInModifiedMenu.ToList(), productsInOrginalMenu.ToList());
                var newProductsToDelete = session.Load<Product>(IdsForNewProductsInModifiedMenu);
                foreach (var newProduct in newProductsToDelete)
                {
                    //Delete all new products for this modified menu
                    if (newProduct.Enterprise == enterpriseId)
                    {
                        session.Delete(newProduct);
                    }
                    else
                        _logger.Warn(string.Format("Was about to delete product ({0}) that not belongs to enterprise ({1}). Code:[iplikhff]", newProduct.Id, enterpriseId));
                }

                if (newProductsToDelete.Any())
                    _logger.Info(string.Format("{0} products deleted for disapproved modified menu. Enterprise: {1}", newProductsToDelete.Count(), enterpriseId));

                session.Delete(modifiedMenu);

                enterprise.ModifiedMenu = null;
                enterprise.LockedFromEdit = false;

                var products = session.Load<Product>(enterprise.Menu.Categories.SelectMany(p => p.Products));
                foreach (var p in products.Where(p => p.UpdatedVersion != null))
                {
                    p.UpdatedVersion = null;
                }

                session.SaveChanges();
            }
        }
    }
}