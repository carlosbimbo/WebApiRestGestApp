using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;

namespace WebApiRestGestApp.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(Guid id, User user);
        Task<bool> DeleteUserAsync(Guid id);
        Task<User> SaveOrUpdateAsync(User user);
        Task<User?> UpdateExpoTokenAsync(Guid userId, string expoPushToken);
        Task<IEnumerable<User>> SaveOrUpdUserSyncAsync(IEnumerable<User> usersList);
               
        Task<User?> UpdateExpoTokenByUsernameAsync(string username, string expoPushToken);
        Task<User?> FindUserByNameAsync(string username);
    }
}