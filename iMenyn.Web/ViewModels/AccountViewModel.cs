using System.Collections.Generic;

namespace iMenyn.Web.ViewModels
{
    public class AccountViewModel
    {
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public List<string> Enterprises { get; set; }
    }
}