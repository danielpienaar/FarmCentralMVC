using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FarmCentral.Models.ModelsDB
{
    public class Product
    {
        //Primary key identity field
        //https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=data-annotations
        //Accessed 29 May 2023
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public Guid ProductId { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string ProductType { get; set; } = null!;

        [Required]
        [Display(Name = "Description")]
        public string ProductDescription { get; set; } = null!;

        [Required]
        [Display(Name = "Price")]
        public decimal ProductPrice { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "List Date")]
        public DateTime ProductListDate { get; set; }

        [Required]
        [Display(Name = "Farmer")]
        public Farmer Farmer { get; set; } = null!;

        //Properties for sorting
        [NotMapped]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [NotMapped]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
    }
}
