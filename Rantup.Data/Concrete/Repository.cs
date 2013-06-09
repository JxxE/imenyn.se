using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rantup.Data.Abstract;
using Rantup.Data.Helpers;
using Rantup.Data.Infrastructure.Index;
using Rantup.Data.Models;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Util;
using Raven.Client;
using Raven.Json.Linq;

namespace Rantup.Data.Concrete
{
    public class Repository : IRepository
    {
        private readonly IDocumentStore _documentStore;
        public Repository(IRavenDbContext ravenDbContext)
        {
            _documentStore = ravenDbContext.DocumentStore;
        }

        public void AddAccount(Account account)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(account);
                session.SaveChanges();
            }
        }

        public IEnumerable<Account> GetAccounts()
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<Account>();
            }
        }

        public Account GetUserByEmail(string email)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<Account>().FirstOrDefault(u => u.Email == email);
            }
        }

        public Account GetAccount(string accountId)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<Account>(accountId);
            }
        }

        public void UpdateAccount(Account account)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(account);
                session.SaveChanges();
            }
        }

        public void AddProduct(Product product)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(product);
                session.SaveChanges();
            }
        }

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

        public void UpdateMenu(Menu menu)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(menu);
                session.SaveChanges();
            }
        }

        public void UpdateProducts(IEnumerable<Product> products)
        {
            using (var session = _documentStore.OpenSession())
            {
                foreach (var product in products)
                {
                    session.Store(product);
                }
                session.SaveChanges();
            }
        }

        public void DeleteEnterpriseById(string enterpriseId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var enterprise = session.Load<Enterprise>(enterpriseId);
                session.Delete(enterprise);
                session.SaveChanges();
            }
        }

        public void DeleteMenuById(string menuId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var menu = session.Load<Menu>(menuId);
                session.Delete(menu);
                session.SaveChanges();
            }
        }

        public void DeleteProductsByIds(List<string> productIds)
        {
            using (var session = _documentStore.OpenSession())
            {
                var products = session.Load<Product>(productIds);
                foreach (var product in products.Where(p => p != null))
                {
                    session.Delete(product);
                }
                session.SaveChanges();
            }
        }

        public void DeleteModifiedMenuById(string modifiedMenuId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var modifiedMenu = session.Load<ModifiedMenu>(modifiedMenuId);
                session.Delete(modifiedMenu);
                session.SaveChanges();
            }
        }

        public void CreateEnterprise(Enterprise enterprise)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(enterprise);
                session.SaveChanges();
            }
        }

        public void CreateMenu(Menu menu)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(menu);
                session.SaveChanges();
            }
        }

        public void CreateProducts(IEnumerable<Product> products)
        {
            using (var session = _documentStore.OpenSession())
            {
                foreach (var product in products)
                {
                    session.Store(product);
                }
                session.SaveChanges();
            }
        }

        public void CreateModifiedMenu(ModifiedMenu modifiedMenu)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(modifiedMenu);
                session.SaveChanges();
            }
        }

        public IEnumerable<Product> GetProducts(List<string> productIds)
        {
            using (var session = _documentStore.OpenSession())
            {
                var products = session.Load<Product>(productIds);
                //Logga för alla produkter som är null. De står med i menyn men är borttagna objekt
                return products.Where(p => p != null);
            }
        }

        public Product GetProductById(string productId)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<Product>(productId);
            }
        }

        public object GetPage(string id)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<object>(id);
            }
        }

        public Enterprise GetEnterpriseById(string enterpriseId)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<Enterprise>(enterpriseId);
            }
        }

        public IEnumerable<Enterprise> GetAllEnterprises()
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<Enterprise>();
            }
        }

        public IEnumerable<ModifiedMenu> GetAllModifiedMenus()
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<ModifiedMenu>();
            }
        }

        public ModifiedMenu GetModifiedMenuByEnterpriseId(string enterpriseId)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<ModifiedMenu>().FirstOrDefault(m => m.EnterpriseId == enterpriseId);
            }
        }

        public ModifiedMenu GetModifiedMenuById(string modifiedMenuId)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<ModifiedMenu>(modifiedMenuId);
            }
        }

        public Enterprise GetEnterpriseByUrlKey(string urlKey)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<Enterprise>("Enterprises-" + urlKey);
            }
        }

        public void UpdateEnterprise(Enterprise enterprise)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(enterprise);
                session.SaveChanges();
            }
        }

        public Menu GetMenuById(string menuId)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<Menu>(menuId);
            }
        }

        public void UpdateProduct(Product product)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(product);
                session.SaveChanges();
            }
        }

        public IEnumerable<Enterprise> SearchEnterprises(string tbSearch, string location, string categorySearch)
        {
            using (var session = _documentStore.OpenSession())
            {
                var isTbSearch = !String.IsNullOrEmpty(tbSearch) && String.IsNullOrEmpty(categorySearch);
                var isCategorySearch = String.IsNullOrEmpty(tbSearch) && !String.IsNullOrEmpty(categorySearch);

                var query = session.Advanced.LuceneQuery<Enterprise, Enterprises>();


                if (isTbSearch || isCategorySearch)
                {
                    var searchTerms = new string[] { };

                    //Search made from textbox
                    if (isTbSearch)
                        searchTerms = tbSearch.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

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

                            query = query.WhereEquals(x => x.IsTemp, false).AndAlso();

                            if (isTbSearch)
                            {
                                query = query.OpenSubclause();
                                query = query.WhereStartsWith(x => x.Name, searchQuery).OrElse();
                                query = query.Search(x => x.Name, searchQuery).OrElse();

                                var stateCode = GeneralHelper.GetCountyNameAndCodes().FirstOrDefault(c => c.Text.ToLower().Contains(searchQuery));
                                if (stateCode != null)
                                {
                                    query = query.WhereEquals(x => x.StateCode, stateCode.Value).OrElse();
                                }

                                #region PostalCode search
                                int postalCode;
                                int.TryParse(tbSearch, out postalCode);
                                if (postalCode > 0)
                                {
                                    query = query.WhereBetweenOrEqual(x => x.PostalCode, postalCode - 500, postalCode + 500).OrElse();
                                }
                                #endregion

                                query = query.WhereStartsWith(x => x.City, searchQuery);
                                query = query.CloseSubclause();

                            }
                            if (isCategorySearch)
                            {
                                if (categorySearch == "All")
                                    query = query.WhereEquals(x => x.StateCode, location);
                                else
                                {
                                    query = query.WhereEquals(x => x.StateCode, location).AndAlso();
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
    }
}