using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using WebApiRestGestApp.Data;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Repositories;

namespace WebApiRestGestApp.Services
{
    public class DiasGestacionService : IDiasGestacionService
    {
        private readonly IRepository<DiasGestacion> _repository;
        private readonly AppDbContext _context; 

        public DiasGestacionService(IRepository<DiasGestacion> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<DiasGestacion>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<DiasGestacion?> GetByIdsAsync(int idDiasg, Guid idUser)
        {
            return await _repository.GetByIdAsync(idDiasg, idUser);
        }

        public async Task<DiasGestacion> CreateAsync(DiasGestacion dias)
        {
            return await _repository.AddAsync(dias);
        }

        public async Task<bool> UpdateAsync(int idDiasg, Guid idUser, DiasGestacion dias)
        {
            var existing = await _repository.GetByIdAsync(idDiasg, idUser);
            if (existing == null) return false;

            UpdatePropertiesDynamically(existing, dias);
            existing.IdDiasg = idDiasg;
            existing.IdUser = idUser;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int idDiasg, Guid idUser)
        {
            var existing = await _repository.GetByIdAsync(idDiasg, idUser);
            if (existing == null) return false;

            await _repository.DeleteAsync(idDiasg, idUser);
            return true;
        }

        // ==========================================
        // MIGRACIÓN OPTIMIZADA (BULK SAVE ANTI-DUPLICADOS)
        // ==========================================
        public async Task<IEnumerable<DiasGestacion>> SaveOrUpdT05diasGestacionSyncAsync(IEnumerable<DiasGestacion> diasList)
        {
            var results = new List<DiasGestacion>();

            // 1. FILTRO ANTI-DUPLICADOS: Si la app móvil envía el mismo registro 2 veces en el mismo JSON, 
            // nos quedamos solo con el último para evitar que Entity Framework colapse (Tracking Exception).
            var uniquePayload = diasList
                .GroupBy(d => d.IdDiasg + "_" + d.IdUser)
                .Select(g => g.Last()) // Tomamos el último enviado en caso de duplicidad
                .ToList();

            // 2. Extraer IDs limpios para consulta masiva
            var userIds = uniquePayload.Select(d => d.IdUser).Distinct().ToList();
            var diasgIds = uniquePayload.Select(d => d.IdDiasg).Distinct().ToList();

            // 3. Traer registros existentes de golpe a la memoria RAM
            var existingRecords = await _context.DiasGestaciones
                .Where(d => userIds.Contains(d.IdUser) && diasgIds.Contains(d.IdDiasg))
                .ToDictionaryAsync(d => d.IdDiasg + "_" + d.IdUser);

            // 4. Procesamiento instantáneo en memoria RAM
            foreach (var incoming in uniquePayload)
            {
                string key = incoming.IdDiasg + "_" + incoming.IdUser;

                if (existingRecords.TryGetValue(key, out var existing))
                {
                    // Si ya existe en la BD, lo actualizamos
                    UpdatePropertiesDynamically(existing, incoming);

                    _context.DiasGestaciones.Update(existing);
                    results.Add(existing);
                }
                else
                {
                    // Si es nuevo, lo preparamos para insertar
                    await _context.DiasGestaciones.AddAsync(incoming);
                    results.Add(incoming);
                }
            }

            // 5. Un solo guardado masivo en PostgreSQL
            await _context.SaveChangesAsync();

            return results;
        }

        // ==========================================
        // MÉTODO DINÁMICO PARA EF CORE (Reflection)
        // ==========================================
        private void UpdatePropertiesDynamically(DiasGestacion existing, DiasGestacion incoming)
        {
            PropertyInfo[] properties = typeof(DiasGestacion).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.Name == nameof(DiasGestacion.IdDiasg) ||
                    property.Name == nameof(DiasGestacion.IdUser))
                    continue;

                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                var newValue = property.GetValue(incoming);
                property.SetValue(existing, newValue);
            }
        }
    }
}