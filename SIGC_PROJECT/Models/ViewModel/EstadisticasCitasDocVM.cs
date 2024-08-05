namespace SIGC_PROJECT.Models.ViewModel
{
    public class EstadisticasCitasDocVM
    {
        public string Doctor { get; set; }
        public int TotalCitas { get; set; }
        public int Completados { get; set; }
        public int Cancelados { get; set; }
        public int Pendientes { get; set; }
        public int PacientesFemeninos { get; set; }
        public int PacientesMasculinos { get; set; }
        public int NumeroPacientesUnicos { get; set; }

        public List<CitasPorMesVM> CitasPorMes { get; set; }

        public List<CitasPorPacienteVM> citasPorPaciente { get; set; }
    }
}

public class CitasPorPacienteVM
{
    public string NombrePaciente { get; set; }
    public int CantidadCitas { get; set; }
}