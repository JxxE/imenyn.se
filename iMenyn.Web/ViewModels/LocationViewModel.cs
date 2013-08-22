using System.Collections.Generic;
using iMenyn.Data.Models;
using iMenyn.Yelp.Data;

namespace iMenyn.Web.ViewModels
{
    public class LocationViewModel
    {
        public Location Location { get; set; }
        public Coordinates Coordinates { get; set; }
        public List<ValueAndText> Counties { get; set; }
    }
}