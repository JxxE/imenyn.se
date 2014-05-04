using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Raven.Abstractions.Util;
using Raven.Client;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Helpers;
using iMenyn.Data.Infrastructure.Index;
using iMenyn.Data.Models;
using AutoMapper;

namespace iMenyn.Data.Concrete.Db
{
    public class RavenDbProducts : IDbProducts
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger _logger;

        public RavenDbProducts(IDocumentStore documentStore, ILogger logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }


        public void AddProduct(Product product, string categoryId, string enterpriseId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var enterprise = session.Load<Enterprise>(enterpriseId);

                if (EnterpriseHelper.ValidEditableEnterprise(enterprise, session))
                {
                    product.Enterprise = enterpriseId;

                    //Om det är en ny enterprise eller en som är ägd, spara produkten direkt
                    if (enterprise.IsNew || enterprise.OwnedByAccount)
                    {
                        enterprise.Menu = MenuHelper.AddProductToMenu(enterprise.Menu, product, categoryId);
                        _logger.Info(string.Format("New product ({0}) added to new enterprise: {1}, Code:[gPrsdfeas3]", product.Id, enterpriseId));
                    }
                    else
                    {
                        //If enterprise is existing, save/create a TEMP-menu for approvement
                        var modifiedMenuId = MenuHelper.GetModifiedMenuId(enterpriseId);
                        var menuInDb = session.Load<ModifiedMenu>(modifiedMenuId);

                        if (menuInDb == null)
                        {
                            //Copy the menu from the Enpterprise
                            var menuCopy = new Menu
                                        {
                                            Categories = new List<Category>()
                                        };
                            foreach (var c in enterprise.Menu.Categories)
                            {
                                var category = new Category { Id = c.Id, Name = c.Name, Products = new List<string>() };
                                foreach (var p in c.Products)
                                {
                                    category.Products.Add(p);
                                }
                                menuCopy.Categories.Add(category);
                            }
                            menuInDb = new ModifiedMenu { Id = modifiedMenuId, Menu = menuCopy };

                            enterprise.ModifiedMenu = modifiedMenuId;
                        }

                        MenuHelper.AddProductToMenu(menuInDb.Menu, product, categoryId);
                        session.Store(menuInDb);
                        _logger.Info(string.Format("New product {0} added to modified menu:{1} Code:[g8iopgdfe]", product.Id, modifiedMenuId));

                    }
                    session.Store(enterprise);
                    session.Store(product);
                    session.SaveChanges();

                }
                else
                {
                    var loggedInUser = !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name) ? string.Format(", logged in user: {0}", HttpContext.Current.User.Identity.Name) : string.Empty;
                    _logger.Warn(string.Format("A product({0}) was about to be added to a non-valid enterprise with id: {1}{2}, Code:[yTerdfds56]", product.Name, enterpriseId, loggedInUser));
                }

            }
        }

        public void UpdateProduct(Product product, string enterpriseId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var enterprise = session.Load<Enterprise>(enterpriseId);

                if (EnterpriseHelper.ValidEditableEnterprise(enterprise, session))
                {
                    //Om det är en ny enterprise eller en som är ägd, spara produkten direkt
                    if (enterprise.IsNew || enterprise.OwnedByAccount)
                    {
                        session.Store(product);
                    }
                    else
                    {
                        if (product.Enterprise == enterpriseId)
                        {
                            //If enterprise is existing, save an updated version of the product for approvement
                            var productInDb = session.Load<Product>(product.Id);
                            Mapper.CreateMap<Product, ProductUpdatedVersion>();
                            var updatedProduct = new ProductUpdatedVersion();
                            Mapper.Map(product, updatedProduct);
                            productInDb.UpdatedVersion = updatedProduct;
                        }
                        else
                        {
                            _logger.Warn(string.Format("Product: {0} belongs to this enterprise: {1} and was about to be updated to {2} Code:[hT882v563]", product.Id, product.Enterprise, enterpriseId));
                        }
                    }
                    _logger.Info(string.Format("Product:{0} was updated. Enterprise: {1}. Code:[poO0789b]",product.Id,enterpriseId));
                    session.SaveChanges();
                }
            }
        }

        public IEnumerable<Product> GetProducts(List<string> productIds)
        {
            using (var session = _documentStore.OpenSession())
            {
                var products = session.Load<Product>(productIds);
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

        public IEnumerable<Product> GetAllProductsInDb()
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<Product>().ToArray();
            }
        }
    }
}