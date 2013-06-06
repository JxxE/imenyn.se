using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Rantup.Data.Abstract;
using Rantup.Data.Helpers;
using Rantup.Data.Models;
using Rantup.Web.Extensionmethods;
using Rantup.Web.Helpers;
using Rantup.Web.Infrastructure;
using Rantup.Web.ViewModels;
using Rantup.Yelp;

namespace Rantup.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IRepository repository)
            : base(repository)
        {
        }


        public ActionResult Index(string q)
        {
            var viewModel = new MainSearchViewModel
            {
                Counties = GeneralHelper.GetCountyNameAndCodes()
            };

            if (q == null)
            {
                return View(viewModel);
            }

            var enterprise = Repository.GetEnterpriseByUrlKey(q);
            if (enterprise != null)
            {

            }
            else
            {
            }

            return View(viewModel);
        }

        public ActionResult Search()
        {
            return View();
        }
    }
}
