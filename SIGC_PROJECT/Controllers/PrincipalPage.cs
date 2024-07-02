using Microsoft.AspNetCore.Mvc;
using SIGC_PROJECT.Helper;

namespace SIGC_PROJECT.Controllers
{
    public class PrincipalPage : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Service()
        {
            return View();
        }

        public IActionResult Contact() 
        {
            return View();
        }
    }
}
