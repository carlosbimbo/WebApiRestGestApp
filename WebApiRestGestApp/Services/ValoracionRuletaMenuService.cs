using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using WebApiRestGestApp.Data;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Repositories;

namespace WebApiRestGestApp.Services
{
    public class ValoracionRuletaMenuService : IValoracionRuletaMenuService
    {
        private readonly IRepository<ValoracionRuletaMenu> _repository;
        private readonly AppDbContext _context;

        public ValoracionRuletaMenuService(IRepository<ValoracionRuletaMenu> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<int> CountLikesByMenuAsync(int idmenu)
        {
            return await _context.ValoracionRuletaMenus
                .CountAsync(v => v.IdMenu == idmenu && v.FlagValora == 1);
        }

        public async Task<int> SaveOrUpdateValoracionAsync(ValoracionRuletaMenu data)
        {
            var existing = await GetByIdAsync(data.IdUser, data.IdMenu);

            if (existing != null)
            {
                // Uso del mapeo dinámico
                UpdatePropertiesDynamically(existing, data);

                _context.ValoracionRuletaMenus.Update(existing);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Crea el nuevo registro si no existía
                await _context.ValoracionRuletaMenus.AddAsync(data);
                await _context.SaveChangesAsync();
            }

            // Retorna el total de likes actualizado
            return await CountLikesByMenuAsync(data.IdMenu);
        }

      
        public async Task<IEnumerable<ValoracionRuletaMenu>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ValoracionRuletaMenu?> GetByIdAsync(string iduser, int idmenu)
        {
            return await _context.ValoracionRuletaMenus
                .FirstOrDefaultAsync(x => x.IdUser == iduser && x.IdMenu == idmenu);
        }

        public async Task<ValoracionRuletaMenu> CreateAsync(ValoracionRuletaMenu valoracion)
        {
            return await _repository.AddAsync(valoracion);
        }

        public async Task<bool> UpdateAsync(string iduser, int idmenu, ValoracionRuletaMenu valoracion)
        {
            var existing = await GetByIdAsync(iduser, idmenu);
            if (existing == null) return false;
            
            UpdatePropertiesDynamically(existing, valoracion);

            // Aseguramos que las llaves primarias no se pierdan por seguridad
            existing.IdUser = iduser;
            existing.IdMenu = idmenu;

            _context.ValoracionRuletaMenus.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string iduser, int idmenu)
        {
            var existing = await GetByIdAsync(iduser, idmenu);
            if (existing == null) return false;

            _context.ValoracionRuletaMenus.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
       
        private void UpdatePropertiesDynamically(ValoracionRuletaMenu existing, ValoracionRuletaMenu incoming)
        {
            PropertyInfo[] properties = typeof(ValoracionRuletaMenu).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                // Protegemos las llaves primarias compuestas
                if (property.Name == nameof(ValoracionRuletaMenu.IdUser) ||
                    property.Name == nameof(ValoracionRuletaMenu.IdMenu))
                    continue;

                // Evitar sobreescribir propiedades de navegación de Entity Framework
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                var newValue = property.GetValue(incoming);
                property.SetValue(existing, newValue);
            }
        }
    }
}