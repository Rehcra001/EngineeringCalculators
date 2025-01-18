using System.ComponentModel.DataAnnotations;

namespace EngineeringCalculators.Web.Models.SheetMetal
{
    public class SMBlankAndPierceModel
    {
        public int Id { get; set; }
        //[Required]
        public string ProjectName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(0.001, double.MaxValue, ErrorMessage = "Thickness must be greater than 0")]
        public double MaterialThickness { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Number of punches must be greater than zero")]
        public int NumberOfPunches { get; set; } = 1;

        [Range(0.01, double.MaxValue, ErrorMessage = "Perimeter must be greater than zero")]
        public double Perimeter { get; set; }

        public double TensileStrength { get; set; }

        public double ShearStrength { get; set; }

        public string CalcType { get; set; } = string.Empty;

        [Range(0.001, 1, ErrorMessage = "Must be greater than 0 and less than or equal to 1")]
        public double PercentageOfTensileStrength { get; set; } = 0.7;

        public string StripperType { get; set; } = string.Empty;

        public double StripperPercentOfCuttingForce { get; set; }


        public double CuttingForce { get; set; }

        public double ReducedCuttingForce { get; set; }

        [Range(0.001, 1, ErrorMessage = "Must be greater than 0 and less than or equal to 1")]
        public double CuttingForceReductionPercent { get; set; } = 0.6;

        public string SharpeningProfileType { get; set; } = string.Empty;


        public double StrippingForce { get; set; }
        public double StrippingConstant { get; set; } = 0.10;

        public double Clearance { get; set; }
        [Range(0.005, 0.035, ErrorMessage = "Must be greater than or equal to 0.005 and less than or equal to 0.035")]
        public double ClearanceConstant { get; set; } = 0.01;
    }
}
