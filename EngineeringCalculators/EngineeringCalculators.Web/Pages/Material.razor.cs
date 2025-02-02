﻿using EngineeringCalculators.Web.Constants;
using EngineeringCalculators.Web.Data;
using EngineeringCalculators.Web.Models;
using EngineeringCalculators.Web.Services.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EngineeringCalculators.Web.Pages
{
    public partial class Material: IDisposable
    {
        private MaterialModel _material = new();
        private List<MaterialModel> _materials = [];
        private List<string> _filterByCategory = [];
        private List<MaterialModel> _filteredMaterial = [];
        private string _searchText = "";
        private string _selectedCategory = "All";
        private bool _canSave = true;
        private bool _disabled = false;
        private bool _canSaveEdit = false;
        private string _newMaterialErrorMessage = "";

        private const string MATERIAL_STORE = IndexedDbStoreNameConstants.MaterialsStore;

        [Inject]
        public required EngineeringCalculatorsDb EngCalcDb { get; set; }

        [Inject]
        public required IIndexedDbService IndexedDbService { get; set; }

        [Inject]
        public required IJSRuntime JSRuntime { get; set; }

        [Inject]
        public required IEventService EventService { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            await EngCalcDb.OpenAsync();
            await HandleLoadingMaterialAsync();
            EventService.IndexedDbRestored += OnDatabaseRestored;
        }

        async void OnDatabaseRestored()
        {
            await InvokeAsync(HandleLoadingMaterialAsync);
            await InvokeAsync(StateHasChanged);
        }

        private void HandleSelectedMaterial(MaterialModel material)
        {
            _material = material;
            _canSave = false;
            _disabled = true;
            _newMaterialErrorMessage = "";
        }

        private async Task HandleLoadingMaterialAsync()
        {
            _materials = await IndexedDbService.GetAllAsync<MaterialModel>(MATERIAL_STORE);
            _filteredMaterial = _materials;
            SortMaterialByName();
            GetCategories();
        }

        private void HandleNewMaterial()
        {
            _material = new();
            _canSave = true;
            _disabled = false;
            _newMaterialErrorMessage = "";
        }

        private async Task HandleDeleteAsync()
        {
            string message = "Are you sure you want to delete this material?";
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", message);

            if (confirmed)
            {
                _materials.Remove(_material);
                await IndexedDbService.DeleteAsync(_material.Id, MATERIAL_STORE);
                _material = new();
            }
        }

        private async Task HandleSaveAsync(MaterialModel material)
        {
            if (IsUniqueMaterial(material) || _canSaveEdit)
            {
                if (String.IsNullOrWhiteSpace(material.Category))
                {
                    material.Category = "--";
                }

                if (material.Id == 0) // New Material
                {
                    if (_materials.Count == 0)
                    {
                        material.Id = 1;
                    }
                    else
                    {
                        int max = _materials.Max(x => x.Id);
                        material.Id = max + 1;
                    }
                    _materials.Add(material);
                    await IndexedDbService.AddAsync<MaterialModel>(material, MATERIAL_STORE);
                }
                else
                {
                    // Existing material - update
                    await IndexedDbService.UpdateAsync<MaterialModel>(material, MATERIAL_STORE);
                }

                _canSave = false;
                _canSaveEdit = false;
                _disabled = true;

                HandleFilterByCategory(_selectedCategory);

                GetCategories();
            }
            else
            {
                _newMaterialErrorMessage = "A material with this name already exists";
            }
            
        }

        private void HandleEdit()
        {
            _canSave = true;
            _canSaveEdit = true;
            _disabled = false;
        }

        private void HandleFilterByCategory(string category)
        {
            _selectedCategory = category;


            if (_selectedCategory.Equals("All"))
            {
                _filteredMaterial = _materials;
            }
            else
            {
                _filteredMaterial = _materials.Where(x => x.Category.Equals(_selectedCategory)).ToList();
            }

            if (String.IsNullOrWhiteSpace(_searchText) == false)
            {
                _filteredMaterial = _filteredMaterial.Where(x => x.Name.Contains(_searchText)).ToList();
            }
        }

        private void HandleSearchText()
        {
            HandleFilterByCategory(_selectedCategory);
        }

        private void HandleClearSearch()
        {
            _searchText = "";
            HandleFilterByCategory(_selectedCategory);
        }

        private bool IsUniqueMaterial(MaterialModel material)
        {
            _newMaterialErrorMessage = "";

            string newName = material.Name;

            bool isUnique = _materials.Where(x => x.Name.ToLower().Equals(newName.ToLower())).Any() == false;

            return isUnique;
        }

        private void GetCategories()
        {
            _filterByCategory = _materials.Select(x => x.Category).Distinct().ToList();
        }

        private void SortMaterialByName()
        {
            _filteredMaterial.Sort((x, y) => x.Name.CompareTo(y.Name));
        }

        private void SortMaterialByCategoryThenName()
        {
            _filteredMaterial = _filteredMaterial.OrderBy(c => c.Category).ThenBy(n => n.Name).ToList();
        }

        public void Dispose()
        {
            EventService.IndexedDbRestored -= OnDatabaseRestored;
        }
    }


}
