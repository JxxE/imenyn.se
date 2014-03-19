﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iMenyn.Data.Helpers;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;

namespace iMenyn.Data.Helpers
{    
    public class ViewModelHelper
    {
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
                    Key = foodType.Key,
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

            //Sort productTypes
            viewModel.FoodTypes = viewModel.FoodTypes.OrderBy(type =>
                                        type.Key == "main-dish"
                                            ? 1
                                            : type.Key == "drink" ? 2 :
                                            type.Key == "extra" ? 3 : 4
                ).ToList();

            return viewModel;
        }

    }
}