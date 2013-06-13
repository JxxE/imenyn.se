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

                var postalCode = 0;
                if(business.location.postal_code != null)
                    int.TryParse(business.location.postal_code.Replace(" ",string.Empty), out postalCode);

                var enterprisesInDb = Repository.CheckIfEnterpriseExists(key, postalCode);
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

            var enterprisesViewModel = new EnterprisesViewModel
                                           {
                                               Enterprises = new List<EnterpriseViewModel>()
                                           };

            foreach (var enterpriseViewModel in enterprises.Select(enterprise => new EnterpriseViewModel
                                                                                     {
                                                                                         Id = enterprise.Id,
                                                                                         Name = enterprise.Name,
                                                                                         Address = enterprise.Address,
                                                                                         PostalCode = enterprise.PostalCode,
                                                                                         City = enterprise.City,
                                                                                         Categories = (from category in enterprise.Categories select GeneralHelper.GetCategories().FirstOrDefault(c => c.Value == category) into categoryToAdd where categoryToAdd != null select categoryToAdd.Text).ToList()
                                                                                     }))
            {
                enterprisesViewModel.Enterprises.Add(enterpriseViewModel);
            }


            return Json(enterprisesViewModel);
        }

        public JsonResult GetEnterprisesByStateCode(string stateCode)
        {
            var enterprises = Repository.GetEnterprisesByStateCode(stateCode);

            var districts = enterprises.Select(enterprise => enterprise.City).Distinct().OrderBy(d=>d).ToList();

            var viewModel = new BrowseViewModel
                {
                    Districts = districts,
                    StateCode = stateCode
                };

            return Json(viewModel);
        }
    }
}
