﻿using System;
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
        public async Task<IActionResult> Index(string? cedula, int? pagina, int cantidadP = 10)
        {
            int numeroP = (pagina ?? 1);

            IQueryable<Paciente> query = _context.Pacientes;

            if (!string.IsNullOrEmpty(cedula))
            {
                query = query.Where(p => p.Cedula.Contains(cedula));
            }

            query = query.OrderBy(p => p.PacienteId);

            var pacientes = query.ToPagedList(numeroP, cantidadP);
            ViewBag.Cedula = cedula;

            return View(pacientes);
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
        [Authorize(Roles = "Secretaria,Administrador")]
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
                //Encontrar el usuario
                var usuario = await _context.Usuarios.FindAsync(paciente.IdUsuario);

                if(usuario != null)
                {
                    //Encontrar y eliminar el rol del usuario
                    var userRol = await _context.UsuarioRols.Where(ur => ur.IdUsuario == usuario.IdUsuario).FirstOrDefaultAsync();

                    if (userRol != null)
                    {
                        _context.UsuarioRols.Remove(userRol);
                    }

                    //Eliminar el Usuario
                    _context.Usuarios.Remove(usuario);
                }

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
