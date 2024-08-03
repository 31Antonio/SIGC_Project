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

        #region REPORTE DE LAS CITAS
        public async Task<IActionResult> CitasReporte(string searchString, string estado)
        {
            var citas = _context.Citas.AsQueryable();

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

            ViewData["Estado"] = estado;

            return View(citasList);
        }

        public async Task<IActionResult> DescargarCitasPDF(string searchString, string estado)
        {
            var citas = _context.Citas.AsQueryable();

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

                return File(stream.ToArray(), "application/pdf", "DetallesCitas.pdf");
            }
        }

        private string RenderHtmlContent(IEnumerable<Cita> citasList)
        {
            var htmlContent = @"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8' />
                <title>Reporte de Citas</title>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        margin: 20px;
                    }
                    h1 {
                        text-align: center;
                        color: #333;
                    }
                    table {
                        width: 100%;
                        border-collapse: collapse;
                        margin: 20px 0;
                        font-size: 16px;
                        text-align: left;
                    }
                    th, td {
                        padding: 12px;
                        border: 1px solid #ddd;
                    }
                    th {
                        background-color: #f4f4f4;
                        color: #333;
                    }
                    tr:nth-child(even) {
                        background-color: #f9f9f9;
                    }
                    tr:hover {
                        background-color: #f1f1f1;
                    }
                </style>
            </head>
            <body>
                <h1>Reporte de Citas</h1>
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

            htmlContent += @"
                    </tbody>
                </table>
            </body>
            </html>";

            return htmlContent;
        }

        #endregion

        #region ESTADISTICAS DE LAS CITAS
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
    }
}
