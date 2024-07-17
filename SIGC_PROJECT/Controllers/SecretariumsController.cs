using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using SIGC_PROJECT.Helper;
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
            //Obtener el id del Usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var roles = await _context.UsuarioRols.Where(r => r.IdUsuario == int.Parse(idUser)).Select(r => r.Rol.Nombre).FirstOrDefaultAsync();

            List<Secretarium> secretaria;

            if (roles == "Doctor")
            {
                var idDoctor = await _context.Doctors.Where(d => d.IdUsuario == int.Parse(idUser)).Select(d => d.DoctorId).FirstOrDefaultAsync();
                secretaria = await _context.Secretaria.Where(s => s.IdDoctor == idDoctor).ToListAsync();

                return View(secretaria);
            }
            else if(roles == "Administrador")
            {
                secretaria = await _context.Secretaria.ToListAsync();
                return View(secretaria);
            }

            return View();
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

        #region ===== VISTAS DE CONFIGURACION =====

        [Authorize(Roles = "Secretaria")]
        public async Task<IActionResult> Configuracion()
        {
            return View();
        }

        [Authorize(Roles = "Secretaria")]
        public async Task<IActionResult> FormEdit()
        {
            //Obtener el id del usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var secretariaId = await _context.Secretaria.Where(s => s.IdUsuario == int.Parse(idUser))
                                                        .Select(s => s.SecretariaId).FirstOrDefaultAsync();

            var model = await _context.Secretaria.FirstOrDefaultAsync(s => s.SecretariaId == secretariaId);

            if (model == null)
            {
                return NotFound();
            }

            return PartialView("_FormEditPartialSecretaria", model);
        }

        [Authorize(Roles = "Secretaria")]
        public IActionResult FormPassword()
        {
            return PartialView("_CuentaUsuario");
        }
        #endregion ================================


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
        [Authorize(Roles = "Doctor")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Secretariums/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("SecretariaId,Cedula,Nombre,Apellido,Telefono,CorreoElectronico,HorarioTrabajo,IdUsuario,IdDoctor")]*/ SecretariaVM jsonData)
        {
            var Usuario = jsonData.Usuario;
            var rol = jsonData.Rol;
            var Secretaria = jsonData.Secretaria;
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var idDoctor = await _context.Doctors.Where(d => d.IdUsuario == int.Parse(idUser)).Select(d => d.DoctorId).FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(Secretaria.Cedula) || string.IsNullOrEmpty(Secretaria.Nombre) || string.IsNullOrEmpty(Secretaria.Apellido))
            {
                TempData["MensajeError"] = "Debe llenar los datos de la secretaria antes de crear el registro";
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
                        if (Usuario.Contrasena.Length < 5)
                        {
                            TempData["MensajeError"] = "La contraseña debe tener 5 o más caracteres.";
                        }
                        else
                        {
                            var cedula = await _context.Secretaria.Where(d => d.Cedula == Secretaria.Cedula).SingleOrDefaultAsync();

                            if (cedula != null)
                            {
                                TempData["MensajeError"] = "Ya hay una secretaria registrada con este número de cédula";
                            }
                            else
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

                                var nuevaSecretaria = new Secretarium()
                                {
                                    Cedula = Secretaria.Cedula,
                                    Nombre = Secretaria.Nombre,
                                    Apellido = Secretaria.Apellido,
                                    Telefono = Secretaria.Telefono,
                                    CorreoElectronico = Secretaria.CorreoElectronico,
                                    HorarioTrabajo = Secretaria.HorarioTrabajo,
                                    IdUsuario = nuevoUsuario.IdUsuario,
                                    IdDoctor = idDoctor
                                };

                                _context.Secretaria.Add(nuevaSecretaria);
                                await _context.SaveChangesAsync();

                                return RedirectToAction("Details", "Secretariums", new { id = nuevaSecretaria.SecretariaId });
                            }
                        }
                    }
                }
            }
            return View(jsonData);
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
        public async Task<IActionResult> Edit(int id, [Bind("SecretariaId,Cedula,Nombre,Apellido,Telefono,CorreoElectronico,HorarioTrabajo,IdUsuario,IdDoctor")] Secretarium secretarium)
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

                if (User.IsInRole("Administrador"))
                {
                    return RedirectToAction(nameof(Index));
                }
                else if (User.IsInRole("Secretaria"))
                {
                    return RedirectToAction("Configuracion");
                }

            }
            return View(secretarium);
        }

        // GET: Secretariums/Delete/5
        [Authorize(Roles = "Doctor")]
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
