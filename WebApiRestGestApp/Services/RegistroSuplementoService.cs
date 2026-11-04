using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Repositories;

namespace WebApiRestGestApp.Services
{
    public class RegistroSuplementoService : IRegistroSuplementoService
    {
        private readonly IRepository<RegistroSuplemento> _repository;

        public RegistroSuplementoService(IRepository<RegistroSuplemento> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RegistroSuplemento>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<RegistroSuplemento?> GetByIdsAsync(int idSuple, Guid idUser)
        {
            return await _repository.GetByIdAsync(idSuple, idUser);
        }

        public async Task<RegistroSuplemento> CreateAsync(RegistroSuplemento suplemento)
        {
            suplemento.CreatedAt = DateTime.UtcNow;
            suplemento.UpdatedAt = DateTime.UtcNow;
            return await _repository.AddAsync(suplemento);
        }

        public async Task<bool> UpdateAsync(int idSuple, Guid idUser, RegistroSuplemento suplemento)
        {
            var existing = await _repository.GetByIdAsync(idSuple, idUser);
            if (existing == null) return false;

            UpdatePropertiesDynamically(existing, suplemento);
            existing.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int idSuple, Guid idUser)
        {
            var existing = await _repository.GetByIdAsync(idSuple, idUser);
            if (existing == null) return false;

            await _repository.DeleteAsync(existing); // Asegúrate de que el repositorio acepte la entidad o pasa los IDs si está preparado para ello
            return true;
        }

        // ==========================================
        // NUEVO: Método migrado de Node.js (syncallsuple)
        // ==========================================
        public async Task<IEnumerable<RegistroSuplemento>> SaveOrUpdSupleSyncAsync(IEnumerable<RegistroSuplemento> suplementos)
        {
            var results = new List<RegistroSuplemento>();

            foreach (var incoming in suplementos)
            {
                var existing = await _repository.GetByIdAsync(incoming.IdSuple, incoming.IdUser);

                if (existing != null)
                {
                    // Actualizar dinámicamente si ya existe
                    UpdatePropertiesDynamically(existing, incoming);
                    existing.UpdatedAt = DateTime.UtcNow;

                    await _repository.UpdateAsync(existing);
                    results.Add(existing);
                }
                else
                {
                    // Crear nuevo si no existe
                    if (incoming.CreatedAt == null) incoming.CreatedAt = DateTime.UtcNow;
                    incoming.UpdatedAt = DateTime.UtcNow;

                    var created = await _repository.AddAsync(incoming);
                    results.Add(created);
                }
            }

            return results;
        }

        // ==========================================
        // MÉTODO DINÁMICO PARA EF CORE (Reflection)
        // ==========================================
        private void UpdatePropertiesDynamically(RegistroSuplemento existing, RegistroSuplemento incoming)
        {
            PropertyInfo[] properties = typeof(RegistroSuplemento).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                // Protegemos las llaves primarias y fechas de creación
                if (property.Name == nameof(RegistroSuplemento.IdSuple) ||
                    property.Name == nameof(RegistroSuplemento.IdUser) ||
                    property.Name == nameof(RegistroSuplemento.CreatedAt))
                    continue;

                // Evitar sobreescribir propiedades de navegación
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                var newValue = property.GetValue(incoming);

                // Opcional: Si quieres evitar sobreescribir con valores nulos enviados en el payload
                // if (newValue != null) 
                property.SetValue(existing, newValue);
            }
        }
    }
}