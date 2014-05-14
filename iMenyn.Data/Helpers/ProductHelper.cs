using System;
using System.Collections.Generic;
using System.Linq;
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

        public static List<ProductViewModel> ModelToViewModel(List<Product> models)
        {
            return models.Select(ModelToViewModel).ToList();
        }

        public static ProductViewModel ModelToViewModel(Product model)
        {
            Mapper.CreateMap<Product, ProductViewModel>();
            return Mapper.Map<Product, ProductViewModel>(model);
        }

        public static Product AddUpdatedInfoToProduct(Product product)
        {
            product.Name = product.UpdatedVersion.Name;
            product.Description = product.UpdatedVersion.Description;
            product.Abv = product.UpdatedVersion.Abv;
            product.Prices = product.UpdatedVersion.Prices;
            product.Image = product.UpdatedVersion.Image;
            return product;
        }

        public static string GetId(string key)
        {
            return string.Format("products-{0}", key);
        }
    }
}