using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;

namespace WebApiRestGestApp.Services
{
    public interface IRegistroSuplementoService
    {
        Task<IEnumerable<RegistroSuplemento>> GetAllAsync();
        Task<RegistroSuplemento?> GetByIdsAsync(int idSuple, Guid idUser);
        Task<RegistroSuplemento> CreateAsync(RegistroSuplemento suplemento);
        Task<bool> UpdateAsync(int idSuple, Guid idUser, RegistroSuplemento suplemento);
        Task<bool> DeleteAsync(int idSuple, Guid idUser);

        // NUEVO: Método migrado de Node.js
        Task<IEnumerable<RegistroSuplemento>> SaveOrUpdSupleSyncAsync(IEnumerable<RegistroSuplemento> suplementos);
    }
}