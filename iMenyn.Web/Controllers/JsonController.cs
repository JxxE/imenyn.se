using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Helpers;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;
using iMenyn.Web.ViewModels;

namespace iMenyn.Web.Controllers
{
    public class JsonController : BaseController
    {
        public JsonController(IDb db, ILogger logger)
            : base(db, logger)
        {
        }

        public JsonResult MainSearch(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return Json(null);

            var enterprises = Db.Enterprises.MainSearch(searchTerm);

            var searchViewModel = new MainSearchViewModel
                                           {
                                               SearchQuery = searchTerm,
                                               Enterprises = new List<LightEnterprise>(),
                                               Locations = new List<string> { "Tumba", "Tullinge" },
                                               Categories = new List<string>()
                                           };

            var categories = GeneralHelper.GetCategories().Where(c => c.Text.ToLower().StartsWith(searchTerm.ToLower()));
            if (categories.Any())
                searchViewModel.Categories = categories.Select(c => c.Text).ToList();

            foreach (var enterpriseViewModel in enterprises.Select(enterprise => new LightEnterprise
                                                                                     {
                                                                                         Key = EnterpriseHelper.GetKey(enterprise.Id),
                                                                                         Name = enterprise.Name,
                                                                                         LocationInfo = EnterpriseHelper.FormatDisplayStreet(enterprise),
                                                                                         DistanceFromMyLocation = string.Empty,
                                                                                         Categories = EnterpriseHelper.GetDisplayLabelsCategories(enterprise.Categories)
                                                                                     }))
            {
                searchViewModel.Enterprises.Add(enterpriseViewModel);
            }
            return Json(searchViewModel);
        }

        public JsonResult GetEnterprisesCloseToMyLocation(string latitude, string longitude)
        {
            var enterprises = Db.Enterprises.GetNearbyEnterprises(latitude, longitude);

            var searchViewModel = new MainSearchViewModel
            {
                Enterprises = new List<LightEnterprise>()
            };

            foreach (var enterpriseViewModel in enterprises.Select(enterprise => new LightEnterprise
            {
                Key = EnterpriseHelper.GetKey(enterprise.Id),
                Name = enterprise.Name,
                LocationInfo = EnterpriseHelper.FormatDisplayStreet(enterprise),
                DistanceFromMyLocation = string.Empty,
                Categories = EnterpriseHelper.GetDisplayLabelsCategories(enterprise.Categories),
                Coordinates = enterprise.Coordinates,

            }))
            {
                searchViewModel.Enterprises.Add(enterpriseViewModel);
            }

            var myCoord = new GeoCoordinate(double.Parse(latitude, CultureInfo.InvariantCulture), double.Parse(longitude, CultureInfo.InvariantCulture));

            foreach (var enterprise in searchViewModel.Enterprises)
            {
                var enterpriseCoord = new GeoCoordinate(enterprise.Coordinates.Lat, enterprise.Coordinates.Lng);

                var distance = myCoord.GetDistanceTo(enterpriseCoord);

                enterprise.DistanceFromMyLocation = string.Format("{0}km", Math.Round((distance / 1000), 1));
            }
            return Json(searchViewModel);
        }


        //public JsonResult BrowseEnterprises(string stateCode, string city)
        //{
        //    BrowseViewModel viewModel;

        //    if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(stateCode))
        //        viewModel = GetEnterprisesByCity(city, stateCode);
        //    else
        //        viewModel = GetEnterprisesByStateCode(stateCode);

        //    return Json(viewModel);
        //}

        private BrowseViewModel GetEnterprisesByStateCode(string stateCode)
        {
            //TODO
            //var enterprises = Repository.GetEnterprisesByLocation(stateCode, null);

            //var districts = enterprises.Select(enterprise => enterprise.City).Distinct().OrderBy(d => d).ToList();

            //var viewModel = new BrowseViewModel
            //{
            //    Districts = districts,
            //    StateCode = stateCode
            //};

            //return viewModel;
            return null;
        }

        private BrowseViewModel GetEnterprisesByCity(string city, string stateCode)
        {
            return null;
            //TODO
            //var enterprises = Repository.GetEnterprisesByLocation(stateCode, city);

            //var viewModel = new BrowseViewModel
            //{
            //    Enterprises = enterprises
            //};

            //return viewModel;
        }

        public JsonResult GetGeneralLocationInfoByAddress(string address)
        {
            var lowerAddress = address.ToLower();
            var formattedAddress = lowerAddress.Replace(", sverige", string.Empty);
            // Prewview URL http://maps.googleapis.com/maps/api/geocode/json?address=sk%C3%B6nstav%C3%A4gen%203&sensor=false&region=se
            var url = "http://maps.googleapis.com/maps/api/geocode/json?address=" + formattedAddress + "&sensor=false&region=se";

            var wc = new WebClient { Encoding = Encoding.UTF8 };

            var json = wc.DownloadString(url);

            var jo = JObject.Parse(json);

            var results = jo["results"][0];

            var status = jo["status"];

            if (status.ToString() == "OK")
            {
                var viewModel = new LocationViewModel
                                    {
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

                    switch (type)
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
                            lan = addressComponent.SelectToken("long_name").Value<string>().ToLower().Replace("s län", string.Empty).Replace(" county", string.Empty); //TODO!!!!! BETTER!?!?!
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
                viewModel.Coordinates.Lat = double.Parse(lat, CultureInfo.InvariantCulture);
                viewModel.Coordinates.Lng = double.Parse(lng, CultureInfo.InvariantCulture);

                //Give streetnumber a space to the left so it looks good with the address.
                streetNumber = streetNumber == "" ? string.Empty : " " + streetNumber;

                viewModel.Location.complete_address = string.Format("{0}{1}", route, streetNumber);

                //Sätt kommun
                viewModel.Location.county = administrative_area_level_2 != "" ? administrative_area_level_2 : locality;

                //var stateCode = GeneralHelper.GetCountyNameAndCodes().FirstOrDefault(p => p.Text.ToLower().Contains(lan));
                //if (stateCode != null && stateCode.Value.Length < 3)
                //    viewModel.Location.state_code = stateCode.Value;

                //viewModel.Counties = GeneralHelper.GetCountyNameAndCodes();

                return Json(viewModel);
            }
            return null;
        }
    }
}