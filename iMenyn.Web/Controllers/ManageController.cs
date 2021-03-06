﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
                if ((enterprise.IsNew && !enterprise.LockedFromEdit) || (HttpContext.User.Identity.IsAuthenticated && CurrentAccount.IsAdmin))
                {
                    viewModel = enterprise;
                    viewModel.ShowForm = true;
                }
            }
            return View(viewModel);
        }

        public ActionResult Edit(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var viewModel = Db.Enterprises.GetCompleteEnterprise(EnterpriseHelper.GetId(key), true);

                if (viewModel.Enterprise.IsNew || viewModel.Enterprise.OwnedByAccount)
                {
                    if (viewModel.Enterprise.OwnedByAccount)
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

        // Detta gäller en NY enterprise. När man redigerar en nuvarande enterprise måste det sparas en TEMP-meny!
        [HttpPost]
        public ActionResult AddOrEditNewProduct(ProductViewModel product)
        {
            if (string.IsNullOrEmpty(product.Name))
                ModelState.AddModelError("Name", "Ange produktens namn");

            if ((product.Prices == null || product.Prices.Count < 1) || product.Prices.First().Price == 0)
                ModelState.AddModelError("Prices", "Ange ett pris");

            if (ModelState.IsValid)
            {
                var p = ProductHelper.ViewModelToModel(product);

                //Take a maximum of 5 prices
                p.Prices = p.Prices.Where(pr => pr.Price > 0).Take(5).ToList();

                if (product.Id != null && Db.Products.GetProductById(product.Id) != null)
                {
                    //if product isnot xatly the same!update
                    Db.Products.UpdateProduct(p, product.Enterprise);
                    return Json(new { success = true, method = "update" });
                }

                Db.Products.AddProduct(p, product.CategoryId, product.Enterprise);
                return Json(new { success = true, method = "add" });
            }

            return PartialView("~/Views/Partials/Menu/Edit/_Product.cshtml", product);
        }

        public ActionResult CreateTempEnterprise(EnterpriseViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Nope))
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrEmpty(viewModel.Name))
                ModelState.AddModelError("Name", "Ange restaurangens namn");

            if (viewModel.DisplayCategories == null || viewModel.DisplayCategories.Count < 1)
                ModelState.AddModelError("DisplayCategories", "Välj minst en kategori");
            else
            {
                viewModel.DisplayCategories = EnterpriseHelper.GetDisplayCategories(viewModel.DisplayCategories);
            }

            if (viewModel.Coordinates.Lat < 1 || viewModel.Coordinates.Lng < 1)
                ModelState.AddModelError("Coordinates", "Du måste ange någon platsinfo");

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
                return Json(new { url =  Url.Action("Edit", new { key = EnterpriseHelper.GetKey(enterprise.Id) }) });
            }

            viewModel.ShowForm = true;

            return PartialView("~/Views/Manage/_AddEnterpriseForm.cshtml", viewModel);
        }

        //Sparar ordningen på menyn. Kategori, produkt-placering 
        [HttpPost]
        public ActionResult SaveMenuSetup(Menu menu, string enterpriseId)
        {
            var updated = Db.Enterprises.UpdateEnterprise(enterpriseId, menu);
            return Json(new { success = updated });
        }

        public PartialViewResult BlankCategory(string enterpriseId)
        {
            var categoryId = GeneralHelper.GetGuid();
            return PartialView("~/Views/Partials/Menu/Edit/_Category.cshtml", new ViewModelCategory { Id = categoryId, Name = string.Empty, Products = new List<ProductViewModel>(), EnterpriseId = enterpriseId });
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
