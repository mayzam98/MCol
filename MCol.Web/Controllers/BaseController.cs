using MCol.BLL.Controller;
using MCol.DTO.Security;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MCol.Web.Controllers
{
    public class BaseController : Controller
    {
        protected SecurityController _securityController;

        public BaseController(SecurityController securityController)
        {
            _securityController = securityController;
        }

        protected bool ValidateSession()
        {
            if (HttpContext.Session.GetString("UserToken") != null)
            {
                var userJson = HttpContext.Session.GetString("User");
                if (!string.IsNullOrEmpty(userJson))
                {
                    var userDto = JsonConvert.DeserializeObject<UserDTO>(userJson);
                    HttpContext.Session.SetString("Permissions", JsonConvert.SerializeObject(_securityController.GetPermissions(userDto.Perfiles)));
                    HttpContext.Session.SetString("Menu", JsonConvert.SerializeObject(_securityController.GetMenu(userDto.Perfiles)));
                    return true;
                }
            }
            return false;
        }

        protected bool ValidateSessionPaginaDependiente(string urlNueva)
        {
            if (ValidateSession())
            {
                var permissionsJson = HttpContext.Session.GetString("Permissions");
                var permissions = JsonConvert.DeserializeObject<List<PermitDTO>>(permissionsJson);

                var pagePermission = permissions.FirstOrDefault(p => p.Pagina.Equals(urlNueva, StringComparison.OrdinalIgnoreCase));

                return pagePermission != null && pagePermission.Acceso;
            }
            return false;
        }
    }

}
