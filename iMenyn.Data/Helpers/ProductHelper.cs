using System;
using AutoMapper;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;

namespace iMenyn.Data.Helpers
{
    public class ProductHelper
    {
        public static string GenerateId()
        {
            return GetId(GeneralHelper.GetGuid());
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

        public static Product UpdatedVersionToModel(ProductUpdatedVersion productUpdated)
        {
            Mapper.CreateMap<ProductUpdatedVersion, Product>();
            return Mapper.Map<ProductUpdatedVersion, Product>(productUpdated);
        }

        public static string GetId(string key)
        {
            return string.Format("products-{0}", key);
        }
    }
}