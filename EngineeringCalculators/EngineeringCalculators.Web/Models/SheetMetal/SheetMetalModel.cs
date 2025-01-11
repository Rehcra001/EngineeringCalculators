namespace EngineeringCalculators.Web.Models.SheetMetal
{
    public class SheetMetalModel
    {
        public SMHeaderModel ProjectDetails { get; set; } = new();
        public List<SMBlankAndPierceModel> BlankAndPierce { get; set; } = [];
    }
}
