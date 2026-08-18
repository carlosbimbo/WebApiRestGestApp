using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;

namespace WebApiRestGestApp.Services
{
    public interface IAgendaGestacionalService
    {
        Task<IEnumerable<AgendaGestacional>> GetAllAsync();
        Task<AgendaGestacional?> GetByIdsAsync(Guid id, int nrosem);
        Task<AgendaGestacional> CreateAsync(AgendaGestacional agenda);
        Task<bool> UpdateAsync(Guid id, int nrosem, AgendaGestacional agenda);
        Task<bool> DeleteAsync(Guid id, int nrosem);
    }
}