namespace EngineeringCalculators.Web.Models.SheetMetal
{
    public class SheetMetalProjectModel
    {
        public SMHeaderModel ProjectDetails { get; set; } = new();
        public List<SMBlankAndPierceModel> BlankAndPierce { get; set; } = [];
    }
}
