namespace SIGC_PROJECT.Models.ViewModel
{
    public class ConsultaGeneralVM
    {
        public int ConsultaId { get; set; }
        public int PacienteId { get; set; }
        public string NombrePaciente { get; set; }
        public int? EdadPaciente { get; set; }
        public int DoctorId { get; set; }
        public string NombreDoctor { get; set; }
        public string EspecialidadDoctor { get; set; }
        public DateTime FechaConsulta { get; set; }
        public string MotivoConsulta { get; set; }
        public string Diagnostico { get; set; }
        public string Tratamiento { get; set; }
        public int? RecetaId { get; set; }
        public string Observaciones { get; set; }
    }
}
