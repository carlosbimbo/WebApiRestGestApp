using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Repositories;

namespace WebApiRestGestApp.Services
{
    public class RegistroSuplementoService : IRegistroSuplementoService
    {
        private readonly IRepository<RegistroSuplemento> _repository;

        public RegistroSuplementoService(IRepository<RegistroSuplemento> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RegistroSuplemento>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<RegistroSuplemento?> GetByIdsAsync(int idSuple, Guid idUser)
        {
            // Enviamos las dos llaves al Repositorio Genérico
            return await _repository.GetByIdAsync(idSuple, idUser);
        }

        public async Task<RegistroSuplemento> CreateAsync(RegistroSuplemento suplemento)
        {
            suplemento.CreatedAt = DateTime.UtcNow;
            suplemento.UpdatedAt = DateTime.UtcNow;
            return await _repository.AddAsync(suplemento);
        }

        public async Task<bool> UpdateAsync(int idSuple, Guid idUser, RegistroSuplemento suplemento)
        {
            var existing = await _repository.GetByIdAsync(idSuple, idUser);
            if (existing == null) return false;

            // Protegemos las llaves primarias y fechas de creación
            suplemento.IdSuple = idSuple;
            suplemento.IdUser = idUser;
            suplemento.CreatedAt = existing.CreatedAt;
            suplemento.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(suplemento);
            return true;
        }

        public async Task<bool> DeleteAsync(int idSuple, Guid idUser)
        {
            var existing = await _repository.GetByIdAsync(idSuple, idUser);
            if (existing == null) return false;

            await _repository.DeleteAsync(idSuple, idUser);
            return true;
        }
    }
}