using Microsoft.AspNetCore.Mvc;
using SIGC_PROJECT.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Bouncycastleconnector;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using Microsoft.AspNetCore.Authorization;
using SIGC_PROJECT.Models.ViewModel;
using System.Security.Claims;
using Syncfusion.EJ2.Linq;
using X.PagedList.Extensions;

namespace SIGC_PROJECT.Controllers
{
    public class ReportesController : Controller
    {
        private readonly SigcProjectContext _context;
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportesController(
            SigcProjectContext context,
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _webHostEnvironment = webHostEnvironment;
        }

        #region REPORTE DE LAS CITAS (PDF)
        public async Task<IActionResult> CitasReporte(string searchString, string estado, int? pagina, int cantidadP = 10)
        {
            // Obtener el id del usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var citas = _context.Citas.AsQueryable();

            // Segregar citas según el rol del usuario
            if (User.IsInRole("Doctor"))
            {
                var doctorId = await _context.Doctors.Where(d => d.IdUsuario == int.Parse(idUser))
                                                     .Select(d => d.DoctorId).FirstOrDefaultAsync();
                citas = citas.Where(c => c.DoctorId == doctorId);
            }
            else if (User.IsInRole("Secretaria"))
            {
                var doctorId = await _context.Secretaria.Where(s => s.IdUsuario == int.Parse(idUser))
                                                        .Select(s => s.IdDoctor).FirstOrDefaultAsync();
                citas = citas.Where(c => c.DoctorId == doctorId);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                citas = citas.Where(c =>
                    c.Paciente.Nombre.Contains(searchString) ||
                    c.Paciente.Apellido.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(estado))
            {
                citas = citas.Where(c => c.Estado == estado);
            }

            //Establecer el numero de pagina 
            int numeroP = pagina ?? 1;

            var citasList = citas.Include(c => c.Paciente).ToPagedList(numeroP, cantidadP);

            ViewData["Estado"] = estado;

            return View(citasList);
        }

        public async Task<IActionResult> DescargarCitasPDF(string searchString, string estado)
        {
            // Obtener el id del usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var citas = _context.Citas.AsQueryable();

            // Segregar citas según el rol del usuario
            if (User.IsInRole("Doctor"))
            {
                var doctorId = await _context.Doctors.Where(d => d.IdUsuario == int.Parse(idUser))
                                                     .Select(d => d.DoctorId).FirstOrDefaultAsync();
                citas = citas.Where(c => c.DoctorId == doctorId);
            }
            else if (User.IsInRole("Secretaria"))
            {
                var doctorId = await _context.Secretaria.Where(s => s.IdUsuario == int.Parse(idUser))
                                                        .Select(s => s.IdDoctor).FirstOrDefaultAsync();
                citas = citas.Where(c => c.DoctorId == doctorId);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                citas = citas.Where(c =>
                    c.Paciente.Nombre.Contains(searchString) ||
                    c.Paciente.Apellido.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(estado))
            {
                citas = citas.Where(c => c.Estado == estado);
            }

            var citasList = await citas
                .Include(c => c.Paciente)
                .ToListAsync();

            var htmlContent = RenderHtmlContent(citasList);

            using (var stream = new MemoryStream())
            {
                var pdfWriter = new PdfWriter(stream);
                var pdfDocument = new PdfDocument(pdfWriter);
                var document = new Document(pdfDocument);

                // Add the HTML content to the PDF
                var converterProperties = new ConverterProperties();
                HtmlConverter.ConvertToPdf(htmlContent, pdfDocument, converterProperties);

                // Make sure to close the document
                document.Close();
                var detalleCitas = "DetallesCitas" + DateTime.Now + ".pdf";

                return File(stream.ToArray(), "application/pdf", detalleCitas);
            }
        }

        private string RenderHtmlContent(IEnumerable<Cita> citasList)
        {
            var fechaActual = DateTime.Now.ToString("dd/MM/yyyy");
            var htmlContent = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8' />
                <title>Reporte de Citas</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        margin: 20px;
                    }}
                    header {{
                        text-align: center;
                        margin-bottom: 20px;
                    }}
                    header h2 {{
                        margin: 0;
                    }}
                    header p {{
                        margin: 5px 0 0;
                        color: #777;
                    }}
                    h1 {{
                        text-align: center;
                        color: #333;
                    }}
                    table {{
                        width: 100%;
                        border-collapse: collapse;
                        margin: 20px 0;
                        font-size: 16px;
                        text-align: left;
                    }}
                    th, td {{
                        padding: 12px;
                        border: 1px solid #ddd;
                    }}
                    th {{
                        background-color: rgba(5, 12, 67, 1);
                        color: white;
                        text-transform: uppercase;
                        letter-spacing: 1px;
                    }}
                    tr:nth-child(even) {{
                        background-color: #f9f9f9;
                    }}
                    tr:hover {{
                        background-color: #f1f1f1;
                    }}
                    footer {{
                        text-align: center;
                        margin-top: 20px;
                        font-size: 14px;
                        color: #777;
                    }}
                </style>
        </head>
        <body>
            <header>
                <h2>Centro Policlínico Nacional</h2>
                <p>Reporte de Citas - {fechaActual}</p>
            </header>

            <table>
                <thead>
                    <tr>
                        <th>Paciente</th>
                        <th>Estado</th>
                        <th>Motivo</th>
                        <th>Fecha y Hora</th>
                    </tr>
                </thead>
                <tbody>";

                foreach (var cita in citasList)
                {
                    htmlContent += $@"
                    <tr>
                        <td>{cita.NombrePaciente}</td>
                        <td>{cita.Estado}</td>
                        <td>{cita.Comentario}</td>
                        <td>{cita.FechaCita.Value.ToString("dd/MM/yyyy")} - {cita.HoraCita}</td>
                    </tr>";
                }

                htmlContent += $@"
                        </tbody>
                    </table>
                    <footer>
                        <p>Centro Policlínico Nacional - Todos los derechos reservados {DateTime.Now.Year}</p>
                    </footer>
                </body>
                </html>";

            return htmlContent;
        }

