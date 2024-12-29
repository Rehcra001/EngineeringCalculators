using System.ComponentModel.DataAnnotations;

namespace EngineeringCalculators.Web.Models.SheetMetal
{
    public class SMProjectModel
    {
        [Required]
        public string ProjectName { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Must be greater than zero")]
        public double Thickness { get; set; }
        [Required]
        public  MaterialModel? Material { get; set; }
    }
}
