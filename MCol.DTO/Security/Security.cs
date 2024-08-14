using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCol.DTO.Security
{
    public class PermitDTO
    {
        public int IdPagina { get; set; }
        public string Pagina { get; set; }
        public string Icono { get; set; }
        public string Modulo { get; set; }
        public int IdPerfil { get; set; }
        public string Perfil { get; set; }
        public bool Crear { get; set; }
        public bool Actualizar { get; set; }
        public bool Leer { get; set; }
        public bool Borrar { get; set; }
        public bool Acceso
        {
            get
            {
                return Leer || Crear || Borrar || Actualizar;
            }
        }
    }

    public class PaginaDTO
    {
        public int Id { get; set; }
        public string NombrePagina { get; set; }
        public string Icono { get; set; }
        public ModuloDTO Modulo { get; set; }
        public string Path { get; set; }
        public string[] Reporte { get; set; }
        public int? Orden { get; set; }

        public bool Estado { get; set; }
    }

    public class ModuloDTO
    {
        public ModuloDTO()
        {

        }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Icono { get; set; }
        public int Orden { get; set; }
        public bool Estado { get; set; }

        public override bool Equals(object obj)
        {

            return this.Id == ((ModuloDTO)obj).Id;
        }
        public override int GetHashCode() { return 0; }
    }

    public class UserLoginDto {
        public UserLoginDto()
        {
            Username = string.Empty;
            Password = string.Empty;
        }

        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserDTO
    {
        public UserDTO()
        {
            Perfiles = new List<PerfilDTO>();
        }
        [DisplayName("Id")]
        public int Id { get; set; }
        [DisplayName("Login")]
        [Required]
        [StringLength(255)]
        public string UserName { get; set; }
        [DisplayName("Nombre")]
        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }
        [DisplayName("Contraseña")]
        [StringLength(255)]
        public string Password { get; set; }
        [StringLength(50)]
        [DisplayName("Correo Electrónico")]
        public string CorreoElectronico { get; set; }
        [DisplayName("Perfil")]
        public int IdPerfil { get; set; }
        public List<PerfilDTO> Perfiles { get; set; }
        public int? IdColegio{ get; set; }
        public string Colegio { get; set; }
        public string TokenAutorizacion { get; set; }
        public bool? CambioClave { get; set; }
        public bool Estado { get; set; }
        public string UsuarioCreacion { get; set; }
        [DisplayName("Fecha Creación")]
        public DateTime? FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Nombre { get; set; }
        public string CorreoElectronico { get; set; }
        public bool? CambioClave { get; set; }
        public bool Estado { get; set; }
        public string Empresa { get; set; }
    }

    public class PerfilDTO
    {
        public PerfilDTO()
        {
            Permisos = new List<PermitDTO>();
        }
        public int Id { get; set; }

        [DisplayName("Descripción")]
        [StringLength(200)]
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
        public bool Enable { get; set; }
        public List<PermitDTO> Permisos { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime? FechaCreacion { get; set; }

        public string UsuarioModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

    }



}
