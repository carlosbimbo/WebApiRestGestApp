using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Repositories;

namespace WebApiRestGestApp.Services
{
    public class RegistroHemoglobinaService : IRegistroHemoglobinaService
    {
        private readonly IRepository<RegistroHemoglobina> _repository;

        public RegistroHemoglobinaService(IRepository<RegistroHemoglobina> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RegistroHemoglobina>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<RegistroHemoglobina?> GetByIdsAsync(int idh, Guid idUser)
        {
            return await _repository.GetByIdAsync(idh, idUser);
        }

        public async Task<RegistroHemoglobina> CreateAsync(RegistroHemoglobina hemoglobina)
        {
            return await _repository.AddAsync(hemoglobina);
        }

        public async Task<bool> UpdateAsync(int idh, Guid idUser, RegistroHemoglobina hemoglobina)
        {
            var existing = await _repository.GetByIdAsync(idh, idUser);
            if (existing == null) return false;

            // Usamos el método dinámico para actualizar
            UpdatePropertiesDynamically(existing, hemoglobina);

            // Aseguramos que las llaves primarias se mantengan intactas
            existing.Idh = idh;
            existing.IdUser = idUser;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int idh, Guid idUser)
        {
            var existing = await _repository.GetByIdAsync(idh, idUser);
            if (existing == null) return false;

            await _repository.DeleteAsync(idh, idUser); // O pasa la entidad si tu repo lo requiere
            return true;
        }

        // ==========================================
        // NUEVO: Método migrado de Node.js (syncallregishemo)
        // ==========================================
        public async Task<IEnumerable<RegistroHemoglobina>> SaveOrUpdRegisHemoSyncAsync(IEnumerable<RegistroHemoglobina> hemoglobinas)
        {
            var results = new List<RegistroHemoglobina>();

            foreach (var incoming in hemoglobinas)
            {
                // Busca por llave primaria compuesta
                var existing = await _repository.GetByIdAsync(incoming.Idh, incoming.IdUser);

                if (existing != null)
                {
                    // Si existe, actualiza dinámicamente
                    UpdatePropertiesDynamically(existing, incoming);
                    await _repository.UpdateAsync(existing);
                    results.Add(existing);
                }
                else
                {
                    // Si no existe, lo crea
                    var created = await _repository.AddAsync(incoming);
                    results.Add(created);
                }
            }

            return results;
        }

        // ==========================================
        // MÉTODO DINÁMICO PARA EF CORE (Reflection)
        // ==========================================
        private void UpdatePropertiesDynamically(RegistroHemoglobina existing, RegistroHemoglobina incoming)
        {
            PropertyInfo[] properties = typeof(RegistroHemoglobina).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                // Protegemos las llaves primarias compuestas
                if (property.Name == nameof(RegistroHemoglobina.Idh) ||
                    property.Name == nameof(RegistroHemoglobina.IdUser))
                    continue;

                // Evitar sobreescribir propiedades de navegación
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                var newValue = property.GetValue(incoming);
                property.SetValue(existing, newValue);
            }
        }
    }
}