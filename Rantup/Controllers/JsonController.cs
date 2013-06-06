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
