﻿using System.Collections.Generic;
using iMenyn.Data.Models;

namespace iMenyn.Data.Abstract.Db
{
    public interface IDbProducts
    {
        void AddProduct(Product product,string categoryId,string enterpriseId);

        IEnumerable<Product> GetProducts(List<string> productIds);

        Product GetProductById(string productId);

        void UpdateProduct(Product product,string enterpriseId);

        void UpdateProducts(IEnumerable<Product> products);

        void DeleteProductsByIds(List<string> productIds);

        IEnumerable<Product> GetAllProductsInDb();


        void AddProductsToDb(List<Product> products);

        int ProductTotalCount();
    }
}
