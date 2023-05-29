using System.ComponentModel.DataAnnotations;

namespace FarmCentral.Models
{
    public class SigninViewModel
    {
        //View models
        //https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/mvc-music-store/mvc-music-store-part-3
        //Accessed 26 May 2023
        [Required(ErrorMessage = "Please enter an email.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Please enter a password.")]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;
    }
}
