using System;
using System.Collections.Generic;
using System.Linq;
using iMenyn.Data.Models;

namespace iMenyn.Data.Helpers
{
    public static class MenuHelper
    {
        public static string GetModifiedMenuId(string enterpriseId)
        {
            return string.Format("modified-menu-{0}", enterpriseId);
        }

        public static Menu AddProductToMenu(Menu menu, Product product, string categoryId)
        {
            if (menu.Categories == null)
            {
                menu.Categories = new List<Category> { new Category { Id = categoryId, Products = new List<string>() } };
            }
            var category = menu.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category == null)
            {
                category = new Category { Id = categoryId, Products = new List<string>() };
                menu.Categories.Add(category);
            }

            category.Products.Add(product.Id);

            return menu;
        }

        public static void ValidateMenu(Menu menu, string enterpriseId, Raven.Client.IDocumentSession session, Abstract.ILogger _logger)
        {
            var allProductIds = menu.Categories.SelectMany(c => c.Products);
            var productIds = allProductIds as string[] ?? allProductIds.ToArray();

            var allProducts = session.Load<Product>(productIds);

            //Check if all products belongs to this enterprise
            foreach (var product in allProducts.Where(product => product.Enterprise != enterpriseId).ToList())
            {
                foreach (var category in from category in menu.Categories from c in category.Products.Where(c => c == product.Id).ToList() select category)
                {
                    category.Products.Remove(product.Id);
                }
                _logger.Warn("Product '{0}' belongs to enterprise: '{1}' was about to be added to '{2}' Code:[hTrsvv563]", product.Id, product.Enterprise, enterpriseId);
            }

            //Remove category if it does not have any products
            foreach (var category in menu.Categories.Where(category => category.Products.Count == 0).ToList())
            {
                menu.Categories.Remove(category);
            }

            try
            {
                var productDuplicates = productIds.GroupBy(p => p.ToUpper()).SelectMany(grp => grp.Skip(1));
                foreach (var productDuplicate in productDuplicates)
                {
                    _logger.Warn("Duplicate in products found: {0}, Enterprise: {1}", productDuplicate, enterpriseId);
                }

                var categoryDuplicates = menu.Categories.GroupBy(c => c.Id.ToUpper()).SelectMany(grp => grp.Skip(1));
                foreach (var categoryDuplicate in categoryDuplicates)
                {
                    _logger.Info("Duplicate in categories found: Name: {0}, Id: {1}, Enterprise: {2}", categoryDuplicate.Name, categoryDuplicate.Id, enterpriseId);
                    categoryDuplicate.Id = GeneralHelper.GetGuid();
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal("ValidateMenu, duplicate check!", ex);
            }
        }
    }
}