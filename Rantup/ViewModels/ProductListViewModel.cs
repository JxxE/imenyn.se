using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rantup.Data.Models;

namespace Rantup.Web.ViewModels
{
    public class ProductListViewModel
    {
        public List<FoodType> FoodTypes { get; set; }
    }

    public class FoodType
    {
        public string Name { get; set; }
        public ProductType ProductType { get; set; }
        public List<Category> Categories { get; set; }
    }

    public class Category
    {
        public string Name { get; set; }
        public List<Product> Products { get; set; }

        public string ProductType { get; set; }
    }
}