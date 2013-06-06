using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rantup.Data.Helpers;
using Rantup.Data.Models;
using Rantup.Web.ViewModels;

namespace Rantup.Web.Helpers
{
    public class ViewModelHelper
    {
        public static StandardViewModel CreateStandardViewModel(Enterprise enterprise, IEnumerable<Product> products)
        {
            var viewModel = new StandardViewModel
            {
                Enterprise = enterprise,
                Phone = enterprise.Phone.Replace(" ",string.Empty).Replace("-",string.Empty),
                Products = GetProductListViewModel(products)
            };

            return viewModel;
        }


        public static ProductListViewModel GetProductListViewModel(IEnumerable<Product> products)
        {
            var viewModel = new ProductListViewModel
                                {
                                    FoodTypes = new List<FoodType>()
                                };

            foreach (var foodType in products.GroupBy(p=>p.ProductType))
            {
                var ft = new FoodType 
                {
                    Name = ProductHelper.FormatProductType(foodType.Key),
                    Categories = new List<Category>()
                };

                foreach (var category in foodType.GroupBy(f=>f.Category))
                {
                    var c = new Category
                                {
                                    Name =  category.Key,
                                    Products = new List<Product>()
                                };
                    foreach (var product in category)
                    {
                        c.Products.Add(product);
                    }
                    ft.Categories.Add(c);
                }

                //Sortera på kategorinamn
                ft.Categories = ft.Categories.OrderBy(c => c.Name).ToList();

                viewModel.FoodTypes.Add(ft);

            }
            return viewModel;
        }

    }
}