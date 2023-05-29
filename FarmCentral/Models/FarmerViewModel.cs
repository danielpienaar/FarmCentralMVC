using System.ComponentModel.DataAnnotations;

namespace FarmCentral.Models
{
    public class FarmerViewModel
    {
        [Required(ErrorMessage = "Please enter the farmer's email.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        public string FarmerEmail { get; set; } = null!;

        [Required(ErrorMessage = "Please enter the farmer's name.")]
        [Display(Name = "Name")]
        public string FarmerName { get; set; } = null!;

        //[Required(ErrorMessage = "Please enter the farmer's password.")]
        [Display(Name = "Password")]
        public string? FarmerPassword { get; set; }

        //[Required(ErrorMessage = "Please confirm the farmer's password.")]
        [Display(Name = "Confirm Password")]
        public string? FarmerPasswordConfirmation { get; set; }
    }
}
