﻿using EngineeringCalculators.Web.Models;

namespace EngineeringCalculators.Web.Services.Contracts
{
    public interface IMaterialndexedDbService
    {
        Task AddAsync(MaterialModel material);
        Task DeleteAsync(MaterialModel material);
        Task<List<MaterialModel>> GetAllAsync();
        Task<MaterialModel?> GetAsync(int id);
        Task UpdateAsync(MaterialModel material);
    }
}