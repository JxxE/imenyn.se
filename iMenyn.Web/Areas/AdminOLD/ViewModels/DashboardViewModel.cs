﻿using System.Collections.Generic;
using iMenyn.Data.Models;

namespace iMenyn.Web.ViewModels
{
    public class DashboardViewModel
    {
        public List<Enterprise> NewEnterprises { get; set; }

        public List<string> ModifiedMenus { get; set; }

        //Accounts, not admin.
       // public StandardViewModel StandardViewModel { get; set; }
    }
}