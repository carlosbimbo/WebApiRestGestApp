using System.Threading.Tasks;

namespace WebApiRestGestApp.Services
{
    public interface IEmailService
    {
        Task<bool> SendRecoveryEmailAsync(string toEmail, string newPassword, string nombuser);
    }
}