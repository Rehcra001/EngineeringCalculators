using System.ComponentModel.DataAnnotations;

namespace EngineeringCalculators.Web.Models.SheetMetal
{
    public class SMBlankAndPierceModel
    {
        public int Id { get; set; }
        [Required]
        public string ProjectName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Number of punches must be greater than zero")]
        public int NumberOfPunches { get; set; } = 1;

        [Range(0.01, double.MaxValue, ErrorMessage = "Perimeter must be greater than zero")]
        public double Perimeter { get; set; }

        public double TensileStrength { get; set; }

        public double ShearStrength { get; set; }

        public string CalcType { get; set; } = nameof(Enums.Enums.PierceAndBlankCalcType.Tensile);

        [Range(0.001, 1, ErrorMessage = "Must be greater than 0 and less than or equal to 1")]
        public double PercentageOfTensileStrength { get; set; } = 0.7;

        public double ForceRequired { get; set; }

        public double StrippingForce { get; set; }

        public double Clearance { get; set; }
    }
}
