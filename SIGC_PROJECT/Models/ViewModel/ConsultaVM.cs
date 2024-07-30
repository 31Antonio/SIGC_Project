namespace SIGC_PROJECT.Models.ViewModel
{
    public class ConsultaVM
    {
        public int PacienteId { get; set; }
        public int DoctorId { get; set; }
        public DateTime FechaConsulta { get; set; }
        public string MotivoConsulta { get; set; }
        public string Diagnostico { get; set; }
        public string Tratamiento { get; set; }
        public int? RecetaId { get; set; }
        public string Observaciones { get; set; }

    }
}
