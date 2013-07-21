using iMenyn.Data.Models;

namespace iMenyn.Web.ViewModels
{
    public class StandardViewModel
    {
        public Enterprise Enterprise { get; set; }
        public string Phone { get; set; }
        public Menu Menu { get; set; }
        public ProductListViewModel Products { get; set; }
        public bool RecentlyModified { get; set; }
    }

}