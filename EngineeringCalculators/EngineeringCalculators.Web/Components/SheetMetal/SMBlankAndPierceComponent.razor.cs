using EngineeringCalculators.Web.Models.SheetMetal;
using EngineeringCalculators.Web.Enums;
using Microsoft.AspNetCore.Components;
using Humanizer;

namespace EngineeringCalculators.Web.Components.SheetMetal
{
    public partial class SMBlankAndPierceComponent
    {
        [Parameter]
        public SMBlankAndPierceModel Model { get; set; } = new();

        [Parameter]
        public EventCallback<SMBlankAndPierceModel> OnSave { get; set; }


        private string CalcErrorMessage = "";
        private string CuttingForceReductionCalcErrorMessage = "";

        private string _submitType { get; set; } = "";

        protected override void OnInitialized()
        {
            if (String.IsNullOrWhiteSpace(Model.CalcType) == false)
            {
                SetCalcType(Model.CalcType);
            }
        }

        private void SetSubmitType(string type)
        {
            _submitType = type;
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
            if (Model.CalcType.Equals(nameof(Enums.Enums.PierceAndBlankCalcType.Tensile)))
            {
                if (ValidateTensileData())
                {
                    UseTensileCalC();
                }
                else
                {
                    return;
                }
            }
            else if (Model.CalcType.Equals(nameof(Enums.Enums.PierceAndBlankCalcType.Shear)))
            {
                if (ValidateShearData())
                {
                    UseShearCalc();
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
            Model.Clearance = Math.Round(CalculateClearance(), 3);

            if (Model.SharpeningProfileType.Equals(nameof(Enums.Enums.SharpeningProfileType.Flat)))
            {
                UseFlatProfileCalc();
            }
            else if (Model.SharpeningProfileType.Equals(nameof(Enums.Enums.SharpeningProfileType.Beveled)))
            {
                if (ValidateBeveledProfileData())
                {
                    UseBeveledProfileCalc();
                }
                else
                {
                    return;
                }
            }
            else
            {
                Model.CuttingForce = 0;
                return;
            }

            StripperCalc();
            PressTonnageRequirement();
        }

        private void PressTonnageRequirement()
        {
            //N to KN
            double kn = 1000;
            //KN to ton
            double ton = 9.81;

            double result = 0;

            switch (Model.StripperType)
            {
                case nameof(Enums.Enums.StripperType.Solid):
                    result = Model.ReducedCuttingForce / kn / ton;
                    Model.PressTonnageRequirements = Math.Round(result * Model.SafetyFactor, 3);
                    break;
                case nameof(Enums.Enums.StripperType.Spring):
                    result = (Model.ReducedCuttingForce + Model.StrippingForce) / kn / ton;
                    Model.PressTonnageRequirements = Math.Round(result * Model.SafetyFactor, 3);
                    break;
            }
        }

        private void UseTensileCalC()
        {
            double result = Model.PercentageOfTensileStrength * Model.TensileStrength;
            result *= Model.MaterialThickness;
            result *= Model.Perimeter;
            result *= Model.NumberOfPunches;

            Model.CuttingForce = Math.Round(result, 3);
        }

        private void UseShearCalc()
        {
            double result = Model.ShearStrength;
            result *= Model.MaterialThickness;
            result *= Model.Perimeter;
            result *= Model.NumberOfPunches;

            Model.CuttingForce = Math.Round(result, 3);
        }

        private double CalculateClearance()
        {
            switch (Model.CalcType)
            {
                case nameof(Enums.Enums.PierceAndBlankCalcType.Tensile):
                    return UseTensileClearanceCalc();
                case nameof(Enums.Enums.PierceAndBlankCalcType.Shear):
                    return UseShearClearanceCalc();
                default:
                    return 0;
            }
        }

        private double UseTensileClearanceCalc()
        {
            double result;
            result = Model.ClearanceConstant * Model.MaterialThickness;
            result *= Math.Sqrt(Model.PercentageOfTensileStrength * Model.TensileStrength);
            //6.32 is constant for this type of calculation and does not change
            result /= 6.32;
            //return the total clearance
            result *= 2;
            return result;
        }

        private double UseShearClearanceCalc()
        {
            double result;
            result = Model.ClearanceConstant * Model.MaterialThickness;
            result *= Math.Sqrt(Model.ShearStrength);
            //6.32 is constant for this type of calculation and does not change
            result /= 6.32;
            //return the total clearance
            result *= 2;
            return result;
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

            Model.ReducedCuttingForce = Math.Round(result, 3);
        }

        private void SetCalcType(string calcType)
        {
            Model.CalcType = (string)calcType;
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

        private bool ValidateTensileData()
        {
            CalcErrorMessage = "";

            if (Model.TensileStrength <= 0)
            {
                CalcErrorMessage = "Tensile Strength must be a positive value";
                return false;
            }
            return true;
        }

        private bool ValidateShearData()
        {
            CalcErrorMessage = "";

            if (Model.ShearStrength <= 0)
            {
                CalcErrorMessage = "Shear Strength must be a positive value";
                return false;
            }
            return true;
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
            CuttingForceReductionCalcErrorMessage = "";

            if (Model.CuttingForceReductionPercent <= 0)
            {
                CuttingForceReductionCalcErrorMessage = "Force reduction constant must be a positive value";
                return false;
            }
            return true;
        }

        private void SetStripperType(string stripperType)
        {
            Model.StripperType = (string)stripperType;
        }

        private void StripperCalc()
        {
            Model.StrippingForce = Math.Round(Model.CuttingForce * Model.StrippingConstant, 3);
        }

        public async Task HandleSaveAsync()
        {
            await OnSave.InvokeAsync(Model);
        }


    }
}
