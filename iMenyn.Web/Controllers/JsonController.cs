using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using iMenyn.Data.Abstract;
using iMenyn.Data.Helpers;
using iMenyn.Data.Models;
using iMenyn.Web.ViewModels;
using iMenyn.Yelp.Data;

namespace iMenyn.Web.Controllers
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
                if (business.location.postal_code != null)
                    int.TryParse(business.location.postal_code.Replace(" ", string.Empty), out postalCode);

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

        public JsonResult SearchEnterprises(string searchTerm)
        {
            var enterprises = Repository.SearchEnterprises(searchTerm, "", "");

            var enterprisesViewModel = new EnterprisesViewModel
                                           {
                                               Enterprises = new List<EnterpriseViewModel>()
                                           };

            foreach (var enterpriseViewModel in enterprises.Select(enterprise => new EnterpriseViewModel
                                                                                     {
                                                                                         Key = enterprise.Key,
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



        public JsonResult BrowseEnterprises(string stateCode, string city)
        {
            BrowseViewModel viewModel;

            if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(stateCode))
                viewModel = GetEnterprisesByCity(city, stateCode);
            else
                viewModel = GetEnterprisesByStateCode(stateCode);

            return Json(viewModel);
        }

        private BrowseViewModel GetEnterprisesByStateCode(string stateCode)
        {
            var enterprises = Repository.GetEnterprisesByLocation(stateCode, null);

            var districts = enterprises.Select(enterprise => enterprise.City).Distinct().OrderBy(d => d).ToList();

            var viewModel = new BrowseViewModel
            {
                Districts = districts,
                StateCode = stateCode
            };

            return viewModel;
        }

        private BrowseViewModel GetEnterprisesByCity(string city, string stateCode)
        {
            var enterprises = Repository.GetEnterprisesByLocation(stateCode, city);

            var viewModel = new BrowseViewModel
            {
                Enterprises = enterprises
            };

            return viewModel;
        }

        public JsonResult GetGeneralLocationInfoByAddress(string address)
        {
            var lowerAddress = address.ToLower();
            var formattedAddress = lowerAddress.Replace(", sverige", string.Empty);
            // Prewview URL http://maps.googleapis.com/maps/api/geocode/json?address=sk%C3%B6nstav%C3%A4gen%203&sensor=false&region=se
            var url = "http://maps.googleapis.com/maps/api/geocode/json?address=" + formattedAddress + "&sensor=false&region=se";

            var wc = new WebClient {Encoding = Encoding.UTF8};

            var json = wc.DownloadString(url);

            var jo = JObject.Parse(json);

            var results = jo["results"][0];

            var status = jo["status"];

            if (status.ToString() == "OK")
            {
                var viewModel = new LocationViewModel
                                    {
                                        Location = new Location(),
                                        Coordinates = new Coordinates()
                                    };

                var address_components = results["address_components"];

                var route = "";
                var streetNumber = "";
                var lan = "";
                var locality = "";
                var administrative_area_level_2 = "";

                foreach (var addressComponent in address_components)
                {
                    var type = addressComponent.SelectToken("types").First.Value<string>();

                    switch(type)
                    {
                        case "street_number":
                            streetNumber = addressComponent.SelectToken("long_name").Value<string>();
                            break;
                        case "route":
                            route = addressComponent.SelectToken("long_name").Value<string>();
                            break;
                        case "sublocality":
                            viewModel.Location.sub_locality = addressComponent.SelectToken("long_name").Value<string>();
                            break;
                        case "locality":
                            locality = addressComponent.SelectToken("long_name").Value<string>();
                            break;
                        case "administrative_area_level_1":
                            lan = addressComponent.SelectToken("long_name").Value<string>().ToLower().Replace("s län",string.Empty).Replace(" county",string.Empty); //TODO!!!!! BETTER!?!?!
                            break;
                        case "administrative_area_level_2":
                            administrative_area_level_2 = addressComponent.SelectToken("long_name").Value<string>();
                            break;
                        case "postal_code":
                            viewModel.Location.postal_code = addressComponent.SelectToken("long_name").Value<string>();
                            break;
                        case "postal_town":
                            viewModel.Location.postal_town = addressComponent.SelectToken("long_name").Value<string>();
                            break;

                    }
                }

                var lat = results["geometry"]["location"]["lat"].ToString().Replace(",", ".");
                var lng = results["geometry"]["location"]["lng"].ToString().Replace(",", ".");
                viewModel.Coordinates.Lat = lat;
                viewModel.Coordinates.Lng = lng;

                //Give streetnumber a space to the left so it looks good with the address.
                streetNumber = streetNumber == "" ? string.Empty : " " + streetNumber;

                viewModel.Location.complete_address = string.Format("{0}{1}", route, streetNumber);

                //Sätt kommun
                viewModel.Location.county = administrative_area_level_2 != "" ? administrative_area_level_2 : locality;

                var stateCode = GeneralHelper.GetCountyNameAndCodes().FirstOrDefault(p => p.Text.ToLower().Contains(lan));
                if (stateCode != null && stateCode.Value.Length < 3)
                    viewModel.Location.state_code = stateCode.Value;

                viewModel.Counties = GeneralHelper.GetCountyNameAndCodes();

                return Json(viewModel);
            }
            return null;
        }

        public JsonResult GetEnterprisesCloseToMyLocation(string latitude, string longitude)
        {
            var enterprisesCloseToMe = new Dictionary<string, double>();

            var lat = double.Parse(latitude, CultureInfo.InvariantCulture);
            var lng = double.Parse(longitude, CultureInfo.InvariantCulture);

            var myCoord = new GeoCoordinate(lat, lng);

            var allEnterprises = Repository.GetAllEnterprises();

            foreach (var enterprise in allEnterprises)
            {
                var enterpriseLat = double.Parse(enterprise.Coordinates.Lat,CultureInfo.InvariantCulture);
                var enterpriseLng = double.Parse(enterprise.Coordinates.Lng,CultureInfo.InvariantCulture);
                var enterpriseCoord = new GeoCoordinate(enterpriseLat, enterpriseLng);

                var distance = myCoord.GetDistanceTo(enterpriseCoord);

                if (distance < 15000)
                {
                    enterprisesCloseToMe.Add(enterprise.Id, distance);
                }
            }

            var sortedEnterprisesCloseToMe =
                (from e in enterprisesCloseToMe orderby e.Value ascending select e).ToDictionary(p => p.Key,
                                                                                                 p => p.Value);

            var enterprisesViewModel = new EnterprisesViewModel
            {
                Enterprises = new List<EnterpriseViewModel>()
            };


            foreach (var d in sortedEnterprisesCloseToMe)
            {
                var en = allEnterprises.ToList().SingleOrDefault(e => e.Id == d.Key);
                if(en == null)continue;
                var enterpriseViewModel = new EnterpriseViewModel
                                              {
                                                  Address = en.Address,
                                                  Categories = en.Categories,
                                                  City = en.City,
                                                  Key = en.Key,
                                                  Name = en.Name,
                                                  PostalCode = en.PostalCode,
                                                  DistanceFromMyLocation = Math.Round((d.Value/1000),1)
                                              };
                enterprisesViewModel.Enterprises.Add(enterpriseViewModel);
            }

            return Json(enterprisesViewModel);
        }
    }
}