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
    public class SecretariumsController : Controller
    {
        private readonly SigcProjectContext _context;

        public SecretariumsController(SigcProjectContext context)
        {
            _context = context;
        }

        // GET: Secretariums
        public async Task<IActionResult> Index()
        {
            return View(await _context.Secretaria.ToListAsync());
        }

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

        // GET: Secretariums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Secretariums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
