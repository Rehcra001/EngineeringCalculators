﻿using EngineeringCalculators.Web.Models.SheetMetal;
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
        private string StripperTypeErrorMessage = "";

        private string _submitType { get; set; } = "";

        protected override void OnInitialized()
        {
            if (String.IsNullOrWhiteSpace(Model.CalcType) == false)
            {
                SetCalcType(Model.CalcType);
            }
        }
        public async Task HandleSaveAsync()
        {
            await OnSave.InvokeAsync(Model);
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

        private void Calculate()
        {
            if (ValidateCalcType())
            {
                if (Model.CalcType.Equals(nameof(Enums.Enums.PierceAndBlankCalcType.Tensile)))
                {
                    UseTensileCalC();
                }
                else if (Model.CalcType.Equals(nameof(Enums.Enums.PierceAndBlankCalcType.Shear)))
                {
                    UseShearCalc();
                }
            }
        }

        private void UseShearCalc()
        {
            throw new NotImplementedException();
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

        private void SetSubmitType(string type)
        {
            _submitType = type;
        }
    }
}
