using System.Threading.Tasks;

namespace WebApiRestGestApp.Services
{
    public interface IUserDataService
    {
        Task<object?> GetAllDataUserAsync(string username);
        Task<object?> GetAllDataUserByEmailAsync(string email);

        // NUEVO MÉTODO: Obtener fotos por username
        Task<object?> GetFotosPorUsuarioAsync(string username);
    }
}