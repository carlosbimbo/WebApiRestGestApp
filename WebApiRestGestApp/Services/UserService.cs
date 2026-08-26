using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Repositories;

namespace WebApiRestGestApp.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        // 1. Coincide con GetAllUsersAsync de la interfaz
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        // 2. Coincide con GetUserByIdAsync de la interfaz
        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        // 3. Coincide con CreateUserAsync de la interfaz
        public async Task<User> CreateUserAsync(User user)
        {
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
            }

            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            return await _userRepository.AddAsync(user);
        }

        // 4. Coincide con UpdateUserAsync de la interfaz
        public async Task<bool> UpdateUserAsync(Guid id, User user)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null) return false;

            UpdatePropertiesDynamically(existingUser, user);
            existingUser.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(existingUser);
            return true;
        }

        // 5. Coincide con DeleteUserAsync de la interfaz
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null) return false;

            await _userRepository.DeleteAsync(id);
            return true;
        }

        // Lógica de Guardar o Actualizar
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

        // MÉTODO DINÁMICO PARA EF CORE
        private void UpdatePropertiesDynamically(User existing, User incoming)
        {
            PropertyInfo[] properties = typeof(User).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                // Protegemos la llave primaria y fecha de creación
                if (property.Name == "Id" || property.Name == "CreatedAt")
                    continue;

                // Evitar propiedades de navegación de Entity Framework
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                var newValue = property.GetValue(incoming);
                property.SetValue(existing, newValue);
            }
        }
    }
}