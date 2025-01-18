using EngineeringCalculators.Web.Models.SheetMetal;
using Microsoft.AspNetCore.Components;

namespace EngineeringCalculators.Web.Components.SheetMetal
{
    public partial class SMProjectHeaderComponent
    {
        [Parameter]
        public SMHeaderModel Model { get; set; } = new();

        private void HandleSubmit()
        {
            
        }
    }
}
