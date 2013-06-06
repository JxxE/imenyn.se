using System.Collections.Generic;
using Rantup.Data.Models;

namespace Rantup.Web.Areas.Admin.ViewModels
{
    public class AllEnterprisesViewModel
    {
        public IEnumerable<Enterprise> Enterprises { get; set; }
    }
}