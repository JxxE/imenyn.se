using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Raven.Client.Document;
using Raven.Imports.Newtonsoft.Json.Linq;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Helpers;
using iMenyn.Data.Models;
using iMenyn.Web.Models;
using iMenyn.Web.ViewModels;

namespace iMenyn.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IDb db, ILogger logger)
            : base(db, logger)
        {
        }


        public ActionResult Index(string q)
        {
            //var viewModel = new MainSearchViewModel
            //{
            //    Counties = GeneralHelper.GetCountyNameAndCodes()
            //};

            //if (q == null)
            //{
            //    return View(viewModel);
            //}

            //var enterprise = Repository.GetEnterpriseByUrlKey(q);
            //if (enterprise != null)
            //{

            //}

            return View();
        }

        public ActionResult Info()
        {
            return View();
        }

        public ActionResult Browse()
        {
            //TODO
            //var enterprises = Repository.GetAllEnterprises();

            //var enterpriseStateCodes = enterprises.Select(enterprise => enterprise.StateCode).Distinct().ToList();

            //var counties = GeneralHelper.GetCountyNameAndCodes();

            //var stateCodesAndNames = new List<ValueAndText>();
            
            //foreach (var enterpriseStateCode in enterpriseStateCodes)
            //{
            //    var stateCodeAndName = new ValueAndText
            //        {
            //            Value = enterpriseStateCode,
            //            Text = counties.First(c=>c.Value == enterpriseStateCode).Text
            //        };
            //    stateCodesAndNames.Add(stateCodeAndName);
            //}

            //var viewModel = new BrowseViewModel
            //{
            //    EnterpriseStateCodes = new List<ValueAndText>()
            //};

            //viewModel.EnterpriseStateCodes = stateCodesAndNames.OrderBy(s=>s.Text).ToList();

            //return View(viewModel);
            return View();
        }
    }
}
