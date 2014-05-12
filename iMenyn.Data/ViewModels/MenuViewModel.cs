using System.Collections.Generic;
using iMenyn.Data.Models;

namespace iMenyn.Data.ViewModels
{
    public class CompleteEnterpriseViewModel
    {
        //public bool RecentlyModified { get; set; }
        public EnterpriseViewModel Enterprise { get; set; }
        public List<ViewModelCategory> ViewModelCategories { get; set; }
    }

    public class EnterpriseViewModel : Enterprise
    {
        public string Address { get; set; }
        public double DistanceFromMyLocation { get; set; }

        public List<ValueAndText> DisplayCategories { get; set; }

        //Used when editing
        //public string EditKey { get; set; }

        //Spam-check property
        public string Nope { get; set; }
    }

    public class ViewModelCategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ProductViewModel> Products { get; set; }

        public string EnterpriseId { get; set; }
    }
    public class ProductViewModel : Product
    {
        public string EnterpriseId { get; set; }
        public string CategoryId { get; set; }
    }
}