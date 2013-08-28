using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace iMenyn.Web.Areas.Admin.ViewModels
{
    public class UserInputViewModel
    {
        [HiddenInput]
        public string Id { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Ange namn")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Ange en epost!")]
        [RegularExpression(@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$", ErrorMessage = "Ej en riktigt epostadress")]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Postal code")]
        public string PostalCode { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(35, MinimumLength = 6, ErrorMessage = "Lösenordet måste vara 6-35 karaktärer långt")]
        [Required(ErrorMessage = "Du måste ange ett lösenord!")]
        public string Password { get; set; }

        [Display(Name = "ConfirmPassword")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Error")]
        public string ConfirmPassword { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsNewUser()
        {
            return Id == "0";
        }
    }
}