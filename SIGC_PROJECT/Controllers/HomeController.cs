using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGC_PROJECT.Helper;
using SIGC_PROJECT.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace SIGC_PROJECT.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly SigcProjectContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(SigcProjectContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //Obtener el id del usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (idUser == null)
            {
                return Unauthorized();
            }

            //Obtener el rol del usuario 
            var rolActual = _context.UsuarioRols.Where(r => r.IdUsuario == int.Parse(idUser))
                                                .Select(r => r.Rol.Nombre).FirstOrDefault();

            if (rolActual == null)
            {
                return Forbid();
            }

            if (rolActual == "Paciente")
            {
                var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.IdUsuario == int.Parse(idUser));

                if (paciente == null)
                {
                    return RedirectToAction("Create", "Pacientes");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                //Redirigir a la vista de index
                return View();
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
