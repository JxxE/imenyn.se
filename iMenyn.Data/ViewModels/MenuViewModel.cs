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

    public class EnterpriseViewModel : IEnterprise
    {
        public string Id { get; set; }
        public string Name{ get; set; }
        public int PostalCode { get; set; }
        public string PostalTown { get; set; }
        public string SubLocality { get; set; }
        public Coordinates Coordinates { get; set; }
        public List<string> Categories { get; set; }
        public bool IsNew { get; set; }
        public bool LockedFromEdit { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public string ModifiedMenu { get; set; }
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