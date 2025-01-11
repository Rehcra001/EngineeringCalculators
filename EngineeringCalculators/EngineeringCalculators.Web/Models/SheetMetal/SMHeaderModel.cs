using System.ComponentModel.DataAnnotations;
using EngineeringCalculators.Web.CustomValidations;

namespace EngineeringCalculators.Web.Models.SheetMetal
{
    public class SMHeaderModel
    {
        [Required]
        public string ProjectName { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;

        [Required]
        public MaterialModel Material { get; set; } = new();

        [Required]
        [GreaterThanZeroDouble]
        public double MaterialThickness { get; set; }
        
    }
}
