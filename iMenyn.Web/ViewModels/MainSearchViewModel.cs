using System.Collections.Generic;
using iMenyn.Data.Models;

namespace iMenyn.Web.ViewModels
{
    public class MainSearchViewModel
    {
        public string SearchQuery { get; set; }

        public List<LightEnterprise> Enterprises { get; set; }
        public List<string> Locations { get; set; }
        public List<string> Categories { get; set; }
    }

    public class LightEnterprise
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string LocationInfo { get; set; }
        public string DistanceFromMyLocation { get; set; }
        public List<string> Categories { get; set; }
        public Coordinates Coordinates { get;set;}
    }
}