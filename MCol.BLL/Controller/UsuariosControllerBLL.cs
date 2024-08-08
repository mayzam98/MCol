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
        public UsuariosControllerBLL(IDbContextFactory<ColegiosCOLContext> contextFactory)
         : base(contextFactory) { }
        public async Task CreateNewUserAsync()
        {
            try
            {
                using var _context = _contextFactory.CreateDbContext();

                var newUser = new tb_usuarios
                {
                    login = "Nombre Usuario",
                    password = "ContraseñaSegura123",
                    correo_electronico = "usuario@example.com",
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
            var user = await _context.tb_usuarios.FirstOrDefaultAsync(u => u.login == nombre);
            return user;
        }

        public async Task UpdateUserEmailAsync(string nombre, string newEmail)
        {
            using var _context = _contextFactory.CreateDbContext();
            var user = await _context.tb_usuarios
                .FirstOrDefaultAsync(u => u.login == nombre);

            if (user != null)
            {
                user.correo_electronico = newEmail;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteUserAsync(string nombre)
        {
            using var _context = _contextFactory.CreateDbContext();
            var user = await _context.tb_usuarios
                .FirstOrDefaultAsync(u => u.login == nombre);

            if (user != null)
            {
                _context.tb_usuarios.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
