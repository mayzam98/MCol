using MCol.DAL.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCol.BLL.Controller
{
    public class SecurityController 
    {
        private readonly ColegiosCOLContext _context;

        public SecurityController(ColegiosCOLContext context)
        {
            _context = context;
        }

        public bool JwtCurrentUser(string username, string token)
        {
            var userSession = _context.tb_usuarios_logueados.FirstOrDefault(u => u.login == username && u.key_sesion == token);
            return userSession != null;
        }

        public void AddUserSession(string username, string token)
        {
            var userSession = new tb_usuarios_logueados
            {
                login = username,
                key_sesion = token,
                fecha_conexion = DateTime.Now
            };
            _context.tb_usuarios_logueados.Add(userSession);
            _context.SaveChanges();
        }

        public void RemoveUserSession(string token)
        {
            var userSession = _context.tb_usuarios_logueados.FirstOrDefault(u => u.key_sesion == token);
            if (userSession != null)
            {
                _context.tb_usuarios_logueados.Remove(userSession);
                _context.SaveChanges();
            }
        }

        public List<string> GetUserPermissions(string username, string page)
        {
            var user = _context.tb_usuarios.Include(u => u.tb_usuarios_perfiles).FirstOrDefault(u => u.login == username);
            if (user == null)
            {
                return new List<string>();
            }

            var userPermissions = from up in _context.tb_usuarios_perfiles
                                  join p in _context.tb_permisos on up.fk_id_perfil equals p.fk_id_perfil
                                  join pg in _context.tb_paginas on p.fk_id_pagina equals pg.id_pagina
                                  where up.fk_id_usuario == user.id_usuario && pg.descripcion == page
                                  select p;

            return userPermissions.Select(p => p.fecha_creacion.ToString()).ToList(); // assuming tipo_permiso contains permission types (read, write, etc.)
        }
    }

}
