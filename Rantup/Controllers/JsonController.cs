using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rantup.Data.Abstract;
using Rantup.Data.Helpers;
using Rantup.Data.Models;
using Rantup.Web.Extensionmethods;
using Rantup.Web.Helpers;
using Rantup.Web.ViewModels;

namespace Rantup.Web.Controllers
{
    public class JsonController : BaseController
    {
        public JsonController(IRepository repository, IAuthentication authentication = null)
            : base(repository, authentication)
        {
        }

        public JsonResult SearchYelp(string searchTerm, string location)
        {
            var yelp = new Yelp.Yelp(YelpConfig.Options);
            var search = yelp.Search(searchTerm, location);

            var categories = GeneralHelper.GetCategories();

            foreach (var business in search.Result.businesses)
            {
                var key = EnterpriseHelper.GenerateEnterpriseKey(business.name);

                var enterprisesInDb = Repository.CheckIfEnterpriseExists(key, business.location.postal_code);
                if (enterprisesInDb == null) continue;

                var enterprises = enterprisesInDb as Enterprise[] ?? enterprisesInDb.ToArray();

                var mayExistInDb = enterprises.Any();
                if (!mayExistInDb) continue;

                business.MayAlreadyExistInDb = true;
                var urlsForMenus = new List<string>();
                foreach (var enterprise in enterprises)
                {
                    if (enterprise.IsTemp)
                        urlsForMenus.Add("");
                    else
                    {
                        var kind = enterprise.IsPremium ? "Premium" : "Standard";
                        var url = string.Format("{0}/{1}", kind, enterprise.Id);
                        urlsForMenus.Add(url);
                    }
                }
                business.MenyUrls = urlsForMenus;
            }


            var viewModel = new SearchYelpViewModel
                                {
                                    Businesses = search.Result.businesses,
                                    Categories = categories
                                };

            return Json(viewModel);
        }

        public JsonResult SearchEnterprises(string searchTerm, string location, string categorySearch)
        {
            var enterprises = Repository.SearchEnterprises(searchTerm, location, categorySearch);
            var viewModel = new EnterprisesViewModel
                                {
                                    Enterprises = enterprises
                                };
            return Json(viewModel);
        }
    }
}
