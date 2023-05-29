using System.ComponentModel.DataAnnotations;

namespace FarmCentral.Models
{
    //View models
    //https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/mvc-music-store/mvc-music-store-part-3
    //Accessed 29 May 2023
    public class EmployeeViewModel
    {
        [Required(ErrorMessage = "Please enter the employee's email.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        public string EmployeeEmail { get; set; } = null!;

        [Required(ErrorMessage = "Please enter the employee's name.")]
        [Display(Name = "Name")]
        public string EmployeeName { get; set; } = null!;

        [Display(Name = "Password")]
        public string? EmployeePassword { get; set; }

        [Display(Name = "Confirm Password")]
        public string? EmployeePasswordConfirmation { get; set; }
    }
}
