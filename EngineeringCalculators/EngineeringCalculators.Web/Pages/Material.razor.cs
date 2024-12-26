using EngineeringCalculators.Web.Models;
using EngineeringCalculators.Web.Services.Contracts;
using Microsoft.AspNetCore.Components;

namespace EngineeringCalculators.Web.Pages
{
    public partial class Material
    {
        private MaterialModel _material = new();
        private List<MaterialModel> _materials = [];
        private List<string> _filterByCategory = [];
        private List<MaterialModel> _filteredMaterial = [];
        private string _searchText = "";
        private string _selectedCategory = "All";
        private bool _canSave = true;
        private bool _disabled = false;

        [Inject]
        public required IMaterialService MaterialService { get; set; }

        private void HandleSelectedMaterial(MaterialModel material)
        {
            _material = material;
            _canSave = false;
            _disabled = true;
        }

        private async Task HandleLoadingMaterialAsync()
        {
            _materials = await MaterialService.GetAllAsync();
            _filteredMaterial = _materials;
            GetCategories();
        }

        private void HandleNewMaterial()
        {
            _material = new();
            _canSave = true;
            _disabled = false;
        }

        private async Task HandleSaveAsync(MaterialModel material)
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
            }



            if (MaterialService.FileHandle is null)
            {
                await MaterialService.SaveAllAsync(_materials);
            }
            else
            {
                await MaterialService.UpdateAsync(_materials);
            }

            _canSave = false;
            _disabled = true;

            HandleFilterByCategory(_selectedCategory);

            GetCategories();
        }

        private void HandleEdit()
        {
            _canSave = true;
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

        private void GetCategories()
        {
            _filterByCategory = _materials.Select(x => x.Category).Distinct().ToList();
        }
    }


}
