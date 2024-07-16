using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SIGC_PROJECT.Models;
using SIGC_PROJECT.Models.ViewModel;
using SIGC_PROJECT.Helper;
using System.Data;
using System.Security.Claims;

namespace SIGC_PROJECT.Controllers
{
    public class RegistrosUsuario : Controller
    {
        private readonly SigcProjectContext _context;

        public RegistrosUsuario(SigcProjectContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Registrarse()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RegistroUsuario()
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

            if (rolActual == null)
            {
                return Forbid();
            }

            //Filtrar los datos basados en el Rol actual
            List<Rol> roles;

            if (rolActual == "Administrador") 
            { 
                roles = _context.Rols.Where(r => r.Nombre == "Paciente" || r.Nombre == "Doctor").ToList();
                ViewBag.Roles = roles;
            }

            return View();
        }

        //METODO PARA REGISTRAR USUARIOS GENERALES

        [HttpPost]
        public async Task<IActionResult> Registrar(AdminRegistro model) 
        {
            var Usuario = model.Usuario;
            var Doctor = model.Doctor;
            var Paciente = model.Paciente;
            var rol = model.Rol;

            var result = await _context.Usuarios.Where(u => u.NombreUsuario == Usuario.NombreUsuario).SingleOrDefaultAsync();

            if (result != null)
            {

                TempData["MensajeError"] = "El nombre de usuario ya existe, ingrese otro.";
                return RedirectToAction("RegistroUsuario");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    return BadRequest(ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage)).ToList());
                }
                else
                {
                    if(rol == 2)
                    {
                        var cedula = await _context.Doctors.Where(d => d.Cedula == Doctor.Cedula).SingleOrDefaultAsync();

                        if (cedula != null)
                        {
                            TempData["MensajeError"] = "Ya hay un doctor registrado con este número de cédula";
                            return RedirectToAction("RegistroUsuario");
                        }
                        else if(cedula == null)
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

                            return RedirectToAction("Index", "Doctors");
                        }
                    } 
                    else if (rol == 4)
                    {
                        var cedulaP = await _context.Pacientes.Where(p => p.Cedula == Paciente.Cedula).SingleOrDefaultAsync();
                        
                        if(cedulaP != null)
                        {
                            TempData["MensajeError"] = "Ya hay un paciente registrado con este número de cédula";
                            return RedirectToAction("RegistroUsuario");
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

                            var nuevoPaciente = new Paciente()
                            {
                                Cedula = Paciente.Cedula,
                                Nombre = Paciente.Nombre,
                                Apellido = Paciente.Apellido,
                                FechaNacimiento = Paciente.FechaNacimiento,
                                Edad = Paciente.Edad,
                                Genero = Paciente.Genero,
                                Direccion = Paciente.Direccion,
                                Telefono = Paciente.Telefono,
                                CorreoElectronico = Paciente.CorreoElectronico,
                                IdUsuario = nuevoUsuario.IdUsuario
                            };

                            _context.Pacientes.Add(nuevoPaciente);
                            await _context.SaveChangesAsync();

                            return RedirectToAction("Index", "Pacientes");
                        }

                    }

                }
            }

            return Json(new { Mensaje = "Ha ocurrido un error" });
        }


        //METODO PARA EL ENVIO DE SOLICITUD DE REGISTRO DEL PACIENTE
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> EnviarSolicitudR(RegistrarPacienteVM pacienteVM)
        {

            if (pacienteVM.nombre.Length < 5)
            {
                return Json(new { success = false, message = "El Nombre de Usuario debe tener más de 5 caracteres" });
            } 
            else if (pacienteVM.contrasena.Length < 5)
            {
                return Json(new { success = false, message = "La contraseña debe tener más de 5 caracteres." });
            }
            
            var result = await _context.Usuarios.Where(u => u.NombreUsuario == pacienteVM.nombre).SingleOrDefaultAsync();
            var result2 = await _context.SolicitudesRegistros.Where(s => s.Nombre == pacienteVM.nombre).SingleOrDefaultAsync();

            if (result != null || result2 != null)
            {
                return Json(new { success = false, message = "El Nombre de Usuario ya existe o ha sido solicitado." });
            }
            else
            {
                if (ModelState.IsValid)
                {
                    //Generar el Hash y el Salt de la clave
                    HashedPassword hashedPassword = HashHelper.Hash(pacienteVM.contrasena, pacienteVM.nombre);

                    //Obtener id de la secretaria
                    var secretariaId = ObtenerSecretaria();

                    //Crear una nueva solicitud de Registro
                    var nuevaSolicitud = new SolicitudesRegistro
                    {
                        Nombre = pacienteVM.nombre,
                        Clave = hashedPassword.Password,
                        FechaSolicitud = DateTime.Now,
                        IdSecretaria = secretariaId
                    };

                    _context.SolicitudesRegistros.Add(nuevaSolicitud);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Su solicitud se ha enviado con éxito. " });
                }

                return View("Registrarse", pacienteVM);
            }
        }

        //OBTENER LA SECRETARIA PARA EL ENVIO DE SOLICITUD
        private int ObtenerSecretaria()
        {
            var secretariaMS = _context.Usuarios.Where(u => u.UsuarioRoles.Any(r => r.IdRol == 3))
                                            .Select(u => new
                                            {
                                                u.IdUsuario,
                                                SolicitudesCount = _context.SolicitudesRegistros.Count(s => s.IdSecretaria == u.IdUsuario)
                                            })
                                            .OrderBy(u => u.SolicitudesCount)
                                            .ThenBy(u => u.IdUsuario) //En caso de que todas tengan la misma cantidad, tomar como referencia el Id
                                            .FirstOrDefault();

            if (secretariaMS == null)
            {
                throw new Exception("No hay Secretarias Disponibles");
            }

            return secretariaMS.IdUsuario;
            //var secretaria = _ctx.Secretarias.FirstOrDefault();
            //return secretaria != null ? secretaria.IdUsuario : -1;
        }


    }
}
