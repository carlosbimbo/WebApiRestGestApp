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
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly AppDbContext _context; 

        public UserService(IRepository<User> userRepository, AppDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (user.Id == Guid.Empty) user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            return await _userRepository.AddAsync(user);
        }

        public async Task<bool> UpdateUserAsync(Guid id, User user)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null) return false;

            UpdatePropertiesDynamically(existingUser, user);
            existingUser.Id = id; // Proteger Id
            existingUser.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(existingUser);
            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null) return false;

            await _userRepository.DeleteAsync(id);
            return true;
        }

        public async Task<User> SaveOrUpdateAsync(User user)
        {
            var existing = await _userRepository.GetByIdAsync(user.Id);

            if (existing != null)
            {
                UpdatePropertiesDynamically(existing, user);
                existing.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(existing);
                return existing;
            }
            else
            {
                if (user.CreatedAt == null) user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                return await _userRepository.AddAsync(user);
            }
        }

        public async Task<User?> UpdateExpoTokenAsync(Guid userId, string expoPushToken)
        {
            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser == null) return null;

            existingUser.ExpoPushToken = expoPushToken;
            existingUser.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(existingUser);
            return existingUser;
        }

        // ==========================================
        // MIGRACIÓN OPTIMIZADA (BULK SAVE ANTI-DUPLICADOS)
        // ==========================================
        public async Task<IEnumerable<User>> SaveOrUpdUserSyncAsync(IEnumerable<User> usersList)
        {
            var results = new List<User>();

            // 1. FILTRO ANTI-DUPLICADOS en memoria
            var uniquePayload = usersList
                .GroupBy(u => u.Id)
                .Select(g => g.Last()) 
                .ToList();

            var userIds = uniquePayload.Select(u => u.Id).ToList();

            // 2. Traer registros existentes a RAM de un solo golpe
            var existingRecords = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            // 3. Cruzar datos e inyectarlos al contexto (sin impactar BD aún)
            foreach (var incoming in uniquePayload)
            {
                if (existingRecords.TryGetValue(incoming.Id, out var existing))
                {
                    UpdatePropertiesDynamically(existing, incoming);
                    existing.UpdatedAt = DateTime.UtcNow;

                    _context.Users.Update(existing);
                    results.Add(existing);
                }
                else
                {
                    if (incoming.Id == Guid.Empty) incoming.Id = Guid.NewGuid();
                    if (incoming.CreatedAt == null) incoming.CreatedAt = DateTime.UtcNow;
                    incoming.UpdatedAt = DateTime.UtcNow;

                    await _context.Users.AddAsync(incoming);
                    results.Add(incoming);
                }
            }
                        
            await _context.SaveChangesAsync();

            return results;
        }

        // ==========================================
        // MÉTODO DINÁMICO PARA EF CORE (Reflection)
        // ==========================================
        private void UpdatePropertiesDynamically(User existing, User incoming)
        {
            PropertyInfo[] properties = typeof(User).GetProperties();

            foreach (PropertyInfo property in properties)
            {              
                if (property.Name == nameof(User.Id) || property.Name == nameof(User.CreatedAt))
                    continue;

                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                var newValue = property.GetValue(incoming);
                property.SetValue(existing, newValue);
            }
        }

        
        public async Task<User?> UpdateExpoTokenByUsernameAsync(string username, string expoPushToken)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            string cleanUsername = username.Trim().ToLower();

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == cleanUsername);

            if (existingUser == null) return null;
                        
            string timeZoneId = OperatingSystem.IsWindows() ? "SA Pacific Standard Time" : "America/Lima";
                       
            TimeZoneInfo limaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            DateTime horaLima = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, limaTimeZone);
                       
            existingUser.ExpoPushToken = expoPushToken;
            existingUser.UpdatedAt = horaLima;

            await _context.SaveChangesAsync();

            return existingUser;
        }

        public async Task<User?> FindUserByNameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            string cleanUsername = username.Trim().ToLower();

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == cleanUsername);
        }

    }
}