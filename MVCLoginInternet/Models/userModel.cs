using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace MVCLoginInternet.Models
{
    public class userModel
    {
        [Required]
        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Email Address: ")]
        public String email { get; set; }

        [Required]
        [Display(Name = "User Name(Twitter Name): ")]
        public String userName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6)]
        [Display(Name = "Password: ")]
        public String password { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name: ")]
        public String firstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name: ")]
        public String lastName { get; set; }
    }

    public class Login
    {
        [Required]
        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Email Address: ")]
        public String email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6)]
        [Display(Name = "Password: ")]
        public String password { get; set; }

  

    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }


    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Display(Name = "Last Name: ")]
        public string lastName { get; set; }

        [Display(Name = "First Name: ")]
        public string firstName { get; set; }

        [Display(Name = "Twitter Name: ")]
        public string twitterName { get; set; }

        public string ExternalLoginData { get; set; }
    }
}