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

                if (enterprise != null)
                {
                    if (EnterpriseHelper.ValidEditableEnterprise(enterprise, session))
                    {
                        session.Store(product);

                        var category = enterprise.Menu.Categories.FirstOrDefault(c => c.Id == categoryId);
                        if (category == null)
                        {
                            category = new Category { Id = categoryId, Name = "Ny kategori", Products = new List<string>() };
                            enterprise.Menu.Categories.Add(category);
                        }

                        //Om det är en ny enterprise eller en som är ägd, spara produkten direkt
                        if (enterprise.IsNew || enterprise.OwnedByAccount)
                        {
                            category.Products.Add(product.Id);
                            enterprise.LastUpdated = DateTime.Now;
                            session.Store(enterprise);    
                        }
                        else
                        {
                            //If enterprise is existing, save/create a TEMP-menu for approvement 
                            //TODO FIXA
                            var modifiedMenu = new ModifiedMenu{Menu = enterprise.Menu};
                            if(!string.IsNullOrEmpty(enterprise.ModifiedMenu))
                            {
                                var modifiedMenuInDb = session.Load<ModifiedMenu>(enterprise.ModifiedMenu);
                                if (modifiedMenuInDb != null)
                                    modifiedMenu = modifiedMenuInDb;
                            }

                        }
                        session.SaveChanges();
                        _logger.Info(string.Format("New product created with Id: {0} for enterprise: {1}", product.Id, enterpriseId));
                    }
                    else
                    {
                        _logger.Warn(string.Format("A product({0}) was about to be added to a non-valid enterprise with id: {1}, Code:[yTerdfds56]", product.Name, enterpriseId));
                    }
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

        public void UpdateProduct(Product product)
        {
            using (var session = _documentStore.OpenSession())
            {
                //TODO, gör en koll om denna får redigeras och om den tillhör denna enterprise!
                //is valid editable
                session.Store(product);
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
    }
}