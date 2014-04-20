using System;
using System.Collections.Generic;
using System.Linq;
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


        public void AddProduct(Product product)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(product);
                session.SaveChanges();
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