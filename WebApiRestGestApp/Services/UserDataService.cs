using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiRestGestApp.Data;

namespace WebApiRestGestApp.Services
{
    public class UserDataService : IUserDataService
    {
        private readonly AppDbContext _context;

        public UserDataService(AppDbContext context)
        {
            _context = context;
        }

        // Búsqueda por Username (El que ya tenías)
        public async Task<object?> GetAllDataUserAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            return await BuildUserDataResponseAsync(user);
        }

        // NUEVO MÉTODO: Búsqueda por Email
        public async Task<object?> GetAllDataUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            return await BuildUserDataResponseAsync(user);
        }

        // MÉTODO AUXILIAR: Construye el JSON idéntico para que no dupliques código
        private async Task<object> BuildUserDataResponseAsync(Models.User user)
        {
            var userId = user.Id;

            var etapas = await _context.EtapasGestacionales.Where(e => e.Id == userId).ToListAsync();
            var eventos = await _context.RegistroEventos.Where(e => e.IdUser == userId).ToListAsync();
            var hemoglobinas = await _context.RegistroHemoglobinas.Where(h => h.IdUser == userId).ToListAsync();
            var suplementos = await _context.RegistroSuplementos.Where(s => s.IdUser == userId).ToListAsync();
            var agendas = await _context.AgendasGestacionales.Where(a => a.Id == userId).ToListAsync();
            var dias = await _context.DiasGestaciones.Where(d => d.IdUser == userId).ToListAsync();

            return new Dictionary<string, object>
            {
                {
                    "CENAN2025", new
                    {
                        t_05_etapa_gestacional = new
                        {
                            etapagesta = etapas,
                            _cantidad = etapas.Count
                        },
                        users = new
                        {
                            usuarios = new[] { user },
                            _cantidad = 1
                        },
                        t_05_registro_eventos = new
                        {
                            eventos = eventos,
                            _cantidad = eventos.Count
                        },
                        t_05_registro_hemoglobina = new
                        {
                            hemoglobinas = hemoglobinas,
                            _cantidad = hemoglobinas.Count
                        },
                        t_05_registro_suplementos = new
                        {
                            suplement = suplementos,
                            _cantidad = suplementos.Count
                        },
                        t_05_agenda_gestacional = new
                        {
                            agendaGestacional = agendas,
                            _cantidad = agendas.Count
                        },
                        t_05_dias_gestacion = new
                        {
                            diasGestacion = dias,
                            _cantidad = dias.Count
                        }
                    }
                }
            };
        }
    }
}