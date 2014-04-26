using System.Collections.Generic;
using iMenyn.Data.Models;
using iMenyn.Web.ViewModels;

namespace iMenyn.Web.Areas.Admin.ViewModels
{
    public class CompareModifiedMenuViewModel
    {
        public Enterprise Enterprise { get; set; }
        public string ModifiedMenuId { get; set; }
        //public ProductListViewModel ModifiedMenuProducts { get; set; }
        //public ProductListViewModel LiveMenuProducts { get; set; }
    }
}