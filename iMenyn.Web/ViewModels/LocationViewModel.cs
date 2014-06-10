using System.Collections.Generic;
using iMenyn.Data.Models;

namespace iMenyn.Web.ViewModels
{
    public class LocationViewModel
    {
        public dynamic Location { get; set; }
        public Coordinates Coordinates { get; set; }
        public List<ValueAndText> Counties { get; set; }
    }
}