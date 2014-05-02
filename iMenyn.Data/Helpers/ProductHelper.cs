using System;
using AutoMapper;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;

namespace iMenyn.Data.Helpers
{
    public class ProductHelper
    {
        [Obsolete("Only used for testing")]
        public static string GenerateId()
        {
            return string.Format("products-{0}", Guid.NewGuid().ToString("N"));
        }

        public static Product ViewModelToModel(ProductViewModel viewmodel)
        {
            Mapper.CreateMap<ProductViewModel, Product>();
            return Mapper.Map<ProductViewModel, Product>(viewmodel);
        }
        public static ProductViewModel ModelToViewModel(Product model)
        {
            Mapper.CreateMap<Product, ProductViewModel>();
            return Mapper.Map<Product, ProductViewModel>(model);
        }

        public static string GetId(string key)
        {
            return string.Format("products-{0}", key);
        }
    }
}