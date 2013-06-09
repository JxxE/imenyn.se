using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rantup.Data.Models;

namespace Rantup.Data.Helpers
{
    public class ProductHelper
    {
        static readonly Random _random = new Random();

        public static List<Product> InitiateProductList(IEnumerable<Product> products, string enterpriseKey)
        {
            var productList = new List<Product>();
            foreach (var product in products)
            {
                product.Id = GenerateProductId(enterpriseKey);
                productList.Add(product);
            }
            return productList;
        }

        public static string GenerateProductId(string enterpriseKey)
        {
            return string.Format("product-{0}-{1}", enterpriseKey, RandomString());
        }

        private static string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(
                Enumerable.Repeat(chars, 12)
                          .Select(s => s[_random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        public static string FormatProductType(string productType)
        {
            var formatted = "";

            ProductType type;
            if (Enum.TryParse(productType, out type))
            {
                switch (type)
                {
                    case ProductType.Drink:
                        formatted = "Drycker";
                        break;
                    case ProductType.Appetizer:
                        formatted = "Förrätter";
                        break;
                    case ProductType.MainDish:
                        formatted = "Varmrätter";
                        break;
                    case ProductType.Dessert:
                        formatted = "Efterrätter";
                        break;
                    default:
                        formatted = "";
                        break;
                }
            }

            if(formatted == "")
            {
                switch (productType)
                {
                    case "drink":
                        formatted = "Dryck";
                        break;
                    case "appetizer":
                        formatted = "Förrätt";
                        break;
                    case "main-dish":
                        formatted = "Varmrätt";
                        break;
                    case "desert":
                        formatted = "Efterrätt";
                        break;
                    case "extra":
                        formatted = "Tillbehör";
                        break;
                    default:
                        formatted = "";
                        break;
                }
            }

            return formatted;
        }
    }
}