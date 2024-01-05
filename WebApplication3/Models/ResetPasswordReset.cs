using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class ResetPasswordReset
    {

        [Required]
        public string token { get; set; } = string.Empty;
        [Required, MinLength(7)]
        public string Password { get; set; } = string.Empty;

        [Required, Compare("Password")]
        public string confirmPassword { get; set; } = string.Empty;
    }
}
