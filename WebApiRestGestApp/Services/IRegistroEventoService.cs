using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;

namespace WebApiRestGestApp.Services
{
    public interface IRegistroEventoService
    {
        Task<IEnumerable<RegistroEvento>> GetAllEventosAsync();
        Task<RegistroEvento?> GetEventoByIdsAsync(int idEven, Guid idUser);
        Task<RegistroEvento> CreateEventoAsync(RegistroEvento evento);
        Task<bool> UpdateEventoAsync(int idEven, Guid idUser, RegistroEvento evento);
        Task<bool> DeleteEventoAsync(int idEven, Guid idUser);
        
        Task<IEnumerable<RegistroEvento>> SaveOrUpdEventUserSyncAsync(IEnumerable<RegistroEvento> eventos);
    }
}