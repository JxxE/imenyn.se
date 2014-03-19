using System.Collections.Generic;
using iMenyn.Data.Models;
using iMenyn.Web.ViewModels;

namespace iMenyn.Web.Areas.Admin.ViewModels
{
    public class DashboardViewModel
    {
        public List<Enterprise> NewEnterprises { get; set; }
        public List<Enterprise> EnterprisesWithModifiedMenus { get; set; }

        //Accounts, not admin.
       // public StandardViewModel StandardViewModel { get; set; }
    }
}