using System.Collections.Generic;
using iMenyn.Data.Models;

namespace iMenyn.Web.ViewModels
{
    public class EnterprisesViewModel
    {
        public List<EnterpriseViewModel> Enterprises { get; set; }
    }

    public class EnterpriseViewModel : Enterprise
    {
        public List<ValueAndText> ChosenCategories { get; set; }
        public double DistanceFromMyLocation { get; set; }

        //Used when editing
        public string EditKey { get; set; }

        //Spam-check property
        public string Nope { get; set; }
    }

}