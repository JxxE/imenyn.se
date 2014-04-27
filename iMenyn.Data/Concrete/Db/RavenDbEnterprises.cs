using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
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
                if (enterprise != null)
                {
                    // TODO if is valid editable
                    enterprise.Menu = menu;
                    //Loopa igenom nuvarande meny o jämför om en produkt har tagits bort. Ta då bort den från databasen.
                    session.Store(enterprise);

                    session.SaveChanges();
                    _logger.Info("Updated enterprise menu settings: " + enterprise.Name); 
                }
                
            }
        }

        //TODO
        public string CreateEnterprise(Enterprise enterprise)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(enterprise);
                session.SaveChanges();
                _logger.Info("New enterprise created: " + enterprise.Name);
                EmailHelper.NewEnterpriseNotification();

                return enterprise.Id;
            }
        }

        //TODO
        public void DeleteEnterpriseById(string enterpriseId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var enterprise = session.Load<Enterprise>(enterpriseId);
                session.Delete(enterprise);
                session.SaveChanges();
                _logger.Info("Enterprise deleted: " + enterprise.Name);
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
                query = query.WhereStartsWith(x => x.Key, searchQuery);

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

        //TODO
        public IEnumerable<Enterprise> GetNewEnterprises()
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Advanced.LuceneQuery<Enterprise, Enterprises>().WhereEquals(e => e.IsNew, true);
            }
        }


        public Enterprise GetEnterpriseById(string enterpriseId)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<Enterprise>(enterpriseId);
            }
        }

        public CompleteEnterpriseViewModel GetCompleteEnterprise(string enterpriseId)
        {
            using (var session = _documentStore.OpenSession())
            {
                
                var viewModel = new CompleteEnterpriseViewModel();

                // Load enterprise, include products
                var enterprise = session.Include<Enterprise>(e => e.Menu.Categories.Select(c => c.Products)).Load(enterpriseId);
                
                var products = new List<Product>();
                foreach (var p in enterprise.Menu.Categories.Select(category => session.Load<Product>(category.Products)))
                {
                    products.AddRange(p);
                }

                // Create to viewmodel
                var categoriesViewModel = new List<ViewModelCategory>();
                Mapper.CreateMap<Product, ProductViewModel>();
                foreach (var category in enterprise.Menu.Categories)
                {
                    var categoryViewModel = new ViewModelCategory
                                {
                                    Name = category.Name,
                                    Id= category.Id,
                                    Products = new List<ProductViewModel>()
                                };

                    foreach (var product in category.Products.Select(productForCategory => products.FirstOrDefault(p => p.Id == productForCategory)))
                    {
                        var p = Mapper.Map<Product, ProductViewModel>(product);
                        categoryViewModel.Products.Add(p);
                    }

                    categoriesViewModel.Add(categoryViewModel);
                }

                // Add to viewmodel
                viewModel.Name = enterprise.Name;
                viewModel.Id = enterprise.Id;
                viewModel.ViewModelCategories = categoriesViewModel;

                return viewModel;
            }
        }

        public IEnumerable<string> GetModifiedMenus()
        {
            using (var session = _documentStore.OpenSession())
            {
                var menus = session.Query<Menu>();
                var enterpriseIds = menus.Where(m => !string.IsNullOrEmpty(m.TempMenuId));
                return null;
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
    }
}