namespace SIGC_PROJECT.Models.ViewModel
{
    public class EstadisticasCitasPacVM
    {
        public string Paciente { get; set; }
        public int TotalCitas { get; set; }
        public int Completados { get; set; }
        public int Cancelados { get; set; }
        public int Pendientes { get; set; }
        public List<CitasPorDoctorVM> CitasPorDoctor { get; set; }
        public List<CitasPorEspecialidadVM> CitasPorEspecialidad { get; set; }
        public List<CitasPorMesVM> CitasPorMes { get; set; }
    }
}
