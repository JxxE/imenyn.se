using System.Collections.Generic;
using iMenyn.Data.Models;
using iMenyn.Yelp.Data;

namespace iMenyn.Web.ViewModels
{
    public class SearchYelpViewModel
    {
        public List<Business> Businesses { get; set; }
        public List<ValueAndText> Categories { get; set; }
    }
}