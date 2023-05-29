using FarmCentral.Models.ModelsDB;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FarmCentral.Models
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "Please enter the product type.")]
        [Display(Name = "Type")]
        public string ProductType { get; set; } = null!;

        [Required(ErrorMessage = "Please enter the product description.")]
        [Display(Name = "Description")]
        public string ProductDescription { get; set; } = null!;

        [Required(ErrorMessage = "Please enter the product price.")]
        [Display(Name = "Price")]
        public string ProductPrice { get; set; } = null!;

        [Required(ErrorMessage = "Please enter the product listing date.")]
        [DataType(DataType.Date)]
        [Display(Name = "ListDate")]
        public DateTime ProductListDate { get; set; }
    }
}