        #endregion

        #region ESTADISTICAS ADMIN

        #region ESTADISTICAS DE LAS CITAS ADMIN
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EstadisticasCitas()
        {
            var citas = await _context.Citas.ToListAsync();

            var totalCitas = citas.Count;
            var completados = citas.Count(c => c.Estado == "COMPLETADO");
            var cancelados = citas.Count(c => c.Estado == "CANCELADO");
            var pendientes = citas.Count(c => c.Estado == "PENDIENTE");

            // Agrupar citas por especialidad
            var citasPorEspecialidad = citas.GroupBy(c => c.EspecialidadDoctor)
                                            .Select(e => new CitasPorEspecialidadVM
                                            {
                                                Especialidad = e.Key,
                                                Total = e.Count()
                                            }).ToList();

            // Agrupar citas por doctor
            var citasPorDoctor = citas.GroupBy(c => c.NombreDoctor)
                                      .Select(d => new CitasPorDoctorVM 
                                      { 
                                          Doctor = d.Key, Total = d.Count()
                                      }).ToList();

            // Agrupar citas por mes
            var citasPorMes = citas.GroupBy(c => c.FechaCita.Value.ToString("MMMM yyyy"))
                                   .Select(g => new CitasPorMesVM
                                   {
                                       Mes = g.Key,
                                       Total = g.Count()
                                   }).ToList();

            var model = new EstadisticasCitasVM
            {
                TotalCitas = totalCitas,
                Completados = completados,
                Cancelados = cancelados,
                Pendientes = pendientes,
                CitasPorEspecialidad = citasPorEspecialidad,
                CitasPorDoctor = citasPorDoctor,
                CitasPorMes = citasPorMes
            };

            return View(model);
        }
        #endregion

