using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGC_PROJECT.Models;

namespace SIGC_PROJECT.Controllers
{
    public class CitasController : Controller
    {
        private readonly SigcProjectContext _context;

        public CitasController(SigcProjectContext context)
        {
            _context = context;
        }

        // GET: Citas
        public async Task<IActionResult> Index()
        {
            var sigcProjectContext = _context.Citas.Include(c => c.Doctor).Include(c => c.Paciente).Include(c => c.Secretaria);
            return View(await sigcProjectContext.ToListAsync());
        }

        // GET: Citas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas
                .Include(c => c.Doctor)
                .Include(c => c.Paciente)
                .Include(c => c.Secretaria)
                .FirstOrDefaultAsync(m => m.CitaId == id);
            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }

        // GET: Citas/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Nombre");
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre");
            ViewData["SecretariaId"] = new SelectList(_context.Secretaria, "SecretariaId", "Nombre");
            return View();
        }

        // POST: Citas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CitaId,PacienteId,DoctorId,SecretariaId,Estado,Comentario")] Cita cita)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Nombre", cita.DoctorId);
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre", cita.PacienteId);
            ViewData["SecretariaId"] = new SelectList(_context.Secretaria, "SecretariaId", "Nombre", cita.SecretariaId);
            return View(cita);
        }

        // GET: Citas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Nombre", cita.DoctorId);
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre", cita.PacienteId);
            ViewData["SecretariaId"] = new SelectList(_context.Secretaria, "SecretariaId", "Nombre", cita.SecretariaId);
            return View(cita);
        }

        // POST: Citas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CitaId,PacienteId,DoctorId,SecretariaId,Estado,Comentario")] Cita cita)
        {
            if (id != cita.CitaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitaExists(cita.CitaId))
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
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Nombre", cita.DoctorId);
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre", cita.PacienteId);
            ViewData["SecretariaId"] = new SelectList(_context.Secretaria, "SecretariaId", "Nombre", cita.SecretariaId);
            return View(cita);
        }

        // GET: Citas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas
                .Include(c => c.Doctor)
                .Include(c => c.Paciente)
                .Include(c => c.Secretaria)
                .FirstOrDefaultAsync(m => m.CitaId == id);
            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }

        // POST: Citas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita != null)
            {
                _context.Citas.Remove(cita);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.CitaId == id);
        }
    }
}
