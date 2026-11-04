using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Repositories;

namespace WebApiRestGestApp.Services
{
    public class EtapaGestacionalService : IEtapaGestacionalService
    {
        private readonly IRepository<EtapaGestacional> _repository;

        public EtapaGestacionalService(IRepository<EtapaGestacional> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<EtapaGestacional>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<EtapaGestacional?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<EtapaGestacional> CreateAsync(EtapaGestacional etapa)
        {
            if (etapa.Id == Guid.Empty) etapa.Id = Guid.NewGuid();
            return await _repository.AddAsync(etapa);
        }

        public async Task<bool> UpdateAsync(Guid id, EtapaGestacional etapa)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            UpdatePropertiesDynamically(existing, etapa);

            // Proteger el Id
            existing.Id = id;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }

        // ==========================================
        // NUEVO: Método migrado de Node.js
        // ==========================================
        public async Task<IEnumerable<EtapaGestacional>> SaveOrUpdEtapaGestaArrayAsync(IEnumerable<EtapaGestacional> etapas)
        {
            var results = new List<EtapaGestacional>();

            foreach (var incoming in etapas)
            {
                if (incoming.Id == Guid.Empty)
                {
                    incoming.Id = Guid.NewGuid();
                }

                var existing = await _repository.GetByIdAsync(incoming.Id);

                if (existing != null)
                {
                    UpdatePropertiesDynamically(existing, incoming);
                    await _repository.UpdateAsync(existing);
                    results.Add(existing);
                }
                else
                {
                    var created = await _repository.AddAsync(incoming);
                    results.Add(created);
                }
            }

            return results;
        }

        // ==========================================
        // MÉTODO DINÁMICO PARA EF CORE (Reflection)
        // ==========================================
        private void UpdatePropertiesDynamically(EtapaGestacional existing, EtapaGestacional incoming)
        {
            PropertyInfo[] properties = typeof(EtapaGestacional).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                // Protegemos la llave primaria
                if (property.Name == nameof(EtapaGestacional.Id))
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