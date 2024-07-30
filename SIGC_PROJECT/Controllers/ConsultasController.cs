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
    public class ConsultasController : Controller
    {
        private readonly SigcProjectContext _context;

        public ConsultasController(SigcProjectContext context)
        {
            _context = context;
        }

        // GET: Consultas
        [Authorize(Roles = "Doctor,Secretaria,Paciente")]
        public async Task<IActionResult> Index()
        {
            var consultas = _context.Consultas.Select(c => new ConsultaGeneralVM
            {
                ConsultaId = c.ConsultaId,
                PacienteId = c.PacienteId,
                NombrePaciente = c.Paciente.Nombre + ' ' + c.Paciente.Apellido,
                EdadPaciente = c.Paciente.Edad,
                DoctorId = c.DoctorId,
                NombreDoctor = c.Doctor.Nombre + ' ' + c.Doctor.Apellido,
                EspecialidadDoctor = c.Doctor.Especialidad,
                FechaConsulta = c.FechaConsulta,
                MotivoConsulta = c.MotivoConsulta,
                Diagnostico = c.Diagnostico,
                Tratamiento = c.Tratamiento,
                RecetaId = c.RecetaId,
                Observaciones = c.Observaciones
            }).ToList();

            return View(consultas);
            //var sigcProjectContext = _context.Consultas.Include(c => c.Doctor).Include(c => c.Paciente).Include(c => c.Receta);
            //return View(await sigcProjectContext.ToListAsync());
        }

        // GET: Consultas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consulta = await _context.Consultas
                .Include(c => c.Doctor)
                .Include(c => c.Paciente)
                .Include(c => c.Receta)
                .FirstOrDefaultAsync(m => m.ConsultaId == id);
            if (consulta == null)
            {
                return NotFound();
            }

            return View(consulta);
        }

        // GET: Consultas/Create
        [Authorize(Roles = "Doctor")]
        public IActionResult Create()
        {
            // Obtener el id del Usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //Obtener el id de la secretaria
            var idDoctor = _context.Doctors.Where(s => s.IdUsuario == int.Parse(idUser)).Select(s => s.DoctorId).FirstOrDefault();

            ViewBag.DoctorId = idDoctor;
            return View();
        }

        //Buscar las pacientes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult buscarPacientePorCedula([FromBody] CedulaRequest request)
        {
            string cedula = request.Cedula;
            var paciente = _context.Pacientes.Where(p => p.Cedula == cedula)
                                             .Select(p => new
                                             {
                                                 NombreCompleto = p.Nombre + " " + p.Apellido,
                                                 p.Edad,
                                                 p.PacienteId,
                                             }).FirstOrDefault();

            if (paciente == null)
            {
                return Json(null);
            }

            return Json(paciente);
        }

        // POST: Consultas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConsultaVM model)
        {
            if (ModelState.IsValid)
            {
                var consulta = new Consulta
                {
                    PacienteId = model.PacienteId,
                    DoctorId = model.DoctorId,
                    FechaConsulta = model.FechaConsulta,
                    MotivoConsulta = model.MotivoConsulta,
                    Diagnostico = model.Diagnostico,
                    Tratamiento = model.Tratamiento,
                    Observaciones = model.Observaciones
                };

                _context.Consultas.Add(consulta);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Consultas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consulta = await _context.Consultas.FindAsync(id);
            if (consulta == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Nombre", consulta.DoctorId);
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre", consulta.PacienteId);
            ViewData["RecetaId"] = new SelectList(_context.Recetas, "RecetaId", "Indicaciones", consulta.RecetaId);
            return View(consulta);
        }

        // POST: Consultas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ConsultaId,PacienteId,DoctorId,FechaConsulta,MotivoConsulta,Diagnostico,Tratamiento,RecetaId,Observaciones")] Consulta consulta)
        {
            if (id != consulta.ConsultaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consulta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsultaExists(consulta.ConsultaId))
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
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Nombre", consulta.DoctorId);
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre", consulta.PacienteId);
            ViewData["RecetaId"] = new SelectList(_context.Recetas, "RecetaId", "Indicaciones", consulta.RecetaId);
            return View(consulta);
        }

        // GET: Consultas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consulta = await _context.Consultas
                .Include(c => c.Doctor)
                .Include(c => c.Paciente)
                .Include(c => c.Receta)
                .FirstOrDefaultAsync(m => m.ConsultaId == id);
            if (consulta == null)
            {
                return NotFound();
            }

            return View(consulta);
        }

        // POST: Consultas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consulta = await _context.Consultas.FindAsync(id);
            if (consulta != null)
            {
                _context.Consultas.Remove(consulta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConsultaExists(int id)
        {
            return _context.Consultas.Any(e => e.ConsultaId == id);
        }
    }
}

public class CedulaRequest
{
    public string Cedula { get; set; }
}