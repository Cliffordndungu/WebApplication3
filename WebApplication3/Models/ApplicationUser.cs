using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Display(Name = "Company name")]
        public string? Companyname {get; set;}

        public string? Tenantid { get; set;}

        public string? stpcustomerid { get; set;}
        public DateTimeOffset? TrialEndDate { get; set;}

        public string datacenterlocation { get; set;}




    }
}
