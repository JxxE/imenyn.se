using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Helpers;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;

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
            return View(viewModel);
        }

        public ActionResult Edit(string id)
        {
            var viewModel = Db.Enterprises.GetCompleteEnterprise(EnterpriseHelper.GetId(id), true);
            return View(viewModel);
        }

        [AllowAnonymous]
        public ActionResult DemoMenu(bool edit=false)
        {
            var viewModel = Db.Enterprises.GetCompleteEnterprise("enterprises-jessetinell",edit);
            return View(viewModel);
        }


        // Detta gäller en NY enterprise. När man redigerar en nuvarande enterprise måste det sparas en TEMP-meny!
        [HttpPost]
        public int AddOrEditNewProduct(ProductViewModel product)
        {
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.Name) && (product.Prices != null && product.Prices.Count > 0))
                {
                    var p = ProductHelper.ViewModelToModel(product);
                    if (product.Id != null && Db.Products.GetProductById(product.Id) != null)
                    {
                        Db.Products.UpdateProduct(p,product.EnterpriseId);
                        return 10;
                    }

                    Db.Products.AddProduct(p, product.CategoryId, product.EnterpriseId);
                    return 20;
                }
            }
            return 30;
        }

        [HttpPost]
        public string CreateTempEnterprise(EnterpriseViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Nope) && string.IsNullOrEmpty(viewModel.Name))
                return string.Empty;

            //var name = form["name"];
            //var phone = form["phone"];

            //var key = EnterpriseHelper.GenerateEnterpriseKey(name, Db.Enterprises);
            //var id = EnterpriseHelper.GetId(key);

            ////Gatuadress
            //var streetNumber = form["street_number"];
            //var streetRoute = form["street_route"];

            ////Postnummer
            //int postalCode = 0;
            //if (!string.IsNullOrEmpty(viewModel.PostalCode))
            //{
            //    var postalCodeString = form["postal_code"].Replace(" ", string.Empty);
            //    int.TryParse(postalCodeString, out postalCode);
            //}

            var categoryList = new List<string>();
            categoryList.AddRange(viewModel.DisplayCategories.Take(6).Select(catgory => catgory.Value));

            //TODO generate search-tags

            var enterprise = new Enterprise
            {
                Name = viewModel.Name,
                Phone = viewModel.Phone,
                StreetNumber = viewModel.StreetNumber,
                StreetRoute = viewModel.StreetRoute,
                PostalCode = viewModel.PostalCode,
                PostalTown = viewModel.PostalTown,
                Commune = viewModel.Commune,
                County = viewModel.County,
                SubLocality = viewModel.SubLocality,
                CountryCode = viewModel.CountryCode ?? "SE",

                Coordinates = new Coordinates { Lat = viewModel.Coordinates.Lat, Lng = viewModel.Coordinates.Lng },
                Categories = categoryList,

                IsNew = true,
                OwnedByAccount = false,
                LockedFromEdit = false,

                LastUpdated = DateTime.Now,

                Menu = new Menu()
            };

            var key = viewModel.EditKey;
            if (!string.IsNullOrEmpty(key))
            {
                var enterpriseInDb = Db.Enterprises.GetEnterpriseById(EnterpriseHelper.GetId(key));
                //if(enterpriseInDb != null && enterpriseInDb.IsNew)
                //{
                //    enterprise.Id = enterpriseInDb.Id;
                //    Db.Enterprises.UpdateEnterprise(enterprise);
                //    return key;
                //}
            }
            else
            {
                //var enterpriseId = Db.Enterprises.CreateEnterprise(enterprise);
                return EnterpriseHelper.GetKey("asd");
            }

            return string.Empty;
        }

        //Sparar ordningen på menyn. Kategori, produkt-placering 
        [HttpPost]
        public void SaveMenuSetup(Menu menu, string enterpriseId)
        {
            Db.Enterprises.UpdateEnterprise(enterpriseId, menu);
        }

        public PartialViewResult BlankCategory(string enterpriseId)
        {
            return PartialView("~/Views/Partials/Menu/Edit/_Category.cshtml", new ViewModelCategory{Id=GeneralHelper.GetGuid(),Name = string.Empty,Products = new List<ProductViewModel>{new ProductViewModel{Id = ProductHelper.GenerateId()}},EnterpriseId = enterpriseId});
        }
        public PartialViewResult BlankProduct(string enterpriseId, string categoryId)
        {
            return PartialView("~/Views/Partials/Menu/Edit/_Product.cshtml", new ProductViewModel { Id = ProductHelper.GenerateId(), EnterpriseId = enterpriseId, CategoryId = categoryId });
        }
        public ViewResult BlankProductPrice()
        {
            return View("~/Views/Partials/Menu/Edit/_ProductPrice.cshtml", new ProductPrice());
        }
    }
}
