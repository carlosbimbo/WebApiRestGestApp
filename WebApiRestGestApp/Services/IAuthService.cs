using System.Threading.Tasks;
using WebApiRestGestApp.DTOs;

namespace WebApiRestGestApp.Services
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(SignInRequest request);
    }
}