        #region ESTADISTICAS DOCTORES
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EstadisticasDoctores()
        {
            var doctores = await _context.Doctors.ToListAsync();

            var totalDoctores = doctores.Count();

            var doctoresPorEspecialidad = doctores.GroupBy(d => d.Especialidad)
                                                  .Select(e => new EspecialidadDoctorVM
                                                  {
                                                      Especialidad = e.Key,
                                                      TotalDoctores = e.Count()
                                                  }).ToList();

            var model = new EstadisticasDoctoresVM
            {
                TotalDoctores = totalDoctores,
                DoctoresPorEspecialidad = doctoresPorEspecialidad
            };

            return View(model);
        }
        #endregion

        #region ESTADISTICAS USUARIOS
        public async Task<IActionResult> EstadisticasUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            var totalUsuarios = usuarios.Count(); 

            //Agrupar Usuarios por Roles
            var usuariosPorRoles = await _context.UsuarioRols.GroupBy(ur => ur.Rol.Nombre)
                                                       .Select(r => new UsuariosPorRolVM
                                                       {
                                                           Rol = r.Key,
                                                           CantidadUsuarios = r.Count()
                                                       }).ToListAsync();

            //Agrupar por ultimo acceso
            var hoy = DateTime.Today;

            var usuariosHoy = await _context.UsuarioRols.Include(ur => ur.Usuario).Include(ur => ur.Rol)
                                                        .Where(ur => ur.Usuario.FechaUltimoAcceso.HasValue &&
                                                               ur.Usuario.FechaUltimoAcceso.Value.Date == hoy)
                                                        .Select(ur => new UsuarioConRolVM
                                                        {
                                                            Usuario = ur.Usuario,
                                                            Rol = ur.Rol.Nombre
                                                        }).ToListAsync();

            var rolesHoy = usuariosHoy.GroupBy(u => u.Rol).Select(r => new RolCantidadVM
                                                          {
                                                            Rol = r.Key,
                                                            Cantidad = r.Count()
                                                          }).ToList();

            var usuariosAcceso = new UsuariosAccesoVM
            {
                UsuariosHoy = usuariosHoy,
                CantidadUsuariosHoy = usuariosHoy.Count(),
                RolesHoy = rolesHoy
            };

            var model = new EstadisticasUsuariosVM
            {
                TotalUsuarios = totalUsuarios,
                UsuariosPorRol = usuariosPorRoles,
                UsuariosAcceso = usuariosAcceso
            };

            return View(model);
        }
        #endregion

        #endregion

        #region ESTADISTICAS DE LAS CITAS DOCTOR - SECRETARIA
        [Authorize(Roles = "Doctor,Secretaria")]
        public async Task<IActionResult> EstadisticasCitasD()
        {
            // Obtener el id del usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? doctorId = null;

            if (User.IsInRole("Doctor"))
            {
                doctorId = await _context.Doctors.Where(d => d.IdUsuario == int.Parse(idUser))
                                                 .Select(d => d.DoctorId).FirstOrDefaultAsync();
            }
            else if (User.IsInRole("Secretaria"))
            {
                doctorId = await _context.Secretaria.Where(s => s.IdUsuario == int.Parse(idUser))
                                                    .Select(s => s.IdDoctor).FirstOrDefaultAsync();
            }

            if (doctorId == null)
            {
                return NotFound();
            }

            var estadisticasPorDoctor = await ObtenerEstadisticasPorDoctor(doctorId.Value);

            if (estadisticasPorDoctor == null)
            {
                return NotFound();
            }

            return View(estadisticasPorDoctor);
        }

