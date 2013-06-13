using System.Collections.Generic;
using Rantup.Data.Models;

namespace Rantup.Web.ViewModels
{
    public class BrowseViewModel
    {
        public List<ValueAndText> EnterpriseStateCodes { get; set; }
        public List<string> Districts { get; set; }
        public string StateCode { get; set; }
    }
}