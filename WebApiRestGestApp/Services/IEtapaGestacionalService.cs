using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;

namespace WebApiRestGestApp.Services
{
    public interface IEtapaGestacionalService
    {
        Task<IEnumerable<EtapaGestacional>> GetAllAsync();
        Task<EtapaGestacional?> GetByIdAsync(Guid id);
        Task<EtapaGestacional> CreateAsync(EtapaGestacional etapa);
        Task<bool> UpdateAsync(Guid id, EtapaGestacional etapa);
        Task<bool> DeleteAsync(Guid id);     
        Task<IEnumerable<EtapaGestacional>> SaveOrUpdEtapaGestaArrayAsync(IEnumerable<EtapaGestacional> etapas);
    }
}