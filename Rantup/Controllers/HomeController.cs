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

        public ActionResult Info()
        {
            return View();
        }

        public ActionResult Browse()
        {
            var enterprises = Repository.GetAllEnterprises();

            var enterpriseStateCodes = enterprises.Select(enterprise => enterprise.StateCode).Distinct().ToList();

            var counties = GeneralHelper.GetCountyNameAndCodes();

            var stateCodesAndNames = new List<ValueAndText>();
            
            foreach (var enterpriseStateCode in enterpriseStateCodes)
            {
                var stateCodeAndName = new ValueAndText
                    {
                        Value = enterpriseStateCode,
                        Text = counties.First(c=>c.Value == enterpriseStateCode).Text
                    };
                stateCodesAndNames.Add(stateCodeAndName);
            }

            var viewModel = new BrowseViewModel
            {
                EnterpriseStateCodes = new List<ValueAndText>()
            };

            viewModel.EnterpriseStateCodes = stateCodesAndNames.OrderBy(s=>s.Text).ToList();

            return View(viewModel);
        }
    }
}
