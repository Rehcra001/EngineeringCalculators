namespace EngineeringCalculators.Web.Models.SheetMetal
{
    public class BlankAndPierceModel
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Perimeter { get; set; }
        public double ForceRequired { get; set; }
        public double StrippingForce { get; set; }
        public double Clearance { get; set; }
    }
}
