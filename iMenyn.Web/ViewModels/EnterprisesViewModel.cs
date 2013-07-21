using System.Collections.Generic;

namespace iMenyn.Web.ViewModels
{
    public class EnterprisesViewModel
    {
        public List<EnterpriseViewModel> Enterprises { get; set; }
    }

    public class EnterpriseViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int PostalCode { get; set; }
        public string City { get; set; }
        public List<string> Categories { get; set; }
    }

}