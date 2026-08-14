using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Repositories;

namespace WebApiRestGestApp.Services
{
    public class RegistroEventoService : IRegistroEventoService
    {
        private readonly IRepository<RegistroEvento> _repository;

        public RegistroEventoService(IRepository<RegistroEvento> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RegistroEvento>> GetAllEventosAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<RegistroEvento?> GetEventoByIdsAsync(int idEven, Guid idUser)
        {
            // Gracias al params object[], enviamos ambas llaves de forma nativa
            return await _repository.GetByIdAsync(idEven, idUser);
        }

        public async Task<RegistroEvento> CreateEventoAsync(RegistroEvento evento)
        {
            return await _repository.AddAsync(evento);
        }

        public async Task<bool> UpdateEventoAsync(int idEven, Guid idUser, RegistroEvento evento)
        {
            var existing = await _repository.GetByIdAsync(idEven, idUser);
            if (existing == null) return false;

            // Protegemos las llaves primarias
            evento.IdEven = idEven;
            evento.IdUser = idUser;

            await _repository.UpdateAsync(evento);
            return true;
        }

        public async Task<bool> DeleteEventoAsync(int idEven, Guid idUser)
        {
            var existing = await _repository.GetByIdAsync(idEven, idUser);
            if (existing == null) return false;

            await _repository.DeleteAsync(idEven, idUser);
            return true;
        }
    }
}