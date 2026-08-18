using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;

namespace WebApiRestGestApp.Services
{
    public interface IDiasGestacionService
    {
        Task<IEnumerable<DiasGestacion>> GetAllAsync();
        Task<DiasGestacion?> GetByIdsAsync(int idDiasg, Guid idUser);
        Task<DiasGestacion> CreateAsync(DiasGestacion dias);
        Task<bool> UpdateAsync(int idDiasg, Guid idUser, DiasGestacion dias);
        Task<bool> DeleteAsync(int idDiasg, Guid idUser);
    }
}