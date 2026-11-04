using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;

namespace WebApiRestGestApp.Services
{
    public interface IRegistroHemoglobinaService
    {
        Task<IEnumerable<RegistroHemoglobina>> GetAllAsync();
        Task<RegistroHemoglobina?> GetByIdsAsync(int idh, Guid idUser);
        Task<RegistroHemoglobina> CreateAsync(RegistroHemoglobina hemoglobina);
        Task<bool> UpdateAsync(int idh, Guid idUser, RegistroHemoglobina hemoglobina);
        Task<bool> DeleteAsync(int idh, Guid idUser);

        // NUEVO: Método migrado de Node.js para procesar array (Sincronización)
        Task<IEnumerable<RegistroHemoglobina>> SaveOrUpdRegisHemoSyncAsync(IEnumerable<RegistroHemoglobina> hemoglobinas);
    }
}