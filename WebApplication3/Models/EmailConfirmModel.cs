using System.Diagnostics.Eventing.Reader;

namespace WebApplication3.Models
{
    public class EmailConfirmModel
    {

        public string Email { get; set; }
        public bool isConfirmed { get; set; }

        public bool EmailSent { get; set; }

        public bool EmailVerified { get; set; }
    }
}