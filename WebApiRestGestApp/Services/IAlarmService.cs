using System.Threading.Tasks;

namespace WebApiRestGestApp.Services
{
    public interface IAlarmService
    {
        Task ProcessAlarmsAsync(string triggerSource = "MANUAL");
    }
}