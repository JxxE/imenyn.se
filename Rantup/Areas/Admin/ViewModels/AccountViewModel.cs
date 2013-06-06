namespace Rantup.Web.Areas.Admin.ViewModels
{
    public class AccountViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsAdmin { get; set; }
        public bool Enabled { get; set; }
    }
}