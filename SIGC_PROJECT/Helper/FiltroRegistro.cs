using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SIGC_PROJECT.Models;
using System.Security.Claims;

namespace SIGC_PROJECT.Helper
{
    public class FiltroRegistro : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var user = context.HttpContext.User;

            //Verificar si el usuario se encuentra autenticado
            if (user.Identity.IsAuthenticated)
            {
                //Obtener el contexto de la base de datos
                var dbcontext = context.HttpContext.RequestServices.GetService(typeof(SigcProjectContext)) as SigcProjectContext;

                //Verificar si el usuario ha completado su informacion
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                //Obtener el rol del usuario 
                var rolActual = dbcontext.UsuarioRols.Where(r => r.IdUsuario == int.Parse(userId))
                                                    .Select(r => r.Rol.Nombre).FirstOrDefault();

                if(rolActual == "Paciente")
                {
                    var registroPac = dbcontext.Pacientes.FirstOrDefault(p => p.IdUsuario == int.Parse(userId));
                    if (registroPac == null)
                    {
                        //Redirigir al formulario de registro
                        context.Result = new RedirectToActionResult("Create", "Pacientes", null);
                    }
                }
            }
        }
    }
}
