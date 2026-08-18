using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Repositories;

namespace WebApiRestGestApp.Services
{
    public class RegistroHemoglobinaService : IRegistroHemoglobinaService
    {
        private readonly IRepository<RegistroHemoglobina> _repository;

        public RegistroHemoglobinaService(IRepository<RegistroHemoglobina> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RegistroHemoglobina>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<RegistroHemoglobina?> GetByIdsAsync(int idh, Guid idUser)
        {
            return await _repository.GetByIdAsync(idh, idUser);
        }

        public async Task<RegistroHemoglobina> CreateAsync(RegistroHemoglobina hemoglobina)
        {
            return await _repository.AddAsync(hemoglobina);
        }

        public async Task<bool> UpdateAsync(int idh, Guid idUser, RegistroHemoglobina hemoglobina)
        {
            var existing = await _repository.GetByIdAsync(idh, idUser);
            if (existing == null) return false;

            // Aseguramos que las llaves primarias se mantengan intactas
            hemoglobina.Idh = idh;
            hemoglobina.IdUser = idUser;

            await _repository.UpdateAsync(hemoglobina);
            return true;
        }

        public async Task<bool> DeleteAsync(int idh, Guid idUser)
        {
            var existing = await _repository.GetByIdAsync(idh, idUser);
            if (existing == null) return false;

            await _repository.DeleteAsync(idh, idUser);
            return true;
        }
    }
}