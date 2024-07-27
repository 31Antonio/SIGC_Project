namespace SIGC_PROJECT.Models.ViewModel
{
    public class CrearCitasVM
    {
        public int DoctorId { get; set; }
        public int? PacienteId { get; set; }
        public int? SecretariaId { get; set; }
        public string? Estado { get; set; }
        public string? Comentario { get; set; }
        public DateTime? FechaCita { get; set; }
        public string? HoraCita { get; set; }
        public string? NombrePaciente { get; set; }
        public string? NombreDoctor { get; set; }
        public string? EspecialidadDoctor { get; set; }
        public List<DisponibilidadDoctor> Disponibilidades { get; set; }

        public CrearCitasVM()
        {
            Disponibilidades = new List<DisponibilidadDoctor>();
        }

    }
}
