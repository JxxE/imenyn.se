using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Security.Application;
using Raven.Abstractions.Util;
using Raven.Client;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
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

        public void UpdateEnterprise(string enterpriseId, Menu menu)
        {
            using (var session = _documentStore.OpenSession())
            {
                var enterprise = session.Load<Enterprise>(enterpriseId);

                if (EnterpriseHelper.ValidEditableEnterprise(enterprise, session))
                {
                    MenuHelper.ValidateMenu(menu, enterpriseId, session, _logger);
                    if (enterprise.IsNew || enterprise.OwnedByAccount)
                    {
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
                    _logger.Info("Updated menu settings for enterprise: " + enterprise.Id);
                }

            }
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
                var productsInDb = session.Load<Product>(enterpriseProductIds);

                var productsToRemove = (from productInDb in productsInDb where productInDb.Id == enterprise.Id select productInDb.Id).ToList();

                foreach (var product in productsToRemove)
                {
                    session.Delete(product);
                }
                session.Delete(enterprise);
                session.SaveChanges();
                _logger.Info(string.Format("Enterprise ({0}, {1}) deleted with {2} products, Code:[45ouupl]", enterprise.Id, enterprise.Name, productsToRemove.Count));
            }
        }

        //TODO
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
                                query = query.Search(x => x.Name, searchQuery).OrElse();

                                var stateCode = GeneralHelper.GetCountyNameAndCodes().FirstOrDefault(c => c.Text.ToLower().Contains(searchQuery));
                                if (stateCode != null)
                                {
                                    //County search
                                    //query = query.WhereEquals(x => x.StateCode, stateCode.Value).OrElse();
                                }

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

                        return query.Take(30);
                    }
                }

                return null;
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
                var menu = enterprise.Menu;

                //Load the modified menu if: edit-mode, has a modified menu, is NOT new, is NOT owned by enterprise
                if (edit && !string.IsNullOrEmpty(enterprise.ModifiedMenu) && !enterprise.IsNew && !enterprise.OwnedByAccount)
                {
                    //Get the modified menu instead
                    var modifiedMenu = session.Include<Menu>(e => e.Categories.Select(c => c.Products)).Load<ModifiedMenu>(enterprise.ModifiedMenu);
                    Mapper.CreateMap<ModifiedMenu, Menu>();
                    menu = Mapper.Map(modifiedMenu.Menu, menu);
                }
                var products = new List<Product>();
                // Create to viewmodel
                var categoriesViewModel = new List<ViewModelCategory>();
                if(menu != null)
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
                                if (edit && product.UpdatedVersion != null)
                                {
                                    Mapper.CreateMap<ProductUpdatedVersion, Product>();
                                    Mapper.Map(product.UpdatedVersion, product);

                                }
                                var p = ProductHelper.ModelToViewModel(product);
                                p.EnterpriseId = enterpriseId;
                                p.CategoryId = category.Id;
                                categoryViewModel.Products.Add(p);
                            }

                            categoriesViewModel.Add(categoryViewModel);
                        }
                    }
                }
                // Add to viewmodel
                viewModel.ViewModelCategories = categoriesViewModel;
                
                viewModel.Enterprise = EnterpriseHelper.ModelToViewModel(enterprise);
                viewModel.Enterprise.DisplayCategories = EnterpriseHelper.GetDisplayCategories(enterprise.Categories);
                viewModel.Enterprise.Address = string.Format("{0} {1}", enterprise.StreetRoute, enterprise.StreetNumber);

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


        //TODO
        public void SetModifiedMenuAsDefault(string enterpriseId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var enterprise = session.Load<Enterprise>(enterpriseId);
                var modifiedMenu = session.Include<ModifiedMenu>(m => m.Menu.Categories.SelectMany(p => p.Products)).Load(enterprise.ModifiedMenu);

                //Should be set to null???
                enterprise.Menu = modifiedMenu.Menu;

                var products = session.Load<Product>(modifiedMenu.Menu.Categories.SelectMany(p => p.Products));
                foreach (var p in products)
                {
                    var product = ProductHelper.UpdatedVersionToModel(p.UpdatedVersion);
                    product.UpdatedVersion = null;
                }
            }
        }
    }
}