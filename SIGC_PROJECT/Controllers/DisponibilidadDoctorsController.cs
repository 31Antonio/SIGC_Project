using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using SIGC_PROJECT.Helper;
using SIGC_PROJECT.Models;
using SIGC_PROJECT.Models.ViewModel;
using static System.Collections.Specialized.BitVector32;

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
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Index()
        {
            // Obtener el id del Usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var idDoctor = await _context.Doctors.Where(d => d.IdUsuario == int.Parse(idUser)).Select(d => d.DoctorId).FirstOrDefaultAsync();

            if(idDoctor == null)
            {
                return NotFound();
            }

            var disponibilidad = await _context.DisponibilidadDoctors.Where(d => d.DoctorId == idDoctor).ToListAsync();
            ViewBag.DoctorId = idDoctor;

            return View(disponibilidad);
        }

        //GET: Ver las disponibilidades
        [Authorize(Roles = "Paciente,Secretaria")]
        [FiltroRegistro]
        [HttpGet]
        public IActionResult VerDisponibilidad()
        {
            //Obtener el id del Usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //Obtener el rol del Usuario Autenticado
            var roles = _context.UsuarioRols.Where(r => r.IdUsuario == int.Parse(idUser)).Select(r => r.Rol.Nombre).FirstOrDefault();

            //Obtener el id de la secretaria
            var idSecretaria = _context.Secretaria.Where(s => s.IdUsuario == int.Parse(idUser)).Select(s => s.SecretariaId).FirstOrDefault();

            var query = _context.DisponibilidadDoctors.Include(d => d.Doctor).AsQueryable();

            if (roles == "Secretaria")
            {
                query = query.Where(d => d.Doctor.Secretaria.Any(s => s.SecretariaId == idSecretaria));
            }

            if (roles == "Paciente")
            {
                var especialidad = Request.Query["especialidad"];
                if (!string.IsNullOrEmpty(especialidad))
                {
                    query = query.Where(d => d.Doctor.Especialidad.Contains(especialidad));
                }
            }

            var disponibilidades = query.ToList();

            var model = disponibilidades.GroupBy(d => d.Doctor).Select(dp => new Doctor_DisponibilidadVM
            {
                idDoctor = dp.Key.DoctorId,
                Nombre = dp.Key.Nombre,
                Apellido = dp.Key.Apellido,
                Especialidad = dp.Key.Especialidad,
                Disponibilidades = dp.Select(d => new DisponibilidadDoctor
                {
                    Dia = d.Dia,
                    HoraInicio = FormatearHora12Horas(d.HoraInicio),
                    HoraFin = FormatearHora12Horas(d.HoraFin)
                }).ToList()
            }).ToList();

            ViewBag.Rol = roles;
            return View(model);
        }

        [HttpGet]
        public IActionResult BuscarPorEspecialidad(string especialidad)
        {
            // Verificar si la especialidad está vacía o nula
            var disponibilidades = string.IsNullOrWhiteSpace(especialidad)
                ? _context.DisponibilidadDoctors.Include(d => d.Doctor).ToList()
                : _context.DisponibilidadDoctors
                    .Include(d => d.Doctor)
                    .Where(d => d.Doctor.Especialidad.Contains(especialidad))
                    .ToList();

            var model = disponibilidades
                .GroupBy(d => d.Doctor)
                .Select(g => new Doctor_DisponibilidadVM
                {
                    idDoctor = g.Key.DoctorId,
                    Nombre = g.Key.Nombre,
                    Apellido = g.Key.Apellido,
                    Especialidad = g.Key.Especialidad,
                    Disponibilidades = g.Select(d => new DisponibilidadDoctor
                    {
                        Dia = d.Dia,
                        HoraInicio = d.HoraInicio,
                        HoraFin = d.HoraFin
                    }).ToList()
                })
                .ToList();

            return Json(model);
        }

        public static string FormatearHora12Horas(string hora24)
        {
            if (DateTime.TryParse(hora24, out var hora))
            {
                return hora.ToString("h:mm tt");
            }
            return hora24;
        }

        [Authorize(Roles = "Doctor")]
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

        [Authorize(Roles = "Doctor")]
        // GET: DisponibilidadDoctors/Create
        public async Task<IActionResult> Create()
        {
            //Obtener el id del Usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var idDoctor = await _context.Doctors.Where(d => d.IdUsuario == int.Parse(idUser)).Select(d => d.DoctorId).FirstOrDefaultAsync();

            var model = new DisponibilidadDocVM
            {
                DoctorId = idDoctor
            };

            return View(model);
        }

        // POST: DisponibilidadDoctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DisponibilidadDocVM model)
        {
            if (ModelState.IsValid)
            {
                foreach(var dia in model.Dias.Where(d => Request.Form["selectedDays"].Contains(d.Key)))
                {
                    var horaInicio = Request.Form[$"HoraInicio_{dia.Key}"];
                    var horaFin = Request.Form[$"HoraFin_{dia.Key}"];

                    //Verificar si ya existe una disponibilidad para un dia especifico
                    var existe = _context.DisponibilidadDoctors.Any(d => d.DoctorId == model.DoctorId && d.Dia == dia.Key);
                    if (!existe)
                    {
                        var disponibilidad = new DisponibilidadDoctor
                        {
                            DoctorId = model.DoctorId,
                            Dia = dia.Key,
                            HoraInicio = horaInicio,
                            HoraFin = horaFin
                        };

                        _context.DisponibilidadDoctors.Add(disponibilidad);
                        _context.SaveChanges();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["Mensaje"] = "Ya tiene un este día registrado, puede actualizarlo";
                    }

                }

            }

            return View(model);
        }

        // GET: DisponibilidadDoctors/Edit/5
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Edit()
        {
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var doctorId = await _context.Doctors.Where(d => d.IdUsuario == int.Parse(idUser)).Select(d => d.DoctorId).FirstOrDefaultAsync();
            var disponibilidades = await _context.DisponibilidadDoctors.Where(d => d.DoctorId == doctorId).ToListAsync();

            var model = new DisponibilidadDocVM
            {
                DoctorId = doctorId
            };

            foreach (var disponibilidad in disponibilidades)
            {
                if (model.Dias.ContainsKey(disponibilidad.Dia))
                {
                    model.Dias[disponibilidad.Dia] = true;
                    model.HorasInicio[disponibilidad.Dia] = disponibilidad.HoraInicio;
                    model.HorasFin[disponibilidad.Dia] = disponibilidad.HoraFin;
                }
            }

            return View(model);
        }

        // POST: DisponibilidadDoctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DisponibilidadDocVM model)
        {
            if (ModelState.IsValid)
            {
                var disponibilidadesExistentes = _context.DisponibilidadDoctors.Where(d => d.DoctorId == model.DoctorId).ToList();

                //Actualizar o eliminar disponibilidades existentes
                foreach(var disponibilidad in disponibilidadesExistentes)
                {
                    if(model.Dias.ContainsKey(disponibilidad.Dia) && model.Dias[disponibilidad.Dia])
                    {
                        //Actualizar la disponibilidad existente
                        disponibilidad.HoraInicio = model.HorasInicio.ContainsKey(disponibilidad.Dia) ?
                                                    model.HorasInicio[disponibilidad.Dia] :
                                                    disponibilidad.HoraInicio;

                        disponibilidad.HoraFin = model.HorasFin.ContainsKey(disponibilidad.Dia) ?
                                                 model.HorasFin[disponibilidad.Dia] : disponibilidad.HoraFin;
                    }
                    else
                    {
                        //Eliminar la disponibilidad no seleccionada
                        _context.DisponibilidadDoctors.Remove(disponibilidad);
                    }
                }

                //Agregar nuevas disponibilidades
                foreach (var dia in model.Dias.Where(d => d.Value))
                {
                    if(!disponibilidadesExistentes.Any(d => d.Dia == dia.Key))
                    {
                        var nuevaDisponibilidad = new DisponibilidadDoctor
                        {
                            DoctorId = model.DoctorId,
                            Dia = dia.Key,
                            HoraInicio = model.HorasInicio.ContainsKey(dia.Key) ? model.HorasInicio[dia.Key] : null,
                            HoraFin = model.HorasFin.ContainsKey(dia.Key) ? model.HorasFin[dia.Key] : null
                        };

                        _context.DisponibilidadDoctors.Add(nuevaDisponibilidad);
                    }
                }
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: DisponibilidadDoctors/Delete/5
        [Authorize(Roles = "Doctor")]
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

