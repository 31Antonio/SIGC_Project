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

        public async Task<IActionResult> RegistroUsuario()
        {
            var roles = _context.Rols.ToList();
            ViewBag.Roles = roles;
            return View();
        }

        //METODO PARA REGISTRAR USUARIOS GENERALES
        [BindProperty]
        public Usuario Usuarios { get; set; }

        [HttpPost]
        public async Task<IActionResult> Registrar(string nombre, string contrasena, int rol) 
        {
            var result = await _context.Usuarios.Where(u => u.NombreUsuario == Usuarios.NombreUsuario).SingleOrDefaultAsync();

            if (result != null)
            {
                return BadRequest(new JObject()
                {
                    { "StatusCode", 400 },
                    { "Mensaje", "El Usuario ya existe, por favor, seleccione otro." }
                });
            }
            else
            {
                if (ModelState.IsValid)
                {
                    return BadRequest(ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage)).ToList());
                }
                else
                {
                    var hash = HashHelper.Hash(contrasena);

                    var nuevoUsuario = new Usuario()
                    {
                        NombreUsuario = nombre,
                        Contrasena = hash.Password,
                        Salt = hash.Salt
                    };

                    _context.Usuarios.Add(nuevoUsuario);
                    await _context.SaveChangesAsync();

                    Usuarios.Contrasena = "";
                    Usuarios.Salt = "";

                    var userRole = new UsuarioRol()
                    {
                        IdUsuario = nuevoUsuario.IdUsuario,
                        IdRol = rol
                    };

                    _context.UsuarioRols.Add(userRole);
                    await _context.SaveChangesAsync();

                    return Created($"/Usuarios/{Usuarios.IdUsuario}", Usuarios);
                }
            }

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

            if (result != null)
            {
                return Json(new { success = false, message = "El Nombre de Usuario ya existe." });
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    //Generar el Hash y el Salt de la clave
                    HashedPassword hashedPassword = HashHelper.Hash(pacienteVM.contrasena);

                    //Obtener id de la secretaria
                    var secretariaId = ObtenerSecretaria();

                    //Crear una nueva solicitud de Registro
                    var nuevaSolicitud = new SolicitudesRegistro
                    {
                        Nombre = pacienteVM.nombre,
                        Clave = hashedPassword.Password,
                        Sal = hashedPassword.Salt,
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
