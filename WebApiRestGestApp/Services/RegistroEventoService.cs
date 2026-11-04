using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApiRestGestApp.Data;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Repositories;

namespace WebApiRestGestApp.Services
{
    public class RegistroEventoService : IRegistroEventoService
    {
        private readonly IRepository<RegistroEvento> _repository;
        private readonly AppDbContext _context; // <-- Requerido para el Bulk Save optimizado

        public RegistroEventoService(IRepository<RegistroEvento> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<RegistroEvento>> GetAllEventosAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<RegistroEvento?> GetEventoByIdsAsync(int idEven, Guid idUser)
        {
            return await _repository.GetByIdAsync(idEven, idUser);
        }

        public async Task<RegistroEvento> CreateEventoAsync(RegistroEvento evento)
        {
            return await _repository.AddAsync(evento);
        }

        public async Task<bool> UpdateEventoAsync(int idEven, Guid idUser, RegistroEvento evento)
        {
            var existing = await _repository.GetByIdAsync(idEven, idUser);
            if (existing == null) return false;

            UpdatePropertiesDynamically(existing, evento);
            existing.IdEven = idEven;
            existing.IdUser = idUser;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteEventoAsync(int idEven, Guid idUser)
        {
            var existing = await _repository.GetByIdAsync(idEven, idUser);
            if (existing == null) return false;

            await _repository.DeleteAsync(idEven, idUser);
            return true;
        }

        // ==========================================
        // MIGRACIÓN OPTIMIZADA (BULK SAVE ANTI-DUPLICADOS)
        // ==========================================
        public async Task<IEnumerable<RegistroEvento>> SaveOrUpdEventUserSyncAsync(IEnumerable<RegistroEvento> eventos)
        {
            var results = new List<RegistroEvento>();

            // 1. FILTRO ANTI-DUPLICADOS: Evita Tracking Exceptions si la app envía el mismo evento 2 veces
            var uniquePayload = eventos
                .GroupBy(e => e.IdEven + "_" + e.IdUser)
                .Select(g => g.Last())
                .ToList();

            // 2. Extraer IDs para consulta masiva
            var userIds = uniquePayload.Select(e => e.IdUser).Distinct().ToList();
            var eventIds = uniquePayload.Select(e => e.IdEven).Distinct().ToList();

            // 3. Traer registros existentes a memoria RAM de un solo golpe
            var existingRecords = await _context.RegistroEventos
                .Where(e => userIds.Contains(e.IdUser) && eventIds.Contains(e.IdEven))
                .ToDictionaryAsync(e => e.IdEven + "_" + e.IdUser);

            // 4. Procesamiento instantáneo en RAM
            foreach (var incoming in uniquePayload)
            {
                string key = incoming.IdEven + "_" + incoming.IdUser;

                if (existingRecords.TryGetValue(key, out var existing))
                {
                    UpdatePropertiesDynamically(existing, incoming);
                    _context.RegistroEventos.Update(existing);
                    results.Add(existing);
                }
                else
                {
                    await _context.RegistroEventos.AddAsync(incoming);
                    results.Add(incoming);
                }
            }

            // 5. Un solo impacto a la base de datos
            await _context.SaveChangesAsync();

            return results;
        }

        // ==========================================
        // MÉTODO DINÁMICO PARA EF CORE (Reflection)
        // ==========================================
        private void UpdatePropertiesDynamically(RegistroEvento existing, RegistroEvento incoming)
        {
            PropertyInfo[] properties = typeof(RegistroEvento).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                // Protegemos las llaves primarias compuestas
                if (property.Name == nameof(RegistroEvento.IdEven) ||
                    property.Name == nameof(RegistroEvento.IdUser))
                    continue;

                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                var newValue = property.GetValue(incoming);
                property.SetValue(existing, newValue);
            }
        }
    }
}