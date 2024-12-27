namespace EngineeringCalculators.Web.Models.SheetMetal
{
    public class SheetMetalProjectModel
    {
        public string ProjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Thickness { get; set; }
        public  MaterialModel? Material { get; set; }
    }
}
