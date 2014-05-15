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
            if (!string.IsNullOrEmpty(key))
            {
                var enterprise = Db.Enterprises.GetCompleteEnterprise(EnterpriseHelper.GetId(key)).Enterprise;
                if (enterprise.IsNew && !enterprise.LockedFromEdit)
                    viewModel = enterprise;
            }
            return View(viewModel);
        }

        public ActionResult Edit(string key)
        {
            if(!string.IsNullOrEmpty(key))
            {
                var viewModel = Db.Enterprises.GetCompleteEnterprise(EnterpriseHelper.GetId(key), true);

                if (viewModel.Enterprise.IsNew || viewModel.Enterprise.OwnedByAccount)
                {
                    if(viewModel.Enterprise.OwnedByAccount)
                    {
                        var account = Db.Accounts.GetAccount(HttpContext.User.Identity.Name);
                        if (account.Enabled && account.Enterprises.Contains(EnterpriseHelper.GetId(key)))
                        {
                            //If account is enabled and contains this enterprise
                            return View(viewModel);
                        }
                    }
                    else
                    {
                        return View(viewModel);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult DemoMenu(bool edit = false)
        {
            var viewModel = Db.Enterprises.GetCompleteEnterprise("enterprises-jessetinell", edit);
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
                        Db.Products.UpdateProduct(p, product.Enterprise);
                        return 10;
                    }

                    Db.Products.AddProduct(p, product.CategoryId, product.Enterprise);
                    return 20;
                }
            }
            return 30;
        }

        [HttpPost]
        public ActionResult Index(EnterpriseViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Nope))
                return RedirectToAction("Index","Home");

            if (string.IsNullOrEmpty(viewModel.Name))
                ModelState.AddModelError("Name", "Ange restaurangens namn");

            if (viewModel.DisplayCategories == null || viewModel.DisplayCategories.Count < 1)
                ModelState.AddModelError("DisplayCategories", "Välj minst en kategori");

            if (viewModel.Coordinates.Lat == null || viewModel.Coordinates.Lng == null)
                ModelState.AddModelError("Coordinates","Du måste ange någon platsinfo");

            if (ModelState.IsValid)
            {
                var categoryList = new List<string>();
                categoryList.AddRange(viewModel.DisplayCategories.Take(6).Select(catgory => catgory.Value));

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

                    SearchTags = EnterpriseHelper.GenerateSearchTags(viewModel.Name),

                    Coordinates = new Coordinates { Lat = viewModel.Coordinates.Lat, Lng = viewModel.Coordinates.Lng },
                    Categories = categoryList,

                    IsNew = true,
                    OwnedByAccount = false,
                    LockedFromEdit = false,

                    LastUpdated = DateTime.Now,

                    Menu = new Menu()
                };

                if (string.IsNullOrEmpty(viewModel.Id))
                {
                    enterprise.Id = EnterpriseHelper.GetId(GeneralHelper.GetGuid());
                    Db.Enterprises.CreateEnterprise(enterprise);
                }
                else
                {
                    var enterpriseInDb = Db.Enterprises.GetEnterpriseById(enterprise.Id);
                    if (enterpriseInDb != null)
                    {
                        Db.Enterprises.UpdateEnterprise(enterprise);
                    }
                }
                return RedirectToAction("Edit", new { key = EnterpriseHelper.GetKey(enterprise.Id) });
            }

            viewModel.DisplayCategories = EnterpriseHelper.GetDisplayCategories(viewModel.DisplayCategories);

            return View(viewModel);
        }

        //Sparar ordningen på menyn. Kategori, produkt-placering 
        [HttpPost]
        public void SaveMenuSetup(Menu menu, string enterpriseId)
        {
            Db.Enterprises.UpdateEnterprise(enterpriseId, menu);
        }

        public PartialViewResult BlankCategory(string enterpriseId)
        {
            var categoryId = GeneralHelper.GetGuid();
            return PartialView("~/Views/Partials/Menu/Edit/_Category.cshtml", new ViewModelCategory { Id = categoryId, Name = string.Empty, Products = new List<ProductViewModel> { new ProductViewModel { Id = ProductHelper.GenerateId(), Enterprise = enterpriseId, CategoryId = categoryId } }, EnterpriseId = enterpriseId });
        }
        public PartialViewResult BlankProduct(string enterpriseId, string categoryId)
        {
            return PartialView("~/Views/Partials/Menu/Edit/_Product.cshtml", new ProductViewModel { Id = ProductHelper.GenerateId(), Enterprise = enterpriseId, CategoryId = categoryId });
        }
        public ViewResult BlankProductPrice()
        {
            return View("~/Views/Partials/Menu/Edit/_ProductPrice.cshtml", new ProductPrice());
        }
    }
}
