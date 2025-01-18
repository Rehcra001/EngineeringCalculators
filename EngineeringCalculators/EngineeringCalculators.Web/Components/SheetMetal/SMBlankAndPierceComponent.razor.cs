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
            if (ValidateCalcType())
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
            }
            else
            {
                return;
            }

            if (ValidateSharpeningProfileType())
            {

                if (Model.SharpeningProfileType.Equals(nameof(Enums.Enums.SharpeningProfileType.Flat)))
                {
                    UseFlatProfileCalc();
                }
                else if (Model.SharpeningProfileType.Equals(nameof(Enums.Enums.SharpeningProfileType.Beveled)))
                {
                    UseBeveledProfileCalc();
                }
                else
                {
                    Model.CuttingForce = 0;
                    return;
                }
            }
            else
            {
                Model.CuttingForce = 0;
                return;
            }
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
            CalcTypeErrorMessage = "";
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

            CuttingForceReductionTypeErrorMessage = "";

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

            StripperTypeErrorMessage = "";
        }

        public async Task HandleSaveAsync()
        {
            await OnSave.InvokeAsync(Model);
        }


    }
}
