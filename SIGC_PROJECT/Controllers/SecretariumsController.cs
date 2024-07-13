using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGC_PROJECT.Models;
using SIGC_PROJECT.Models.ViewModel;

namespace SIGC_PROJECT.Controllers
{
    public class SecretariumsController : Controller
    {
        private readonly SigcProjectContext _context;

        public SecretariumsController(SigcProjectContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrador,Doctor")]
        // GET: Secretariums
        public async Task<IActionResult> Index()
        {
            return View(await _context.Secretaria.ToListAsync());
        }

        [Authorize(Roles = "Administrador,Doctor")]
        // GET: Secretariums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var secretarium = await _context.Secretaria
                .FirstOrDefaultAsync(m => m.SecretariaId == id);
            if (secretarium == null)
            {
                return NotFound();
            }

            return View(secretarium);
        }

        [Authorize(Roles = "Secretaria")]
        public async Task<IActionResult> Configuracion()
        {
            return View();
        }

        #region ===== APROBAR SOLICITUDES DE REGISTRO =====

        [Authorize(Roles = "Secretaria")]
        [HttpGet]
        public async Task<IActionResult> RegistrosPendientes()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var solicitudes = _context.SolicitudesRegistros.Where(s => s.IdSecretaria == int.Parse(userId)).ToList();

            return View(solicitudes);
        }

        [HttpPost]
        public async Task<IActionResult> AprobarRegistro(int id)
        {
            var solicitud = _context.SolicitudesRegistros.SingleOrDefault(s => s.IdSolicitud == id);

            if (solicitud != null)
            {
                //Crear nuevo usuario 
                var nuevoUsuario = new Usuario
                {
                    NombreUsuario = solicitud.Nombre,
                    Contrasena = solicitud.Clave
                };

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                //Asignar el rol de paciente al nuevo usuario
                var userRol = new UsuarioRol
                {
                    IdUsuario = nuevoUsuario.IdUsuario,
                    IdRol = ObtenerRol("Paciente")
                };

                _context.UsuarioRols.Add(userRol);
                await _context.SaveChangesAsync();

                //Eliminar la solicitud Aprobada
                _context.SolicitudesRegistros.Remove(solicitud);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("RegistrosPendientes");
        }

        [HttpGet]
        public IActionResult ObtenerConteo()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var conteo = _context.SolicitudesRegistros.Count(s => s.IdSecretaria == int.Parse(userId));

            return Json(new { conteo });
        }

        private int ObtenerRol(string rol)
        {
            var rolNombre = _context.Rols.SingleOrDefault(r => r.Nombre == rol);
            return rolNombre != null ? rolNombre.IdRol : -1;
        }

        #endregion

        // GET: Secretariums/Create
        [Authorize(Roles = "Administrador,Doctor")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Secretariums/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SecretariaId,Cedula,Nombre,Apellido,Telefono,CorreoElectronico,HorarioTrabajo")] Secretarium secretarium)
        {
            if (ModelState.IsValid)
            {
                _context.Add(secretarium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(secretarium);
        }

        // GET: Secretariums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var secretarium = await _context.Secretaria.FindAsync(id);
            if (secretarium == null)
            {
                return NotFound();
            }
            return View(secretarium);
        }

        // POST: Secretariums/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SecretariaId,Cedula,Nombre,Apellido,Telefono,CorreoElectronico,HorarioTrabajo")] Secretarium secretarium)
        {
            if (id != secretarium.SecretariaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(secretarium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SecretariumExists(secretarium.SecretariaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(secretarium);
        }

        // GET: Secretariums/Delete/5
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var secretarium = await _context.Secretaria
                .FirstOrDefaultAsync(m => m.SecretariaId == id);
            if (secretarium == null)
            {
                return NotFound();
            }

            return View(secretarium);
        }

        // POST: Secretariums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var secretarium = await _context.Secretaria.FindAsync(id);
            if (secretarium != null)
            {
                _context.Secretaria.Remove(secretarium);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SecretariumExists(int id)
        {
            return _context.Secretaria.Any(e => e.SecretariaId == id);
        }
    }
}
