using System.Collections.Generic;
using iMenyn.Data.Models;

namespace iMenyn.Web.ViewModels
{
    public class ProductListViewModel
    {
        public List<FoodType> FoodTypes { get; set; }
    }

    public class FoodType
    {
        public string Name { get; set; }
        public string Key { get; set; }
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