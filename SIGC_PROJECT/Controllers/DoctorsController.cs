using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGC_PROJECT.Helper;
using SIGC_PROJECT.Models;
using SIGC_PROJECT.Models.ViewModel;
using X.PagedList.Extensions;

namespace SIGC_PROJECT.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly SigcProjectContext _context;

        public DoctorsController(SigcProjectContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrador")]
        // GET: Doctors
        public async Task<IActionResult> Index(string? especialidad, int? pagina, int cantidadP = 10)
        {
            //Establecer el numero de pagina
            int numeroP = (pagina ?? 1);

            IQueryable<Doctor> query = _context.Doctors;

            if (!string.IsNullOrEmpty(especialidad))
            {
                query = query.Where(d => d.Especialidad.Contains(especialidad));
            }

            query = query.OrderBy(d => d.DoctorId);

            var doctores = query.ToPagedList(numeroP, cantidadP);
            ViewBag.Especialidad = especialidad;

            return View(doctores);
        }

        #region ===== VISTAS DE CONFIGURACION =====
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Configuracion()
        {
            return View();
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> FormEdit()
        {
            //Obtener el id del usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var doctorId = await _context.Doctors.Where(d => d.IdUsuario == int.Parse(idUser))
                                           .Select(d => d.DoctorId).FirstOrDefaultAsync();

            var model = await _context.Doctors.FirstOrDefaultAsync(p => p.DoctorId == doctorId);

            if(model == null)
            {
                return NotFound();
            }

            return PartialView("_FormEditPartialDoctor", model);
        }

        [Authorize(Roles = "Doctor")]
        public IActionResult FormPassword()
        {
            return PartialView("_CuentaUsuario");
        }

        #endregion ================================

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Secretaria)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctors/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("DoctorId,Cedula,Nombre,Apellido,Especialidad,NumeroExequatur,Telefono,CorreoElectronico,IdUsuario")] */ DoctorVM jsonData)
        {
            var Usuario = jsonData.Usuario;
            var Doctor = jsonData.Doctor;
            var rol = jsonData.Rol;

            if (string.IsNullOrEmpty(Doctor.Cedula) || string.IsNullOrEmpty(Doctor.Nombre) || string.IsNullOrEmpty(Doctor.Apellido)
            || string.IsNullOrEmpty(Doctor.Especialidad) || string.IsNullOrEmpty(Doctor.NumeroExequatur))
            {
                TempData["MensajeError"] = "Debe llenar los datos del doctor antes de crear el registro";
            }
            else
            {
                var result = await _context.Usuarios.Where(u => u.NombreUsuario == Usuario.NombreUsuario).SingleOrDefaultAsync();

                if (result != null)
                {
                    TempData["MensajeError"] = "El nombre de usuario ya existe, ingrese otro.";
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        if (Usuario.Contrasena == null || Usuario.Contrasena.Length < 5)
                        {
                            TempData["MensajeError"] = "La contraseña debe tener 5 o más caracteres.";
                        }
                        else
                        {
                            var cedula = await _context.Doctors.Where(d => d.Cedula == Doctor.Cedula).SingleOrDefaultAsync();

                            if (cedula != null)
                            {
                                TempData["MensajeError"] = "Ya hay un doctor registrado con este número de cédula";
                            }
                            else if (cedula == null)
                            {
                                var hash = HashHelper.Hash(Usuario.Contrasena, Usuario.NombreUsuario);

                                var nuevoUsuario = new Usuario()
                                {
                                    NombreUsuario = Usuario.NombreUsuario,
                                    Contrasena = hash.Password
                                };

                                _context.Usuarios.Add(nuevoUsuario);
                                await _context.SaveChangesAsync();

                                var userRole = new UsuarioRol()
                                {
                                    IdUsuario = nuevoUsuario.IdUsuario,
                                    IdRol = rol
                                };

                                _context.UsuarioRols.Add(userRole);
                                await _context.SaveChangesAsync();

                                var nuevoDoctor = new Doctor()
                                {
                                    Cedula = Doctor.Cedula,
                                    Nombre = Doctor.Nombre,
                                    Apellido = Doctor.Apellido,
                                    Especialidad = Doctor.Especialidad,
                                    NumeroExequatur = Doctor.NumeroExequatur,
                                    Telefono = Doctor.Telefono,
                                    CorreoElectronico = Doctor.CorreoElectronico,
                                    IdUsuario = nuevoUsuario.IdUsuario
                                };

                                _context.Doctors.Add(nuevoDoctor);
                                await _context.SaveChangesAsync();

                                return RedirectToAction("Details", "Doctors", new { id = nuevoDoctor.DoctorId });
                            }
                        }
                    }
                }
            }
            return View(jsonData);
        }

        // GET: Doctors/Edit/5
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            ViewBag.UserDoctorId = doctor.IdUsuario;
            //ViewData["SecretariaId"] = new SelectList(_context.Secretaria, "SecretariaId", "Nombre", doctor.SecretariaId);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,Cedula,Nombre,Apellido,Especialidad,NumeroExequatur,Telefono,CorreoElectronico,IdUsuario")] Doctor doctor)
        {
            if (id != doctor.DoctorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.DoctorId))
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
                else if (User.IsInRole("Doctor"))
                {
                    return RedirectToAction("Configuracion");
                }

            }
            //ViewData["SecretariaId"] = new SelectList(_context.Secretaria, "SecretariaId", "Nombre", doctor.SecretariaId);
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Secretaria)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                //Encontrar el usuario
                var usuario = await _context.Usuarios.FindAsync(doctor.IdUsuario);

                if (usuario != null)
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

                _context.Doctors.Remove(doctor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.DoctorId == id);
        }
    }
}
