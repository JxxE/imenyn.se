using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iMenyn.Data.Abstract;
using iMenyn.Data.Models;
using iMenyn.Web.Helpers;
using iMenyn.Web.ViewModels;

namespace iMenyn.Web.Controllers
{
    public class CabaretController : BaseController
    {
        public CabaretController(IRepository repository) : base(repository) { }

        public ActionResult Index(string c)
        {
            if (!string.IsNullOrEmpty(c))
            {
                var enterprise = Repository.GetEnterpriseById(c);
                if (enterprise.IsPremium)
                {
                    return RedirectToAction("Premium", "Cabaret", new { id = enterprise.Id });
                }
                if (!enterprise.IsPremium)
                {
                    return RedirectToAction("Standard", "Cabaret", new { id = enterprise.Id });
                }
            }
            return View();
        }

        public ActionResult Premium(string id)
        {
            return View();
        }

        public ActionResult Standard(string id)
        {
            var enterprise = Repository.GetEnterpriseById(id);
            if (enterprise.IsTemp)
                return RedirectToAction("Index");

            if (enterprise.Menu == null)
                return RedirectToAction("NoMenu",new {id = enterprise.Id});

            var menu = Repository.GetMenuById(enterprise.Menu);
            var products = Repository.GetProducts(menu.Products.ToList());

            var model = ViewModelHelper.CreateStandardViewModel(enterprise, products);

            var recentlyModified = Repository.GetModifiedMenuByEnterpriseId(id) != null;

            model.RecentlyModified = recentlyModified;

            return View(model);
        }

        //TODO: Vad gör denna?
        public ActionResult NoMenu(string id)
        {
            //var enterprise = Repository.GetEnterpriseById(id);
            //var viewModel = new EnterprisesViewModel
            //                    {
            //                        Enterprises = new List<Enterprise>() {enterprise}
            //                    };
            return View();
        }

        public ActionResult Product(string id)
        {
            var product = Repository.GetProductById(id);
            ViewBag.Product = product;
            return View();
        }


    }
}
