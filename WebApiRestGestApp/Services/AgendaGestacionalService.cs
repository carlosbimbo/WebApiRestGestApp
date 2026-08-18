using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Repositories;

namespace WebApiRestGestApp.Services
{
    public class AgendaGestacionalService : IAgendaGestacionalService
    {
        private readonly IRepository<AgendaGestacional> _repository;

        public AgendaGestacionalService(IRepository<AgendaGestacional> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AgendaGestacional>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<AgendaGestacional?> GetByIdsAsync(Guid id, int nrosem)
        {
            // Respetamos el orden: primero Id (Guid), luego Nrosem (int)
            return await _repository.GetByIdAsync(id, nrosem);
        }

        public async Task<AgendaGestacional> CreateAsync(AgendaGestacional agenda)
        {
            return await _repository.AddAsync(agenda);
        }

        public async Task<bool> UpdateAsync(Guid id, int nrosem, AgendaGestacional agenda)
        {
            var existing = await _repository.GetByIdAsync(id, nrosem);
            if (existing == null) return false;

            // Protegemos las llaves primarias
            agenda.Id = id;
            agenda.Nrosem = nrosem;

            await _repository.UpdateAsync(agenda);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, int nrosem)
        {
            var existing = await _repository.GetByIdAsync(id, nrosem);
            if (existing == null) return false;

            await _repository.DeleteAsync(id, nrosem);
            return true;
        }
    }
}