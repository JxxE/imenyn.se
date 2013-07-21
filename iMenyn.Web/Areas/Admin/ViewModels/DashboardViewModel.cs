using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iMenyn.Data.Models;


namespace iMenyn.Web.Areas.Admin.ViewModels
{
    public class DashboardViewModel
    {
        public List<Enterprise> NewEnterprises { get; set; }
        public List<Enterprise> EnterprisesWithModifiedMenus { get; set; }
    }
}