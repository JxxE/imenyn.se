using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Helpers;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;
using iMenyn.Web.ViewModels;
using Category = iMenyn.Data.Models.Category;

namespace iMenyn.Web.Controllers
{
    public class ManageController : BaseController
    {
        public ManageController(IDb db, ILogger logger)
            : base(db, logger)
        {
        }

        public ActionResult Index(string key)
        {
            var viewModel = new EnterpriseViewModel();

            //if (!string.IsNullOrEmpty(key))
            //{
            //    var enterprise = Db.Enterprises.GetEnterpriseById(EnterpriseHelper.GetId(key));
            //    if (enterprise != null && enterprise.IsNew)
            //    {
            //        viewModel.Name = enterprise.Name;
            //        viewModel.Phone = enterprise.Name;

            //        // Geolocation
            //        viewModel.StreetNumber = enterprise.StreetNumber;
            //        viewModel.StreetRoute = enterprise.StreetRoute;
            //        viewModel.PostalCode = enterprise.PostalCode;
            //        viewModel.PostalTown = enterprise.PostalTown;
            //        viewModel.Commune = enterprise.Commune;
            //        viewModel.County = enterprise.County;

            //        // Hidden fields
            //        viewModel.CountryCode = enterprise.CountryCode;
            //        viewModel.SubLocality = enterprise.SubLocality;
            //        viewModel.Coordinates = enterprise.Coordinates;
            //        viewModel.EditKey = key;


            //        // Add chosen categories to viewmodel
            //        var availableCategories = GeneralHelper.GetCategories();
            //        viewModel.ChosenCategories = new List<ValueAndText>();
            //        foreach (var c in enterprise.Categories)
            //        {
            //            var category = availableCategories.FirstOrDefault(p => p.Value == c);
            //            if (category != null)
            //                viewModel.ChosenCategories.Add(new ValueAndText { Text = category.Text, Value = c });
            //        }
            //    }
            //}

            CreateMenu();

            return View(viewModel);
        }

        [AllowAnonymous]
        public ActionResult DemoMenu()
        {
            var viewModel = Db.Enterprises.GetCompleteEnterprise("enterprises-1");

            return View(viewModel);
        }

        public ActionResult AddEnterprise()
        {
            ViewBag.Counties = GeneralHelper.GetCountyNameAndCodes();
            ViewBag.Categories = GeneralHelper.GetCategories();
            return View();
        }

        public void CreateMenu()
        {
            var enterprise = Db.Enterprises.GetEnterpriseById("enterprises-1");

            var menu = new Menu();
            var categories = new List<Category>();
            var drinks = new List<string>();
            var pizzas = new List<string>();
            var allProducts = new List<Product>();


            var p1 = new Product { Id = ProductHelper.GenerateId(), Name = "Vesuvio", Prices = new List<ProductPrice>{new ProductPrice{Price = 74}}};
            var p2 = new Product { Id = ProductHelper.GenerateId(), Name = "Oscar", Prices = new List<ProductPrice> { new ProductPrice { Price = 55 } } };
            pizzas.Add(p1.Id);
            pizzas.Add(p2.Id);

            var p3 = new Product { Id = ProductHelper.GenerateId(), Name = "Heineken", Prices = new List<ProductPrice> { new ProductPrice { Price = 85 } } };
            var p4 = new Product { Id = ProductHelper.GenerateId(), Name = "Xider", Prices = new List<ProductPrice> { new ProductPrice { Price = 35 } } };
            drinks.Add(p3.Id);
            drinks.Add(p4.Id);

            allProducts.Add(p1);
            allProducts.Add(p2);
            allProducts.Add(p3);
            allProducts.Add(p4);

            var c1 = new Category
                         {
                             Id = Guid.NewGuid().ToString("N"),
                             Name = "Dryck",
                             Products = drinks
                         };
            var c2 = new Category
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Pizza",
                Products = pizzas
            };

            categories.Add(c1);
            categories.Add(c2);

            menu.Categories = categories;

            enterprise.Menu = menu;

            //Db.Enterprises.UpdateEnterprise(enterprise,allProducts);
            //Db.Menus.CreateMenu(enterprise, allProducts);

        }

        // Detta gäller en NY enterprise. När man redigerar en nuvarande enterprise måste det sparas en TEMP-meny!
        [HttpPost]
        public void AddOrEditNewProduct(Product product,string categoryId,string enterpriseId)
        {
            if(product != null)
            {
                if (product.Id != null && Db.Products.GetProductById(product.Id) != null)
                {
                    Db.Products.UpdateProduct(product);
                }
                else
                {
                    Db.Products.AddProduct(product, categoryId,enterpriseId);
                }
            }
        }

        //Sparar ordningen på menyn. Kategori, produkt-placering 
        [HttpPost]
        public void SaveMenuSetup(Menu menu,string enterpriseId)
        {
            Db.Enterprises.UpdateEnterprise(enterpriseId,menu);
        }

    }
}
