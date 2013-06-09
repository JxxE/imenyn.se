using System.Collections.Generic;
using Rantup.Data.Models;
using Rantup.Yelp.Data;

namespace Rantup.Web.ViewModels
{
    public class SearchYelpViewModel
    {
        public List<Business> Businesses { get; set; }
        public List<ValueAndText> Categories { get; set; }
    }
}