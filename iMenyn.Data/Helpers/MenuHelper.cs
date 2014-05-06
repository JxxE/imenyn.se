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
            //TODO future: product Load can fail if categories.Count is greater than 30
            foreach (var category in menu.Categories)
            {
                foreach (var product in session.Load<Product>(category.Products))
                {
                    if (product.Enterprise != enterpriseId)
                    {
                        category.Products.Remove(product.Id);
                        _logger.Warn(string.Format("Product '{0}' belongs to enterprise: '{1}' was about to be added to '{2}' Code:[hTrsvv563]", product.Id, product.Enterprise, enterpriseId));
                    }
                }
            }

           try
           {
               var productIds = menu.Categories.SelectMany(category => category.Products);
               var productDuplicates = productIds.GroupBy(p => p.ToUpper()).SelectMany(grp => grp.Skip(1));
               foreach (var productDuplicate in productDuplicates)
               {
                   _logger.Info(string.Format("Duplicate in products found: {0}, Enterprise: {1}", productDuplicate, enterpriseId));
                   //var dp = menu.Categories.SelectMany(p => p.Products.Where(product => product == productDuplicate));
                   //if(dp != null)
                   //    dp = GeneralHelper.GetGuid();
               }

               var categoryDuplicates = menu.Categories.GroupBy(c => c.Id.ToUpper()).SelectMany(grp => grp.Skip(1));
               foreach (var categoryDuplicate in categoryDuplicates)
               {
                   _logger.Info(string.Format("Duplicate in categories found: Name: {0}, Id: {1}, Enterprise: {2}", categoryDuplicate.Name,categoryDuplicate.Id,enterpriseId));
                   categoryDuplicate.Id = GeneralHelper.GetGuid();
               }
           }
            catch(Exception ex)
            {
                _logger.Fatal("ValidateMenu, duplicate check!",ex);
            }
        }
    }
}