using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using SIGC_PROJECT.Models;
using SIGC_PROJECT.Models.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        [Authorize(Roles = "Paciente, Secretaria")]
        public IActionResult Create(int idDoctor)
        {
            var doctor = _context.Doctors.Include(d => d.DisponibilidadDoctors)
                                         .FirstOrDefault(d => d.DoctorId == idDoctor);
            if(doctor == null)
            {
                return NotFound();
            }

            var model = new CrearCitasVM
            {
                DoctorId = doctor.DoctorId,
                NombreDoctor = doctor.Nombre + ' ' + doctor.Apellido,
                EspecialidadDoctor = doctor.Especialidad,
                Disponibilidades = doctor.DisponibilidadDoctors.Select( d => new DisponibilidadDoctor
                {
                    Dia = d.Dia,
                    HoraInicio = d.HoraInicio,
                    HoraFin = d.HoraFin
                }).ToList()
            };

            return View(model);
        }

        // POST: Citas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearCitasVM model)
        {
            if (ModelState.IsValid)
            {
                // Obtener el id del Usuario
                var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                //Obtener el rol del Usuario Autenticado
                var roles = _context.UsuarioRols.Where(r => r.IdUsuario == int.Parse(idUser)).Select(r => r.Rol.Nombre).FirstOrDefault();

                //Obtener el id de la secretaria
                var idSecretaria = await _context.Secretaria.Where(s => s.IdUsuario == int.Parse(idUser)).Select(s => s.SecretariaId).FirstOrDefaultAsync();

                //Obtener el id del paciente
                var idPaciente = await _context.Pacientes.Where(p => p.IdUsuario == int.Parse(idUser)).Select(p => p.PacienteId).FirstOrDefaultAsync();

                var cita = new Cita
                {
                    DoctorId = model.DoctorId,
                    NombreDoctor = model.NombreDoctor,
                    EspecialidadDoctor = model.EspecialidadDoctor,
                    FechaCita = model.FechaCita,
                    HoraCita = model.HoraCita,
                    Comentario = model.Comentario,
                    Estado = "PENDIENTE"
                };

                if (roles == "Paciente")
                {
                    var nombrePaciente = await _context.Pacientes.Where(p => p.PacienteId == idPaciente).Select(p => p.Nombre).FirstOrDefaultAsync();
                    var apellidoPaciente = await _context.Pacientes.Where(p => p.PacienteId == idPaciente).Select(p => p.Apellido).FirstOrDefaultAsync();
                    cita.PacienteId = idPaciente;
                    cita.NombrePaciente = nombrePaciente + ' ' + apellidoPaciente;
                    cita.SecretariaId = null;
                }
                else if(roles == "Secretaria")
                {
                    cita.PacienteId = null;
                    cita.SecretariaId = idSecretaria;
                    cita.NombrePaciente = model.NombrePaciente;
                }

                _context.Citas.Add(cita);
                await _context.SaveChangesAsync();

                if(roles == "Paciente")
                {
                    return RedirectToAction("CitaPaciente");
                }
                else if(roles == "Secretaria")
                {
                    return RedirectToAction("CitasDelDia");
                }
                
            }

            return View(model);
        }

        //Obtener la disponibilidad del doctor
        [HttpPost]
        public IActionResult ObtenerDisponibilidadDoctor([FromBody] DisponibilidadRequest disponibilidad)
        {
            var disponibilidades = _context.DisponibilidadDoctors.Where(d => d.DoctorId == disponibilidad.DoctorId && d.Dia == disponibilidad.Dia)
                                                                 .Select(d => new
                                                                 {
                                                                     d.HoraInicio,
                                                                     d.HoraFin
                                                                 }).FirstOrDefault();

            return Json(disponibilidades);
        }

        [HttpPost]
        public IActionResult ObtenerHorasOcupadas([FromBody] HorasOcupadas horas_Ocupadas)
        {
            var horasOcupadas = _context.Citas.Where(c => c.DoctorId == horas_Ocupadas.DoctorId && c.FechaCita == horas_Ocupadas.Fecha && (c.Estado == "PENDIENTE" || c.Estado == "COMPLETADO"))
                                              .Select(c => c.HoraCita).ToList();

            return Json(horasOcupadas);
        }

        [Authorize(Roles = "Paciente")]
        public async Task<IActionResult> CitaPaciente()
        {
            //Obtener el id del usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //Obtener el id del paciente
            var pacienteId = await _context.Pacientes.Where(p => p.IdUsuario == int.Parse(idUser)).Select(p => p.PacienteId).FirstOrDefaultAsync();

            var citasPendientes = await _context.Citas.Include(d => d.Doctor)
                                                      .Where(c => c.PacienteId == pacienteId && c.Estado == "PENDIENTE")
                                                      .ToListAsync();

            var citasCompletadas = await _context.Citas.Include(d => d.Doctor)
                                                       .Where(c => c.PacienteId == pacienteId && (c.Estado == "COMPLETADO" || c.Estado == "CANCELADO"))
                                                       .ToListAsync();
            
            var model = new CitasPacientesVM
            {
                CitasPendientes = citasPendientes,
                CitasCompletadas = citasCompletadas
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CancelarCita(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if(cita != null && cita.Estado == "PENDIENTE")
            {
                cita.Estado = "CANCELADO";
                await _context.SaveChangesAsync();
            }

            if (User.IsInRole("Paciente"))
            {
                return RedirectToAction("CitaPaciente");
            }
            else if(User.IsInRole("Secretaria") || User.IsInRole("Doctor"))
            {
                return RedirectToAction("CalendarioCitas");
            }

            return View(cita);
            
        }
        
        [HttpGet]
        public async Task<IActionResult> CompletarCita(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if(cita != null && cita.Estado == "PENDIENTE")
            {
                cita.Estado = "COMPLETADO";
                await _context.SaveChangesAsync();
            }

            if (User.IsInRole("Paciente"))
            {
                return RedirectToAction("CitaPaciente");
            }
            else if (User.IsInRole("Secretaria") || User.IsInRole("Doctor"))
            {
                return RedirectToAction("CalendarioCitas");
            }

            return View(cita);
        }

        //==============================================================//

        [Authorize(Roles = "Doctor,Secretaria")]
        public IActionResult CalendarioCitas()
        {
            //Obtener el id del usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //Obtener el rol del usuario 
            var rolActual = _context.UsuarioRols.Where(r => r.IdUsuario == int.Parse(idUser))
                                                .Select(r => r.Rol.Nombre).FirstOrDefault();

            if(rolActual == "Doctor")
            {
                var doctorId = _context.Doctors.Where(d => d.IdUsuario == int.Parse(idUser))
                                               .Select(d => d.DoctorId).FirstOrDefault();

                ViewBag.DoctorId = doctorId;
            }
            else if (rolActual == "Secretaria")
            {
                var doctorId = _context.Secretaria.Where(s => s.IdUsuario == int.Parse(idUser))
                                                  .Select(d => d.IdDoctor).FirstOrDefault();

                ViewBag.DoctorId = doctorId;
            }


            return View();
        }

        [Authorize(Roles = "Doctor,Secretaria")]
        public IActionResult CitasDelDia()
        {
            return View();
        }
        
        [Authorize(Roles = "Doctor,Secretaria")]
        public IActionResult CitasAnteriores()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Dia([FromBody] CalendarioSolicitudes request)
        {
            DateTime fechaSeleccionada = DateTime.Parse(request.Date);
            var doctorId = request.DoctorId;

            var citas = _context.Citas.Include(c => c.Doctor)
                                      .Where(c => c.FechaCita == fechaSeleccionada.Date && c.DoctorId == doctorId)
                                      .ToList();

            var model = new CitasDelDiaVM
            {
                Fecha = fechaSeleccionada,
                Citas = citas
            };

            if(fechaSeleccionada < DateTime.Now.Date)
            {
                //Redirigir a la vista de tablas
                var redirectUrl = Url.Action("CitasAnteriores", "Citas", new { date = fechaSeleccionada.ToString("yyyy-MM-dd"), doctorId = request.DoctorId });
                return Json(new { redirectUrl });
            }
            else
            {
                //Redirigir a la vista de citas del dia
                var redirectUrl = Url.Action("CitasDelDia", "Citas", new { date = fechaSeleccionada.ToString("yyyy-MM-dd"), doctorId = request.DoctorId });
                return Json(new { redirectUrl });
            }
        }

        [HttpGet]
        public IActionResult CitasDelDia(DateTime date, int doctorId)
        {
            var citas = _context.Citas.Include(c => c.Doctor)
                                      .Where(c => c.DoctorId == doctorId && c.FechaCita == date.Date && c.Estado == "PENDIENTE")
                                      .ToList();

            var model = new CitasDelDiaVM
            {
                Fecha = date,
                Citas = citas
            };

            return View(model);
        }
        
        [HttpGet]
        public IActionResult CitasAnteriores(DateTime date, int doctorId)
        {
            var citas = _context.Citas.Include(c => c.Doctor)
                                      .Where(c => c.DoctorId == doctorId && c.FechaCita == date.Date)
                                      .ToList();

            var model = new CitasDelDiaVM
            {
                Fecha = date,
                Citas = citas
            };

            return View(model);
        }
        //======================

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

public class DisponibilidadRequest
{
    public int DoctorId { get; set; }
    public string Dia { get; set; }
}
public class HorasOcupadas
{
    public int DoctorId { get; set; }
    public DateTime Fecha { get; set; }
}

public class CalendarioSolicitudes
{
    public string Date { get; set; }
    public int DoctorId { get; set; }
}
