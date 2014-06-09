using System.Collections.Generic;
using iMenyn.Data.Models;

namespace iMenyn.Data.ViewModels
{
    public class CompleteEnterpriseViewModel
    {
        //public bool RecentlyModified { get; set; }
        public EnterpriseViewModel Enterprise { get; set; }
        public List<ViewModelCategory> ViewModelCategories { get; set; }

        //Only visible for Admin when approving menu
        public List<ProductViewModel> DeletedProducts { get; set; }
    }

    public class EnterprisesViewModel
    {
        public List<EnterpriseViewModel> Enterprises { get; set; }
    }

    public class EnterpriseViewModel : Enterprise
    {
        public string DisplayStreet { get; set; }
        public string Key { get; set; }

        public List<ValueAndText> DisplayCategories { get; set; }

        public bool ShowForm { get; set; }

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
        public string CategoryId { get; set; }

        public bool Updated { get; set; }
        public bool New { get; set; }
        public bool Deleted { get; set; }
        public Product OriginalProduct { get; set; }
    }
}