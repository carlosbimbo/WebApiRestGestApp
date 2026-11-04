using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;

namespace WebApiRestGestApp.Services
{
    public interface IValoracionRuletaMenuService
    {
        // ==========================================
        // CRUD ESTÁNDAR
        // ==========================================
        Task<IEnumerable<ValoracionRuletaMenu>> GetAllAsync();
        Task<ValoracionRuletaMenu?> GetByIdAsync(string iduser, int idmenu);
        Task<ValoracionRuletaMenu> CreateAsync(ValoracionRuletaMenu valoracion);
        Task<bool> UpdateAsync(string iduser, int idmenu, ValoracionRuletaMenu valoracion);
        Task<bool> DeleteAsync(string iduser, int idmenu);

        // ==========================================
        // MÉTODOS MIGRADOS DE NODE.JS
        // ==========================================
        Task<int> CountLikesByMenuAsync(int idmenu);
        Task<int> SaveOrUpdateValoracionAsync(ValoracionRuletaMenu data);
    }
}