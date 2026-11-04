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
                
        public async Task<object?> GetAllDataUserAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            return await BuildUserDataResponseAsync(user);
        }
                
        public async Task<object?> GetAllDataUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            return await BuildUserDataResponseAsync(user);
        }
                
        private async Task<object> BuildUserDataResponseAsync(Models.User user)
        {
            var userId = user.Id;
            var userIdString = user.Id.ToString();

            var etapas = await _context.EtapasGestacionales.Where(e => e.Id == userId).ToListAsync();
            var eventos = await _context.RegistroEventos.Where(e => e.IdUser == userId).ToListAsync();
            var hemoglobinas = await _context.RegistroHemoglobinas.Where(h => h.IdUser == userId).ToListAsync();
            var suplementos = await _context.RegistroSuplementos.Where(s => s.IdUser == userId).ToListAsync();
            var agendas = await _context.AgendasGestacionales.Where(a => a.Id == userId).ToListAsync();
            var dias = await _context.DiasGestaciones.Where(d => d.IdUser == userId).ToListAsync();
            var valormenus = await _context.ValoracionRuletaMenus.Where(d => d.IdUser == userIdString).ToListAsync();

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
                        },
                        t_05_valoracion_ruleta_menu = new
                        {
                            valoracionMenu = valormenus,
                            _cantidad = valormenus.Count
                        }
                    }
                }
            };
        }

        
        public async Task<object?> GetFotosPorUsuarioAsync(string username)
        {
            // 1. Buscamos al usuario por su username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            // Si el usuario no existe, retornamos null para que el controlador lance el error
            if (user == null) return null;

            // 2. Buscamos en la tabla de suplementos usando el Id del usuario encontrado
            var fotosList = await _context.RegistroSuplementos
                .Where(s => s.IdUser == user.Id && !string.IsNullOrEmpty(s.Foto))
                .Select(s => new
                {
                    foto = s.Foto,
                    // Concatenamos la ruta base con el nombre de la foto
                    url = $"https://www.macrocorpsystem.com/cenan2025/fotoscapture/{s.Foto}"
                })
                .ToListAsync();

            // 3. Retornamos el objeto encapsulado en la propiedad "fotos"
            return new { fotos = fotosList };
        }
    }
}