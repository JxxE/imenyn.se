using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Rantup.Data.Abstract;
using Rantup.Data.Helpers;
using Rantup.Data.Models;
using Rantup.Web.Helpers;

namespace Rantup.Web.Controllers
{
    public class ManageController : BaseController
    {
        public ManageController(IRepository repository, IAuthentication authentication = null)
            : base(repository, authentication)
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEnterprise()
        {
            ViewBag.Counties = GeneralHelper.GetCountyNameAndCodes();
            ViewBag.Categories = GeneralHelper.GetCategories();
            return View();
        }

        public ActionResult Products(string enterpriseId)
        {
            if (enterpriseId == null)
                return RedirectToAction("Index");

            var enterprise = Repository.GetEnterpriseById(enterpriseId);
            if (enterprise == null)
                return RedirectToAction("Index");

            //Check if there already is a modified menu wating for approval
            var modifiedMenu = Repository.GetModifiedMenuByEnterpriseId(enterpriseId);
            if(modifiedMenu != null)
                return RedirectToAction("Index");

            ViewBag.Enterprise = enterprise.Name;
            ViewBag.EnterpriseId = enterpriseId;

            var isEdit = enterprise.Menu != null;

            if (isEdit)
            {
                ViewBag.IsEdit = true;
                var menu = Repository.GetMenuById(enterprise.Menu);
                var products = Repository.GetProducts(menu.Products.ToList());
                ViewBag.Products = products;
            }
            else
            {
                ViewBag.IsEdit = false;
            }

            return View();
        }

        public void SaveProducts(List<Product> products, string enterpriseId)
        {
            var enterprise = Repository.GetEnterpriseById(enterpriseId);
            if (enterprise == null) return;

            var isModified = enterprise.Menu != null;


            var updatedProducts = ProductHelper.InitiateProductList(products, enterprise.Key);

            var productIds = new List<string>();
            productIds.AddRange(updatedProducts.Select(updatedProduct => updatedProduct.Id));

            if (isModified)
            {               
                var modifiedMenu = new ModifiedMenu
                                       {
                                           Id = ModifiedMenuHelper.GetId(enterprise.Key),
                                           EnterpriseId = enterpriseId,
                                           ProductIds = productIds
                                       };
                Repository.CreateModifiedMenu(modifiedMenu);
            }
            else
            {
                var menu = new Menu
                {
                    Id = MenuHelper.GetId(enterprise.Key),
                    Products = productIds
                };

                enterprise.Menu = menu.Id;

                Repository.CreateMenu(menu);
                Repository.UpdateEnterprise(enterprise);
            }

            Repository.CreateProducts(updatedProducts);
        }

        public ActionResult CreateEnterprise(FormCollection form)
        {
            var name = form["name"];
            var phone = form["phone"];
            var address = form["address"];
            var postalCode = form["postalCode"];
            var city = form["city"].Contains(",") ? form["city"].Split(',').First() : form["city"];
            var countryCode = form["countryCode"] ?? "SE";
            var state_code = form["state_code"];
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

            var possibleKey = EnterpriseHelper.GenerateEnterpriseKey(name);
            var enterpriseExist = Repository.GetEnterpriseByUrlKey(possibleKey) != null;
            var key = enterpriseExist ? string.Format("{0}-{1}", possibleKey, GeneralHelper.RandomString(4)) : possibleKey;

            var id = EnterpriseHelper.GetId(key);

            var enterprise = new Enterprise
            {
                Name = name,
                Phone = phone,
                Address = address,
                PostalCode = postalCode,
                City = city,
                CountryCode = countryCode,
                StateCode = state_code,
                Coordinates = new Coordinates { Lat = lat, Lng = lng },
                YelpId = yelpId,
                Categories = categoryList,

                IsTemp = true,
                IsPremium = false,
                Key = key,
                Id = id,
                LastUpdated = DateTime.Now
            };

            Repository.CreateEnterprise(enterprise);
            
            return RedirectToAction("Products", "Manage", new { enterpriseId = id });
        }

    }
}
