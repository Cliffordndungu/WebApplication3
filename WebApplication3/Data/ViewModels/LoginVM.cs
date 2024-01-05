using System.ComponentModel.DataAnnotations;
using System.Web.Http.ModelBinding;

namespace WebApplication3.Data.ViewModels
{
    public class LoginVM
    {
        [Display (Name = "Email address")]
        [Required(ErrorMessage = "Email address is required")]
        public string emailaddress { get; set; }

        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
