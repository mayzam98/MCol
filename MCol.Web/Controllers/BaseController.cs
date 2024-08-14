using MCol.BLL.Controller;
using MCol.DTO.Security;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MCol.Web.Controllers
{
    public class BaseController : Controller
    {
        protected SecurityController _securityController;
        protected int CurrentUser { get; set; }
        private UserDTO sessionUser { get; set; }
        protected PermitDTO Permit;
        public BaseController(SecurityController securityController)
        {
            _securityController = securityController;
        }
        protected bool ValidateSession()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                var userJson = HttpContext.Session.GetString("User");
                if (!string.IsNullOrEmpty(userJson))
                {

                    Permit = new PermitDTO();
                    sessionUser = JsonConvert.DeserializeObject<UserDTO>(userJson);
                    var url = HttpContext.Request.Path.Value;
                    if (url != null)
                    {
                        if (!string.IsNullOrEmpty(url) && url != "/")
                        {
                            Permit = _securityController.Validate(url, sessionUser.Perfiles);
                        }
                        try
                        {
                            if (!Permit.Acceso && url != "/" && !url.Contains("Login") && !url.Contains("Home"))
                            {
                                return false;
                            }
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                    ViewBag.Permit = Permit;
                    var userDto = JsonConvert.DeserializeObject<UserDTO>(userJson);
                    sessionUser = userDto;
                    HttpContext.Session.SetString("Permissions", JsonConvert.SerializeObject(_securityController.GetPermissions(userDto.Perfiles)));
                    //HttpContext.Session.SetString("Menu", JsonConvert.SerializeObject(_securityController.GetMenu(userDto.Perfiles)));
                    // Save permissions and menu in session
                    var permissions = _securityController.GetPermissions(userDto.Perfiles);
                    HttpContext.Session.SetString("Permissions", JsonConvert.SerializeObject(permissions));

                    var menu = _securityController.GetMenu(userDto.Perfiles);
                    HttpContext.Session.SetString("Menu", JsonConvert.SerializeObject(menu));
                    ViewBag.Menu = menu;

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

        protected UserDTO SessionUser
        {
            get
            {
                if (sessionUser == null)
                {
                    if (HttpContext.Session.GetString("User") != null)
                    {
                        var json = HttpContext.Session.GetString("User");
                        sessionUser = JsonConvert.DeserializeObject<UserDTO>(json);
                        this.CurrentUser = sessionUser.Id;
                    }
                    else
                    {
                        sessionUser = new UserDTO();
                    }
                }

                return sessionUser;
            }
        }
    }

}
