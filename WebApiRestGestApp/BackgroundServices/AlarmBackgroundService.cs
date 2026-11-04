using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiRestGestApp.Services;

namespace WebApiRestGestApp.BackgroundServices
{
    public class AlarmBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private bool _isRunning = false; // Bloqueo para evitar ejecuciones paralelas

        public AlarmBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("⏱ Alarm Cron (Background Service) inicializado en .NET");

            // Configurar el timer para que corra cada 2 minutos
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(2));

            while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                if (_isRunning)
                {
                    Console.WriteLine("⚠️ Cron ya está en ejecución, se omite...");
                    continue;
                }

                _isRunning = true;
                var watch = System.Diagnostics.Stopwatch.StartNew();

                try
                {
                    // Como los BackgroundService son "Singleton", necesitamos crear un Scope temporal
                    // para poder utilizar servicios transitorios como AppDbContext
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var alarmService = scope.ServiceProvider.GetRequiredService<IAlarmService>();
                        await alarmService.ProcessAlarmsAsync("CRON");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error en Alarm Cron: {ex.Message}");
                }
                finally
                {
                    watch.Stop();
                    Console.WriteLine($"✅ Cron finalizado en {watch.ElapsedMilliseconds} ms");
                    _isRunning = false;
                }
            }
        }
    }
}