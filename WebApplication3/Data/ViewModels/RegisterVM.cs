using System.ComponentModel.DataAnnotations;
using WebApplication3.Data.Enums;

namespace WebApplication3.Data.ViewModels
{
    public class RegisterVM
    {
        [Display(Name = "Full name ")]
        [Required(ErrorMessage = "Full name  is required")]
        public string fullname { get; set; }

        [Display(Name = "Company name")]
        public string? companyname { get; set; }

        [Display(Name = "Email address")]
        [Required,EmailAddress (ErrorMessage = "Email address is required")]
        public string emailaddress { get; set; }

        public string phoneNumber { get; set; }

        [Display(Name = "Data-Center Location")]
        public DataCenterLocation SelectedLocation { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name ="Confirm Password")]
        [Required(ErrorMessage ="Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password" , ErrorMessage= "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
