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
    public class AgendaGestacionalService : IAgendaGestacionalService
    {
        private readonly IRepository<AgendaGestacional> _repository;
        private readonly AppDbContext _context; // <-- Agregado para operaciones Bulk

        public AgendaGestacionalService(IRepository<AgendaGestacional> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<AgendaGestacional>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<AgendaGestacional?> GetByIdsAsync(Guid id, int nrosem)
        {
            return await _repository.GetByIdAsync(id, nrosem);
        }

        public async Task<AgendaGestacional> CreateAsync(AgendaGestacional agenda)
        {
            return await _repository.AddAsync(agenda);
        }

        public async Task<bool> UpdateAsync(Guid id, int nrosem, AgendaGestacional agenda)
        {
            var existing = await _repository.GetByIdAsync(id, nrosem);
            if (existing == null) return false;

            UpdatePropertiesDynamically(existing, agenda);
            existing.Id = id;
            existing.Nrosem = nrosem;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, int nrosem)
        {
            var existing = await _repository.GetByIdAsync(id, nrosem);
            if (existing == null) return false;

            await _repository.DeleteAsync(id, nrosem);
            return true;
        }

        // ==========================================
        // MIGRACIÓN OPTIMIZADA (BULK SAVE ANTI-DUPLICADOS)
        // ==========================================
        public async Task<IEnumerable<AgendaGestacional>> SaveOrUpdAgendaGestacionalSyncAsync(IEnumerable<AgendaGestacional> agendaList)
        {
            var results = new List<AgendaGestacional>();

            // 1. FILTRO ANTI-DUPLICADOS: Evita caídas por llaves duplicadas en el payload JSON
            var uniquePayload = agendaList
                .GroupBy(a => a.Id + "_" + a.Nrosem)
                .Select(g => g.Last()) // Tomamos el más reciente en caso de repetidos
                .ToList();

            // 2. Extraer listas de IDs para búsqueda masiva (OJO: Id es Guid y Nrosem es int)
            var userIds = uniquePayload.Select(a => a.Id).Distinct().ToList();
            var nroSems = uniquePayload.Select(a => a.Nrosem).Distinct().ToList();

            // 3. Descargar registros existentes a la memoria RAM
            var existingRecords = await _context.AgendasGestacionales
                .Where(a => userIds.Contains(a.Id) && nroSems.Contains(a.Nrosem))
                .ToDictionaryAsync(a => a.Id + "_" + a.Nrosem);

            // 4. Procesamiento instantáneo (sin tocar la BD)
            foreach (var incoming in uniquePayload)
            {
                string key = incoming.Id + "_" + incoming.Nrosem;

                if (existingRecords.TryGetValue(key, out var existing))
                {
                    UpdatePropertiesDynamically(existing, incoming);

                    _context.AgendasGestacionales.Update(existing);
                    results.Add(existing);
                }
                else
                {
                    await _context.AgendasGestacionales.AddAsync(incoming);

                    // Aseguramos el diccionario para evitar duplicidad de rastreo
                    existingRecords[key] = incoming;
                    results.Add(incoming);
                }
            }

            // 5. Impactar la base de datos de 1 sola vez
            await _context.SaveChangesAsync();

            return results;
        }

        // ==========================================
        // MÉTODO DINÁMICO PARA EF CORE (Reflection)
        // ==========================================
        private void UpdatePropertiesDynamically(AgendaGestacional existing, AgendaGestacional incoming)
        {
            PropertyInfo[] properties = typeof(AgendaGestacional).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                // Protegemos las dos columnas de la llave primaria
                if (property.Name == nameof(AgendaGestacional.Id) ||
                    property.Name == nameof(AgendaGestacional.Nrosem))
                    continue;

                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                var newValue = property.GetValue(incoming);
                property.SetValue(existing, newValue);
            }
        }
    }
}