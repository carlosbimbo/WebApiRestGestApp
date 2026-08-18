using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Repositories;

namespace WebApiRestGestApp.Services
{
    public class DiasGestacionService : IDiasGestacionService
    {
        private readonly IRepository<DiasGestacion> _repository;

        public DiasGestacionService(IRepository<DiasGestacion> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DiasGestacion>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<DiasGestacion?> GetByIdsAsync(int idDiasg, Guid idUser)
        {
            return await _repository.GetByIdAsync(idDiasg, idUser);
        }

        public async Task<DiasGestacion> CreateAsync(DiasGestacion dias)
        {
            return await _repository.AddAsync(dias);
        }

        public async Task<bool> UpdateAsync(int idDiasg, Guid idUser, DiasGestacion dias)
        {
            var existing = await _repository.GetByIdAsync(idDiasg, idUser);
            if (existing == null) return false;

            // Protegemos las llaves primarias correctas
            dias.IdDiasg = idDiasg;
            dias.IdUser = idUser;

            await _repository.UpdateAsync(dias);
            return true;
        }

        public async Task<bool> DeleteAsync(int idDiasg, Guid idUser)
        {
            var existing = await _repository.GetByIdAsync(idDiasg, idUser);
            if (existing == null) return false;

            await _repository.DeleteAsync(idDiasg, idUser);
            return true;
        }
    }
}