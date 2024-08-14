using MCol.DAL.Modelo;
using MCol.DTO.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MCol.BLL.Controller
{
    public class SecurityController : BaseControllerBLL
    {
        private readonly ColegiosCOLContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor; 
        private static string _secretKey;
        private readonly IConfiguration _configuration;


        public SecurityController(IDbContextFactory<ColegiosCOLContext> contextFactory, IHttpContextAccessor httpContextAccessor , IConfiguration configuration) : base(contextFactory)
        {
            _context = CreateDbContext();
            _httpContextAccessor = httpContextAccessor;
            _secretKey = configuration["Jwt:Key"];
            _configuration = configuration;

        }

        public UserDTO Login(string username, string password)
        {
            try
            {
                var user = _context.tb_usuarios
               .Include(u => u.tb_usuarios_perfiles)
               .FirstOrDefault(u => u.login == username && u.password == password/*Sha256(password)*/);
                if (user != null)
                {
                    return GetUsuario(username);
                }
            }
            catch (Exception)
            {

            }
            return null;
        }
        public UserDTO GetUsuario(string username, int id = 0) 
        {   
            UserDTO userDTO = null;
            try
            {
                var user = _context.tb_usuarios
               .Include(u => u.tb_usuarios_perfiles)
               .FirstOrDefault(u => u.login == username/*Sha256(password)*/);

                if (user != null)
                {
                    userDTO = new UserDTO
                    {
                        Id = user.id_usuario,
                        UserName = user.login,
                        Nombre = user.nombre_completo,
                        CorreoElectronico = user.correo_electronico,
                        Perfiles = _context.tb_usuarios_perfiles
                            .Where(up => up.fk_id_usuario == user.id_usuario)
                            .Select(up => new PerfilDTO
                            {
                                Id = up.fk_id_perfil,
                                Descripcion = "problema?entity?",//up.tbl_perfiles.descripcion,
                                Estado = (bool)up.estado,
                                Permisos = _context.tb_permisos
                                    .Include(p => p.fk_id_paginaNavigation)
                                    .Include(p => p.fk_id_paginaNavigation.fk_id_moduloNavigation)
                                    .Where(p => p.fk_id_perfil == up.fk_id_perfil)
                                    .Select(p => new PermitDTO
                                    {
                                        Actualizar = p.actualizar,
                                        Borrar = p.borrar,
                                        Crear = p.crear,
                                        Leer = p.lectura,
                                        IdPagina = (int)p.fk_id_pagina, 
                                        IdPerfil = p.fk_id_perfil,
                                        Icono = p.fk_id_paginaNavigation.icono,
                                        Modulo = p.fk_id_paginaNavigation.fk_id_moduloNavigation.descripcion,
                                        Pagina = p.fk_id_paginaNavigation.descripcion,
                                        Perfil = p.fk_id_perfilNavigation.descripcion                                   
                                    }).ToList(),
                            }).ToList(),
                        CambioClave = user.cambio_clave,
                        IdColegio = user.fk_id_colegio,
                        Colegio = user.fk_id_colegio != null ? _context.tb_colegios
                            .FirstOrDefault(c => c.id_colegio == user.fk_id_colegio)
                            .nombre : "",
                        Estado = user.estado,
                        FechaCreacion = user.fecha_creacion,
                        FechaModificacion = user.fecha_modificacion,
                        TokenAutorizacion = user.token,
                        UsuarioCreacion = user.usuario_creacion,
                        UsuarioModificacion = user.usuario_modificacion,    
                        Password = "",

                    };

                    userDTO.TokenAutorizacion = GenerateToken(userDTO.UserName);
                    ControlInicioSessionUser(userDTO);
                    return userDTO;
                }
                return userDTO;

            }
            catch (Exception)
            {

                throw;
            }  
           


        } 


        public List<PermitDTO> GetPermissions(List<PerfilDTO> perfiles)
        {
            var permissions = new List<PermitDTO>();

            var paginas = _context.tb_paginas.Include(p => p.fk_id_moduloNavigation).Include(p => p.tb_permisos).ToList();

            foreach (var pagina in paginas)
            {
                var permiso = new PermitDTO
                {
                    IdPagina = pagina.id_pagina,
                    Pagina = pagina.descripcion,
                    Icono = pagina.icono,
                    Modulo = pagina.fk_id_moduloNavigation.descripcion,//pagina.tbl_modulos.nombre
                };

                foreach (var perfil in perfiles)
                {
                    var permisoPerfil = _context.tb_permisos.FirstOrDefault(p => p.fk_id_pagina == pagina.id_pagina && p.fk_id_perfil == perfil.Id);

                    if (permisoPerfil != null)
                    {
                        permiso.Leer = permiso.Leer || permisoPerfil.lectura;
                        permiso.Crear = permiso.Crear || permisoPerfil.crear;
                        permiso.Actualizar = permiso.Actualizar || permisoPerfil.actualizar;
                        permiso.Borrar = permiso.Borrar || permisoPerfil.borrar;
                    }
                }

                if (permiso.Acceso)
                {
                    permissions.Add(permiso);
                }
            }

            return permissions;
        }

        public List<PaginaDTO> GetMenu(List<PerfilDTO> perfiles)
        {
            // Obtener todos los permisos en memoria primero
            var permisos = GetPermissions(perfiles).Where(p => p.Acceso).ToList();

            // Obtener todas las páginas desde la base de datos
            var paginas = _context.tb_paginas
                .Include(p => p.fk_id_moduloNavigation)
                .ToList();

            // Filtrar las páginas en memoria utilizando los permisos obtenidos anteriormente
            var paginasFiltradas = paginas
                .Where(p => permisos.Any(pe => pe.IdPagina == p.id_pagina))
                .ToList();

            return paginasFiltradas.Select(p => new PaginaDTO
            {
                Id = p.id_pagina,
                NombrePagina = p.descripcion,
                Icono = p.icono,
                Modulo = new ModuloDTO
                {
                    Id = p.fk_id_moduloNavigation.id_modulo,
                    Nombre = p.fk_id_moduloNavigation.descripcion,
                    Icono = p.fk_id_moduloNavigation.imagenIcono,
                    Estado = p.fk_id_moduloNavigation.estado
                },
                Path = p.ruta,
                Estado = p.estado
            }).ToList();
        }

        public void ControlInicioSessionUser(UserDTO user)
        {
            var userSession = _context.tb_usuarios_logueados.FirstOrDefault(u => u.login == user.UserName);

            if (userSession != null)
            {
                userSession.ip_conexion = GetIP();
                userSession.fecha_conexion = DateTime.Now;
                userSession.key_sesion = user.TokenAutorizacion;
            }
            else
            {
                _context.tb_usuarios_logueados.Add(new tb_usuarios_logueados
                {
                    login = user.UserName,
                    fecha_conexion = DateTime.Now,
                    ip_conexion = GetIP(),
                    key_sesion = user.TokenAutorizacion
                });
            }

            _context.SaveChanges();
        }

        public void ControlFinalSessionUser(string token)
        {
            try
            {
                var userSession = _context.tb_usuarios_logueados.FirstOrDefault(u => u.key_sesion == token);
                var secretKey = _configuration["Jwt:Key"];
                var audienceToken = _configuration["Jwt:Audience"];
                var issuerToken = _configuration["Jwt:Issuer"];
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                var validationParameters = new TokenValidationParameters()
                {
                    ValidAudience = audienceToken,
                    ValidIssuer = issuerToken,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey
                };
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);
                var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
                var userName = claimsIdentity.Name;
                if (userSession != null)
                {
                    _context.tb_usuarios_logueados.Remove(userSession);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {

            }
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

        public List<PermitDTO> GetUserPermissions(string username, string page)
        {
            var user = _context.tb_usuarios
                .Include(u => u.tb_usuarios_perfiles)
                .FirstOrDefault(u => u.login == username);

            if (user == null)
            {
                return new List<PermitDTO>();
            }

            var userPermissions = from up in _context.tb_usuarios_perfiles
                                  join p in _context.tb_permisos on up.fk_id_perfil equals p.fk_id_perfil
                                  join pg in _context.tb_paginas on p.fk_id_pagina equals pg.id_pagina
                                  join mdl in _context.tb_modulos on pg.fk_id_modulo equals mdl.id_modulo
                                  join perf in _context.tb_perfiles on p.fk_id_perfil equals perf.id_perfil
                                  where up.fk_id_usuario == user.id_usuario && pg.descripcion == page
                                  select new PermitDTO
                                  {
                                      IdPagina = pg.id_pagina,
                                      Pagina = pg.descripcion,
                                      Icono = pg.icono,
                                      Modulo = mdl.descripcion,
                                      IdPerfil = p.fk_id_perfil,
                                      Perfil = perf.descripcion,
                                      Crear = p.crear,
                                      Actualizar = p.actualizar,
                                      Leer = p.lectura,
                                      Borrar = p.borrar
                                  };

            return userPermissions.ToList();
        }

        public List<PaginaDTO> GetUserMenu(string username)
        {
            var user = _context.tb_usuarios
                .Include(u => u.tb_usuarios_perfiles)
                .FirstOrDefault(u => u.login == username);

            if (user == null)
            {
                return new List<PaginaDTO>();
            }

            var userPermissions = from up in _context.tb_usuarios_perfiles
                                  join p in _context.tb_permisos on up.fk_id_perfil equals p.fk_id_perfil
                                  join pg in _context.tb_paginas on p.fk_id_pagina equals pg.id_pagina
                                  join mdl in _context.tb_modulos on pg.fk_id_modulo equals mdl.id_modulo
                                  where up.fk_id_usuario == user.id_usuario && p.lectura // Se asume que el permiso de lectura define el acceso al menú
                                  select new PaginaDTO
                                  {
                                      Id = pg.id_pagina,
                                      NombrePagina = pg.descripcion,
                                      Icono = pg.icono,
                                      Path = pg.ruta,
                                      Orden = pg.orden,
                                      Estado = pg.estado,
                                      Modulo = new ModuloDTO
                                      {
                                          Id = mdl.id_modulo,
                                          Nombre = mdl.descripcion,
                                          Icono = mdl.imagenIcono,
                                          Orden = mdl.orden,
                                          Estado = mdl.estado
                                      }
                                  };

            return userPermissions.Distinct().OrderBy(p => p.Modulo.Orden).ThenBy(p => p.Orden).ToList();
        }

        public string GenerateToken(string username, IEnumerable<Claim> additionalClaims = null)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_very_long_secret_key_here_which_is_32_chars"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        public string GenerateToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static string Sha256(string data)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string GetIP()
            {
                string IP;
                try
                {
                    var httpContext = _httpContextAccessor.HttpContext;

                    // Obtener el nombre del host
                    string strHostName = Dns.GetHostName();
                    IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
                    System.Diagnostics.Debug.WriteLine("Dirección IP: " + Convert.ToString(ipEntry.AddressList[ipEntry.AddressList.Length - 1]));
                    System.Diagnostics.Debug.WriteLine("HostName: " + Convert.ToString(ipEntry.HostName));

                    // Obtener la IP del cliente desde los encabezados
                    string ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                    if (!string.IsNullOrEmpty(ipAddress))
                    {
                        IP = ipAddress;
                    }
                    else
                    {
                        // Si no está en los encabezados, obtener la IP remota directamente
                        IP = httpContext.Connection.RemoteIpAddress?.ToString();
                    }

                    if (IP.Length >= 50)
                    {
                        IP = IP.Substring(0, 50);
                    }
                }
                catch (Exception)
                {
                    IP = "Sin valor";
                }
                return IP;
            }

        public PermitDTO Validate(string url, List<PerfilDTO> perfiles)
        {
            var permit = new PermitDTO();
            var urlSplit = url.Split('/');

            var controller = urlSplit[1];
            var action = urlSplit.Length > 2 ? urlSplit[2] : "";

            var page = _context.tb_paginas.FirstOrDefault(p => p.ruta.Contains(controller) && p.ruta.Contains(action));
            if (page == null)
                page = _context.tb_paginas.FirstOrDefault(p => url.Contains(p.ruta));
            if (page != null)
            {
                permit = GetPermissions(perfiles).FirstOrDefault(p => p.IdPagina == page.id_pagina);
            }
            return permit;
        }
    }
}
