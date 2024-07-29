using Azure.Core;
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

        [FiltroRegistro]
        public async Task<IActionResult> Index()
        {
            // Obtener el id del Usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Crear una variable con la fecha de hoy
            DateTime fechaActual = DateTime.Now;

            if (User.IsInRole("Secretaria"))
            {
                //Obtener el id del Doctor
                var idDoctor = await _context.Secretaria.Where(s => s.IdUsuario == int.Parse(idUser)).Select(s => s.IdDoctor).FirstOrDefaultAsync();
                return RedirectToAction("CitasDelDia", "Citas", new { date = fechaActual.ToString("yyyy-MM-dd"), doctorId = idDoctor });

            }
            else if (User.IsInRole("Doctor"))
            {
                //Obtener el id del Doctor
                var idDoctor = await _context.Doctors.Where(s => s.IdUsuario == int.Parse(idUser)).Select(s => s.DoctorId).FirstOrDefaultAsync();
                return RedirectToAction("CitasDelDia", "Citas", new { date = fechaActual.ToString("yyyy-MM-dd"), doctorId = idDoctor });

            }

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
