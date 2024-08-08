using MCol.DAL.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCol.BLL.Controller
{
    public class UsuariosControllerBLL : BaseControllerBLL
    {
        private readonly IDbContextFactory<ColegiosCOLContext> _contextFactory;

        // Asegúrate de que el constructor sea público
        public UsuariosControllerBLL(IDbContextFactory<ColegiosCOLContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task CreateNewUserAsync()
        {
            try
            {
                using var _context = _contextFactory.CreateDbContext();

                var newUser = new tb_usuarios
                {
                    nombre = "Nombre Usuario",
                    password = "ContraseñaSegura123",
                    email = "usuario@example.com",
                };

                _context.tb_usuarios.Add(newUser);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex )
            {

                throw;
            }
          
        }
        public async Task<tb_usuarios> GetUserByLoginAsync( string nombre)
        {
            using var _context = _contextFactory.CreateDbContext();
            var user = await _context.tb_usuarios.FirstOrDefaultAsync(u => u.nombre == nombre);
            return user;
        }

        public async Task UpdateUserEmailAsync(string nombre, string newEmail)
        {
            using var _context = _contextFactory.CreateDbContext();
            var user = await _context.tb_usuarios
                .FirstOrDefaultAsync(u => u.nombre == nombre);

            if (user != null)
            {
                user.email = newEmail;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteUserAsync(string nombre)
        {
            using var _context = _contextFactory.CreateDbContext();
            var user = await _context.tb_usuarios
                .FirstOrDefaultAsync(u => u.nombre == nombre);

            if (user != null)
            {
                _context.tb_usuarios.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
