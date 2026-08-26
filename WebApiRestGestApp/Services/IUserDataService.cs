using System.Threading.Tasks;

namespace WebApiRestGestApp.Services
{
    public interface IUserDataService
    {
        Task<object?> GetAllDataUserAsync(string username);
        Task<object?> GetAllDataUserByEmailAsync(string email);
    }
}