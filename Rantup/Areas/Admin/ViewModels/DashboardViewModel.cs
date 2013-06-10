using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rantup.Data.Models;


namespace Rantup.Web.Areas.Admin.ViewModels
{
    public class DashboardViewModel
    {
        public List<Enterprise> NewEnterprises { get; set; }
        public List<Enterprise> EnterprisesWithModifiedMenus { get; set; }
    }
}