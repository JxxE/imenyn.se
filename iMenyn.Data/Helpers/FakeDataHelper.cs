using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using iMenyn.Data.Models;
using AutoMapper;

namespace iMenyn.Data.Helpers
{
    public static class FakeDataHelper
    {
        private static readonly Random _random = new Random((int)DateTime.Now.Ticks);

        public static Enterprise CreateFakeEnterprise(Abstract.Db.IDb Db,bool modified)
        {
            var enterprise = new Enterprise
                                 {
                                     Id = EnterpriseHelper.GetId(GeneralHelper.GetGuid()),
                                     Name = RandomString(),
                                     Categories = RandomListString(),
                                     LastUpdated = DateTime.Now
                                 };
            var menu = new Menu();
            var categories = new List<Category>();

            var products = new List<Product>();

            for (var i = 0; i < _random.Next(6, 20); i++)
            {
                var category = new Category
                                      {
                                          Id = GeneralHelper.GetGuid(),
                                          Name = RandomString(),
                                          Products = new List<string>()
                                      };
                for (var j = 0; j < _random.Next(6, 40); j++)
                {
                    var product = NewProduct(enterprise.Id,modified);


                    products.Add(product);
                    category.Products.Add(product.Id);
                }
                categories.Add(category);
            }


            menu.Categories = categories;


            if(!modified)
            {
                enterprise.IsNew = true; 
            }

            enterprise.Menu = menu;
            
            Db.Enterprises.CreateEnterprise(enterprise);
            Db.Products.AddProductsToDb(products);

            if(modified)
            {
                Thread.Sleep(1000);
                var categoryCount = enterprise.Menu.Categories.Count;

                //Remove random products
                for (var i = 0; i < _random.Next(1, 4); i++)
                {
                    var randomCategory = enterprise.Menu.Categories[_random.Next(categoryCount)];
                    var productCountForCategory = randomCategory.Products.Count;
                    enterprise.Menu.Categories[_random.Next(categoryCount)].Products.RemoveAt(_random.Next(productCountForCategory));
                }

                //Create new products
                var newProducts = new List<Product>();
                for (var i = 0; i < _random.Next(1, 8);i++ )
                {
                    newProducts.Add(NewProduct(enterprise.Id,false));
                }

                //Add the new products to random categories in random places
                foreach (var newProduct in newProducts)
                {
                    var randomCategory = enterprise.Menu.Categories[_random.Next(categoryCount)];
                    var productCountForCategory = randomCategory.Products.Count;
                    randomCategory.Products.Insert(_random.Next(productCountForCategory),newProduct.Id);
                }

                Db.Products.AddProductsToDb(newProducts);
                Thread.Sleep(1000);
                Db.Enterprises.UpdateEnterprise(enterprise.Id, enterprise.Menu);
            }

            return enterprise;
        }

        private static Product NewProduct(string enterpriseId,bool modified)
        {
            var product = new Product
            {
                Id = ProductHelper.GenerateId(),
                Enterprise = enterpriseId,
                Name = RandomString(),
                Prices = new List<ProductPrice>()
            };

            if (RandomBool())
                product.Description = RandomString();

            var productPrices = new List<ProductPrice>();
            for (var k = 0; k < _random.Next(1, 4); k++)
            {
                var productPrice = new ProductPrice { Price = RandomNumber() };
                if (RandomBool())
                    productPrice.Description = RandomString();

                productPrices.Add(productPrice);
            }

            product.Prices = productPrices;

            if (modified)
            {
                if (RandomBool())
                {
                    Mapper.CreateMap<Product, ProductUpdatedVersion>();
                    var updatedProduct = new ProductUpdatedVersion();
                    Mapper.Map(product, updatedProduct);
                    updatedProduct.Description = RandomString();
                    product.UpdatedVersion = updatedProduct;
                }
            }

            return product;
        }

        private static string RandomString()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < _random.Next(4,15); i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }
        private static int RandomNumber()
        {
            return _random.Next(10,200);
        }
        private static bool RandomBool()
        {
            return _random.Next(3) == 1;
        }
        private static List<string> RandomListString(int maxLength = 5)
        {
            var list = new List<string>();
            for (var i = 0; i < _random.Next(1, maxLength); i++)
            {
                list.Add(RandomString());
            }
            return list;
        }
    }
}