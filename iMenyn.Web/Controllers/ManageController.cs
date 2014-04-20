using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Helpers;
using iMenyn.Data.Models;
using iMenyn.Web.ViewModels;

namespace iMenyn.Web.Controllers
{
    public class ManageController : BaseController
    {
        public ManageController(IDb db, ILogger logger)
            : base(db,logger)
        {
        }

        public ActionResult Index(string key)
        {
            var viewModel = new EnterpriseViewModel();

            if(!string.IsNullOrEmpty(key))
            {
                var enterprise = Db.Enterprises.GetEnterpriseById(EnterpriseHelper.GetId(key));
                if(enterprise != null && enterprise.IsNew)
                {
                    viewModel.Name = enterprise.Name;
                    viewModel.EditKey = key;

                    // Add chosen categories to viewmodel
                    var availableCategories = GeneralHelper.GetCategories();
                    viewModel.ChosenCategories = new List<ValueAndText>();
                    foreach (var c in enterprise.Categories)
                    {
                        var category = availableCategories.FirstOrDefault(p => p.Value == c);
                        if (category != null)
                            viewModel.ChosenCategories.Add(new ValueAndText { Text = category.Text, Value = c });
                    }
                }
            }

            return View(viewModel);
        }

        public ActionResult AddEnterprise()
        {
            ViewBag.Counties = GeneralHelper.GetCountyNameAndCodes();
            ViewBag.Categories = GeneralHelper.GetCategories();
            return View();
        }


        public ActionResult Products(string r)
        {
            if (r == null)
                return RedirectToAction("Index");
            
            var enterprise = Db.Enterprises.GetEnterpriseByUrlKey(r);
            if (enterprise == null)
            {
                Logger.Warn(string.Format("Enterprise (key: {0}) was null when editing",r));
                return RedirectToAction("Index");
            }

            //Check if there already is a modified menu wating for approval
            //var modifiedMenu = Repository.GetModifiedMenuByEnterpriseId(r);
            //if (modifiedMenu != null)
            //    return RedirectToAction("Index");

            ViewBag.EnterpriseName = enterprise.Name;
            ViewBag.EnterpriseKey = r;

            //var isEdit = enterprise.Menu != null;

            //if (isEdit)
            //{
            //    ViewBag.IsEdit = true;
            //    var menu = Repository.GetMenuById(enterprise.Menu);
            //    var products = Repository.GetProducts(menu.Products.ToList());
            //    ViewBag.Products = products;
            //}
            //else
            //{
            //    ViewBag.IsEdit = false;
            //}

            return View();
        }

        public Product SaveProduct(FormCollection productForm, string enterpriseKey)
        {
            //Kolla om enterpriset finns
            return new Product
                       {
                           Name = "HEJ",
                           Price = 56
                       };
        }

        public void SaveProducts(List<Product> products, string enterpriseId)
        {
            /*
             * Spara produkt:
             * Sparar en produkt o returnerar ID
             * 
             * */



            //TODO
            //var enterprise = Repository.GetEnterpriseById(enterpriseId);
            //if (enterprise == null) return;

            //var isModified = enterprise.Menu != null;


            //var updatedProducts = ProductHelper.InitiateProductList(products, enterprise.Key);

            //var productIds = new List<string>();
            //productIds.AddRange(updatedProducts.Select(updatedProduct => updatedProduct.Id));

            //if (isModified)
            //{               
            //    var modifiedMenu = new ModifiedMenu
            //                           {
            //                               Id = ModifiedMenuHelper.GetId(enterprise.Key),
            //                               EnterpriseId = enterpriseId,
            //                               ProductIds = productIds
            //                           };
            //    Repository.CreateModifiedMenu(modifiedMenu);
            //}
            //else
            //{
            //    var menu = new Menu
            //    {
            //        Id = MenuHelper.GetId(enterprise.Key),
            //        Products = productIds
            //    };

            //    enterprise.Menu = menu.Id;

            //    Repository.CreateMenu(menu);
            //    Repository.UpdateEnterprise(enterprise);
            //}

            //Repository.CreateProducts(updatedProducts);
        }

        [Obsolete("Using other method in JsonController now")]
        public ActionResult CreateEnterprise(FormCollection form)
        {
            //TODO
            if (!string.IsNullOrEmpty(form["nope"]))
                return RedirectToAction("Index", "Home");

            var name = form["name"];
            var phone = form["phone"];
            var address = form["address"];

            int postalCode;
            var postalCodeString = form["postalCode"].Replace(" ",string.Empty);
            int.TryParse(postalCodeString, out postalCode);

            var city = form["city"].Contains(",") ? form["city"].Split(',').First() : form["city"];
            var countryCode = form["countryCode"] ?? "SE";
            var state_code = form["state_code"];
            var subLocality = form["subLocality"];
            var lat = form["lat"];
            var lng = form["lng"];
            var yelpId = form["yelpId"];

            var categoryList = new List<string>();
            for (var i = 0; i < 6; i++)
            {
                var category = form["category" + i];
                if(category != null)
                {
                    categoryList.Add(category);
                }
            }

            //var possibleKey = EnterpriseHelper.GenerateEnterpriseKey(name, TODO);
            //var enterpriseExist = Db.Enterprises.GetEnterpriseByUrlKey(possibleKey) != null;
            //var key = enterpriseExist ? string.Format("{0}-{1}", possibleKey, GeneralHelper.RandomString(4)) : possibleKey;

            //var id = EnterpriseHelper.GetId(key);

            var enterprise = new Enterprise
            {
                Name = name,
                Phone = phone,
                Address = address,
                PostalCode = postalCode,
                City = city,
                CountryCode = countryCode,
                SubLocality = subLocality,
                StateCode = state_code,
                Coordinates = new Coordinates { Lat = lat, Lng = lng },
                YelpId = yelpId,
                Categories = categoryList,

                IsNew = true,
                IsPremium = false,
                LastUpdated = DateTime.Now
            };

            Db.Enterprises.CreateEnterprise(enterprise);
           
           return RedirectToAction("Products", "Manage", new { r = "" });
        }



    }
}
