using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGC_PROJECT.Helper;
using SIGC_PROJECT.Models;
using X.PagedList.Extensions;

namespace SIGC_PROJECT.Controllers
{
    public class PacientesController : Controller
    {
        private readonly SigcProjectContext _context;

        public PacientesController(SigcProjectContext context)
        {
            _context = context;
        }

        // GET: Pacientes
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<IActionResult> Index(int? pagina, int cantidadP = 10)
        {
            int numeroP = (pagina ?? 1);
            var pacientes = _context.Pacientes.OrderBy(p => p.PacienteId).ToPagedList(numeroP, cantidadP);
            return View(pacientes);
            //return View(await _context.Pacientes.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> buscarPacientesCedula(string cedula, int pagina = 1)
        {
            var pacientes = _context.Pacientes.Where(p => p.Cedula.Contains(cedula)).ToPagedList(pagina, 10);

            return View("Index", pacientes);
        }

        #region ===== Vistas de Configuracion =====

        [Authorize(Roles = "Paciente")]
        [FiltroRegistro]
        public async Task<IActionResult> Configuracion()
        {
            return View();
        }

        [Authorize(Roles = "Paciente")]
        public async Task<IActionResult> FormEdit()
        {
            //Obtener el id del usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var pacienteId = await _context.Pacientes.Where(p => p.IdUsuario == int.Parse(idUser))
                                                     .Select(p => p.PacienteId).FirstOrDefaultAsync();

            var model = await _context.Pacientes.FirstOrDefaultAsync(p => p.PacienteId == pacienteId);
            if (model == null)
            {
                return NotFound();
            }

            return PartialView("_FormEditPartialPaciente", model);
        }

        [Authorize(Roles = "Paciente")]
        public IActionResult FormPassword()
        {
            return PartialView("_CuentaUsuario");
        }

        #endregion

        // GET: Pacientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(m => m.PacienteId == id);
            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // GET: Pacientes/Create
        [Authorize(Roles = "Administrador,Paciente")]
        public IActionResult Create()
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

            if(rolActual == "Paciente")
            {
                var paciente = _context.Pacientes.FirstOrDefault(p => p.IdUsuario == int.Parse(idUser));

                if (paciente == null)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }

        // POST: Pacientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PacienteId,Cedula,Nombre,Apellido,FechaNacimiento,Edad,Genero,Direccion,Telefono,CorreoElectronico,HistorialMedico,IdUsuario")] Paciente paciente)
        {
            
            if (ModelState.IsValid)
            {
                var cedula = await _context.Pacientes.Where(p => p.Cedula == paciente.Cedula).FirstOrDefaultAsync();

                if (cedula != null)
                {
                    TempData["MensajeError"] = "Ya hay un paciente registrado con este número de cédula";
                }
                else if(cedula == null)
                {
                    _context.Add(paciente);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home", null);
                }

            }

            return View(paciente);
        }

        // GET: Pacientes/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }
            return View(paciente);
        }

        // POST: Pacientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PacienteId,Cedula,Nombre,Apellido,FechaNacimiento,Edad,Genero,Direccion,Telefono,CorreoElectronico,HistorialMedico,IdUsuario")] Paciente paciente)
        {
            if (id != paciente.PacienteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paciente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PacienteExists(paciente.PacienteId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                if (User.IsInRole("Administrador"))
                {
                    return RedirectToAction(nameof(Index));
                }
                else if (User.IsInRole("Paciente"))
                {
                    return RedirectToAction("Configuracion");
                }
                
            }
            return View(paciente);
        }

        // GET: Pacientes/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(m => m.PacienteId == id);
            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // POST: Pacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente != null)
            {
                _context.Pacientes.Remove(paciente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PacienteExists(int id)
        {
            return _context.Pacientes.Any(e => e.PacienteId == id);
        }
    }
}
