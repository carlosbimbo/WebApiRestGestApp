using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Repositories;

namespace WebApiRestGestApp.Services
{
    public class EtapaGestacionalService : IEtapaGestacionalService
    {
        private readonly IRepository<EtapaGestacional> _repository;

        public EtapaGestacionalService(IRepository<EtapaGestacional> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<EtapaGestacional>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<EtapaGestacional?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<EtapaGestacional> CreateAsync(EtapaGestacional etapa)
        {
            return await _repository.AddAsync(etapa);
        }

        public async Task<bool> UpdateAsync(Guid id, EtapaGestacional etapa)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            // Aseguramos que el Id no sea sobrescrito
            etapa.Id = existing.Id;

            await _repository.UpdateAsync(etapa);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}