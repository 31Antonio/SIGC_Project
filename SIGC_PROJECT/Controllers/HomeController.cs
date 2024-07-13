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
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
