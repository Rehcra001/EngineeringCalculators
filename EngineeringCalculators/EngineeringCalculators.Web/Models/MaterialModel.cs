using System.ComponentModel.DataAnnotations;

namespace EngineeringCalculators.Web.Models
{
    public class MaterialModel
    {
        //[Range(1, int.MaxValue, ErrorMessage = "Id value of zero is not allowed. This must be set to a positive non zero value in code")]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double ElasticModulus { get; set; } // N/mm^2
        public double PoissonsRatio { get; set; } //Dimensionaless
        public double ShearModulus { get; set; } // N/mm^2
        public double MassDensity { get; set; } // kg/m^3
        public double TensileStrength { get; set; } // N/mm^2
        public double CompressiveStrength { get; set; } // N/mm^2
        public double YieldStrength { get; set; } // N/mm^2
        public double ShearStrength { get; set; } // N/mm^2
        public string Category { get; set; } = string.Empty;

    }
}
