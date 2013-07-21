using System.Collections.Generic;
using iMenyn.Data.Models;

namespace iMenyn.Web.ViewModels
{
    public class BrowseViewModel
    {
        public List<ValueAndText> EnterpriseStateCodes { get; set; }
        public List<string> Districts { get; set; }
        public string StateCode { get; set; }
        public IEnumerable<Enterprise> Enterprises { get; set; }
    }
}