using System.Collections.Generic;
using Rantup.Data.Models;
using Rantup.Web.ViewModels;

namespace Rantup.Web.Areas.Admin.ViewModels
{
    public class CompareModifiedMenuViewModel
    {
        public Enterprise Enterprise { get; set; }
        public string ModifiedMenuId { get; set; }
        public ProductListViewModel ModifiedMenuProducts { get; set; }
        public ProductListViewModel LiveMenuProducts { get; set; }
    }
}