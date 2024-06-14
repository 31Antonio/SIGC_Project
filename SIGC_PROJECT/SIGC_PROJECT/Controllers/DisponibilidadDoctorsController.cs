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
    public class DisponibilidadDoctorsController : Controller
    {
        private readonly SigcProjectContext _context;

        public DisponibilidadDoctorsController(SigcProjectContext context)
        {
            _context = context;
        }

        // GET: DisponibilidadDoctors
        public async Task<IActionResult> Index()
        {
            var sigcProjectContext = _context.DisponibilidadDoctors.Include(d => d.Doctor);
            return View(await sigcProjectContext.ToListAsync());
        }

        // GET: DisponibilidadDoctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disponibilidadDoctor = await _context.DisponibilidadDoctors
                .Include(d => d.Doctor)
                .FirstOrDefaultAsync(m => m.DisponibilidadDoctorId == id);
            if (disponibilidadDoctor == null)
            {
                return NotFound();
            }

            return View(disponibilidadDoctor);
        }

        // GET: DisponibilidadDoctors/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "DoctorId");
            return View();
        }

        // POST: DisponibilidadDoctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DisponibilidadDoctorId,DoctorId,Dia,HoraInicio,HoraFin")] DisponibilidadDoctor disponibilidadDoctor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(disponibilidadDoctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "DoctorId", disponibilidadDoctor.DoctorId);
            return View(disponibilidadDoctor);
        }

        // GET: DisponibilidadDoctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disponibilidadDoctor = await _context.DisponibilidadDoctors.FindAsync(id);
            if (disponibilidadDoctor == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "DoctorId", disponibilidadDoctor.DoctorId);
            return View(disponibilidadDoctor);
        }

        // POST: DisponibilidadDoctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DisponibilidadDoctorId,DoctorId,Dia,HoraInicio,HoraFin")] DisponibilidadDoctor disponibilidadDoctor)
        {
            if (id != disponibilidadDoctor.DisponibilidadDoctorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(disponibilidadDoctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisponibilidadDoctorExists(disponibilidadDoctor.DisponibilidadDoctorId))
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
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "DoctorId", disponibilidadDoctor.DoctorId);
            return View(disponibilidadDoctor);
        }

        // GET: DisponibilidadDoctors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disponibilidadDoctor = await _context.DisponibilidadDoctors
                .Include(d => d.Doctor)
                .FirstOrDefaultAsync(m => m.DisponibilidadDoctorId == id);
            if (disponibilidadDoctor == null)
            {
                return NotFound();
            }

            return View(disponibilidadDoctor);
        }

        // POST: DisponibilidadDoctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var disponibilidadDoctor = await _context.DisponibilidadDoctors.FindAsync(id);
            if (disponibilidadDoctor != null)
            {
                _context.DisponibilidadDoctors.Remove(disponibilidadDoctor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DisponibilidadDoctorExists(int id)
        {
            return _context.DisponibilidadDoctors.Any(e => e.DisponibilidadDoctorId == id);
        }
    }
}
