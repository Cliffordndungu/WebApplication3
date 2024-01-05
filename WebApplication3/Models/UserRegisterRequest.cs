using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class UserRegisterRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required, Compare("Password")]
        public string confirmPassword { get; set; } = string.Empty;

    }
}
