using EngineeringCalculators.Web.Models.SheetMetal;
using EngineeringCalculators.Web.Enums;
using Microsoft.AspNetCore.Components;

namespace EngineeringCalculators.Web.Components.SheetMetal
{
    public partial class SMBlankAndPierceComponent
    {
        [Parameter]
        public SMBlankAndPierceModel Model { get; set; } = new();

        [Parameter]
        public EventCallback<SMBlankAndPierceModel> OnSave { get; set; }


        private string CalcErrorMessage = "";
        private string CalcTypeErrorMessage = "";
        private string CuttingForceReductionTypeErrorMessage = "";
        private string CuttingForceReductionCalcErrorMessage = "";
        private string StripperTypeErrorMessage = "";

        private string _submitType { get; set; } = "";

        protected override void OnInitialized()
        {
            if (String.IsNullOrWhiteSpace(Model.CalcType) == false)
            {
                SetCalcType(Model.CalcType);
            }
        }


        private async Task HandleValidSubmit()
        {
            if (_submitType.Equals(nameof(Enums.Enums.SubmitType.Calculate)))
            {
                Calculate();
            }
            else if (_submitType.Equals(nameof(Enums.Enums.SubmitType.Save)))
            {
                await HandleSaveAsync();
            }
        }

        private void Calculate()
        {
            if (ValidateCalcType() && ValidateSharpeningProfileType())
            {
                if (Model.CalcType.Equals(nameof(Enums.Enums.PierceAndBlankCalcType.Tensile)) && ValidateTensileData())
                {
                    UseTensileCalC();
                }
                else if (Model.CalcType.Equals(nameof(Enums.Enums.PierceAndBlankCalcType.Shear)) && ValidateShearData())
                {
                    UseShearCalc();
                }
                else
                {
                    return;
                }

                if (Model.SharpeningProfileType.Equals(nameof(Enums.Enums.SharpeningProfileType.Flat)))
                {
                    UseFlatProfileCalc();
                }
                else if (Model.SharpeningProfileType.Equals(nameof(Enums.Enums.SharpeningProfileType.Beveled)) && ValidateBeveledProfileData())
                {
                    UseBeveledProfileCalc();
                }
                else
                {
                    Model.CuttingForce = 0;
                    return;
                }
            }
        }

        public async Task HandleSaveAsync()
        {
            await OnSave.InvokeAsync(Model);
        }

        private void SetForceReductionType(string reductionType)
        {
            Model.SharpeningProfileType = (string)reductionType;

            

            if (Model.SharpeningProfileType.Equals(nameof(Enums.Enums.SharpeningProfileType.Beveled)))
            {
                ValidateBeveledProfileData();
            }
        }

        private bool ValidateBeveledProfileData()
        {
            if (Model.CuttingForceReductionPercent <= 0)
            {
                CuttingForceReductionCalcErrorMessage = "Force reduction ratio must be a positive value";
                return false;
            }
            return true;
        }

        private bool ValidateSharpeningProfileType()
        {
            CuttingForceReductionTypeErrorMessage = "";

            switch (Model.SharpeningProfileType)
            {
                case nameof(Enums.Enums.SharpeningProfileType.Flat):
                    return true;
                case nameof(Enums.Enums.SharpeningProfileType.Beveled):
                    return true;
                default:
                    if (String.IsNullOrWhiteSpace(Model.SharpeningProfileType))
                    {
                        CuttingForceReductionTypeErrorMessage = "Please select one of the options below to perform a calculation";

                    }
                    return false;
            }
        }

        private void SetCalcType(string calcType)
        {
            Model.CalcType = (string)calcType;

            CalcErrorMessage = "";

            //Will use tensile strength
            if (Model.CalcType.Equals(nameof(Enums.Enums.PierceAndBlankCalcType.Tensile)))
            {
                ValidateTensileData();
            }
            //Will use shear strength
            else if (Model.CalcType.Equals(nameof(Enums.Enums.PierceAndBlankCalcType.Shear)))
            {
                ValidateShearData();
            }
        }

        private void SetStripperType(string stripperType)
        {
            Model.StripperType = (string)stripperType;

            StripperTypeErrorMessage = "";
        }

        

        private void UseFlatProfileCalc()
        {
            //No reduction in cutting force
            Model.ReducedCuttingForce = Model.CuttingForce;
        }

        private void UseBeveledProfileCalc()
        {
            double result = Model.CuttingForce;
            result *= Model.CuttingForceReductionPercent;

            Model.ReducedCuttingForce = result;
        }

        private void UseShearCalc()
        {
            double result = Model.ShearStrength;
            result *= Model.MaterialThickness;
            result *= Model.Perimeter;
            result *= Model.NumberOfPunches;

            Model.CuttingForce = result;
        }

        private void UseTensileCalC()
        {
            double result = Model.PercentageOfTensileStrength * Model.TensileStrength;
            result *= Model.MaterialThickness;
            result *= Model.Perimeter;
            result *= Model.NumberOfPunches;

            Model.CuttingForce = result;
        }

        private bool ValidateCalcType()
        {
            CalcTypeErrorMessage = "";

            switch (Model.CalcType)
            {
                case nameof(Enums.Enums.PierceAndBlankCalcType.Tensile):
                    return true;
                case nameof(Enums.Enums.PierceAndBlankCalcType.Shear):
                    return true;
                default:
                    if (String.IsNullOrWhiteSpace(Model.CalcType))
                    {
                        CalcTypeErrorMessage = "Please select one of the options below to perform a calculation";
                       
                    }
                    return false;
            }
        }

        private bool ValidateShearData()
        {
            if (Model.ShearStrength <= 0)
            {
                CalcErrorMessage = "Shear Strength must be a positive value";
                return false;
            }
            return true;
        }

        private bool ValidateTensileData()
        {
            if (Model.TensileStrength <= 0)
            {
                CalcErrorMessage = "Tensile Strength must be a positive value";
                return false;
            }
            return true;
        }

        

        private void SetSubmitType(string type)
        {
            _submitType = type;
        }
    }
}
