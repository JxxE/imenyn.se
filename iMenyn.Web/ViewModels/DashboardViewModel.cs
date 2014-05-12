using System.Collections.Generic;
using iMenyn.Data.Models;

namespace iMenyn.Web.ViewModels
{
    public class DashboardViewModel
    {
        public List<Enterprise> NewEnterprises { get; set; }

        public List<Enterprise> ModifiedEnterprises { get; set; }

        public AccountViewModel Account { get; set; }

        public IEnumerable<Enterprise> AllEnterprises { get; set; }

        public int ProductCount { get; set; }

        //Accounts, not admin.
       // public StandardViewModel StandardViewModel { get; set; }
    }
}