        private async Task<EstadisticasCitasDocVM> ObtenerEstadisticasPorDoctor(int doctorId)
        {
            // Obtener las citas
            var citas = await _context.Citas.Where(c => c.DoctorId == doctorId).Include(c => c.Paciente).ToListAsync();

            var doctor = await _context.Doctors.FindAsync(doctorId);

            if (doctor == null)
            {
                return null;
            }

            //Agrupar por pacientes
            var pacientesUnicos = await _context.Citas.Where(c => c.DoctorId == doctorId).GroupBy(c => c.Paciente)
                                                      .Select(p => new
                                                      {
                                                         Paciente = p.Key,
                                                         CantidadCitas = p.Count()
                                                      }).ToListAsync();

            var estadisticasPorDoctor = new EstadisticasCitasDocVM
            {
                Doctor = doctor.Nombre + ' ' + doctor.Apellido,
                TotalCitas = citas.Count(),
                Completados = citas.Count(c => c.Estado == "COMPLETADO"),
                Pendientes = citas.Count(c => c.Estado == "PENDIENTE"),
                Cancelados = citas.Count(c => c.Estado == "CANCELADO"),
                PacientesFemeninos = pacientesUnicos.Count(c => c.Paciente?.Genero == "Femenino"),
                PacientesMasculinos = pacientesUnicos.Count(c => c.Paciente?.Genero == "Masculino"),
                CitasPorMes = citas.GroupBy(c => c.FechaCita.Value.ToString("MMMM yyyy"))
                                   .Select(m => new CitasPorMesVM
                                   {
                                       Mes = m.Key,
                                       Total = m.Count()
                                   }).ToList(),
                NumeroPacientesUnicos = pacientesUnicos.Count(),
                citasPorPaciente = pacientesUnicos.Select(p => new CitasPorPacienteVM
                                   {
                                       NombrePaciente = p.Paciente?.Nombre + ' ' + p.Paciente?.Apellido,
                                       CantidadCitas = p.CantidadCitas
                                   }).ToList()
                };

            return estadisticasPorDoctor;
        }
        #endregion

        #region ESTADISTICAS DE LAS CITAS PACIENTE
        [Authorize(Roles = "Paciente")]
        public async Task<IActionResult> EstadisticasCitasP()
        {
            // Obtener el id del usuario
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Obtener el id del Paciente
            var idPaciente = await _context.Pacientes.Where(p => p.IdUsuario == int.Parse(idUser)).Select(p => p.PacienteId).FirstOrDefaultAsync();

            var citas = await _context.Citas.Where(c => c.PacienteId == idPaciente).ToListAsync();

            var paciente = await _context.Pacientes.Where(p => p.PacienteId == idPaciente)
                                                   .Select(p => new { NombrePaciente = p.Nombre + ' ' + p.Apellido }).FirstOrDefaultAsync();

            var totalCitas = citas.Count;
            var completados = citas.Count(c => c.Estado == "COMPLETADO");
            var cancelados = citas.Count(c => c.Estado == "CANCELADO");
            var pendientes = citas.Count(c => c.Estado == "PENDIENTE");

            var citasPorDoctor = citas.GroupBy(c => c.NombreDoctor)
                                      .Select(d => new CitasPorDoctorVM
                                      {
                                          Doctor = d.Key,
                                          Total = d.Count()
                                      }).ToList();

            var citasPorEspecialidad = citas.GroupBy(c => c.EspecialidadDoctor)
                                            .Select(e => new CitasPorEspecialidadVM
                                            {
                                                Especialidad = e.Key,
                                                Total = e.Count()
                                            }).ToList();

            var citasPorMes = citas.GroupBy(c => c.FechaCita.Value.ToString("MMMM yyyy"))
                                   .Select(m => new CitasPorMesVM
                                   {
                                       Mes = m.Key,
                                       Total = m.Count()
                                   }).ToList();

            var model = new EstadisticasCitasPacVM
            {
                Paciente = paciente.NombrePaciente,
                TotalCitas = totalCitas,
                Completados = completados,
                Pendientes = pendientes,
                Cancelados = cancelados,
                CitasPorDoctor = citasPorDoctor,
                CitasPorEspecialidad = citasPorEspecialidad,
                CitasPorMes = citasPorMes

            };

            return View(model);
        }
        #endregion
    }
}
