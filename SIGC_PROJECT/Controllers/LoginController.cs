using SIGC_PROJECT.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SIGC_PROJECT.Models.ViewModel;
using System.Security.Claims;
using SIGC_PROJECT.Models;
using Microsoft.AspNetCore.Authorization;

namespace SIGC_PROJECT.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SigcProjectContext _context;

        public LoginController(SigcProjectContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Home");
            }
            else
            {
                return View();
            }
        }

        //METODO DEL INICIO DE SESION
        [BindProperty]
        public UsuarioVM UsuarioVM { get; set; }

        public async Task<IActionResult> InicioSesion()
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Ha ocurrido un error" });
            }
            else
            {
                var result = await _context.Usuarios.Include("UsuarioRoles.Rol").Where(u => u.NombreUsuario == UsuarioVM.nombre).SingleOrDefaultAsync();
                if (result == null)
                {
                    //return NotFound(new JObject()
                    //{
                    //    { "StatusCode", 403 },
                    //    { "Message", "Usuario o contraseña no valida." }
                    //});

                    return Json(new { success = false, message = "Usuario no encontrado" });

                }
                else
                {
                    if (HashHelper.CheckHash(UsuarioVM.contrasena, result.Contrasena, result.NombreUsuario))
                    {

                        if (result.UsuarioRoles.Count == 0)
                        {
                            return Json(new { success = false, message = "El usuario no tiene asignado ningún rol" });
                        }

                        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, result.IdUsuario.ToString()));
                        identity.AddClaim(new Claim(ClaimTypes.Name, result.NombreUsuario));


                        foreach (var Rol in result.UsuarioRoles)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, Rol.Rol.Nombre));
                        }

                        var principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                            new AuthenticationProperties { ExpiresUtc = DateTime.Now.AddDays(1), IsPersistent = true });

                        //Guardar la fecha de último acceso del usuario
                        result.FechaUltimoAcceso = DateTime.Now;
                        _context.SaveChanges();

                        return Ok();
                    }
                    else
                    {
                        return Json(new { success = false, message = "La contraseña es incorrecta" });
                    }
                }

            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Login/Login");
        }

    }
}
