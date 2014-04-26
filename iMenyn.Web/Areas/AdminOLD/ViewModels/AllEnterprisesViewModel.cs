using System.Collections.Generic;
using iMenyn.Data.Models;

namespace iMenyn.Web.Areas.Admin.ViewModels
{
    public class AllEnterprisesViewModel
    {
        public IEnumerable<Enterprise> Enterprises { get; set; }
    }
}