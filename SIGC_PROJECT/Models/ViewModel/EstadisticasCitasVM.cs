namespace SIGC_PROJECT.Models.ViewModel
{
    public class EstadisticasCitasVM
    {
        public int TotalCitas { get; set; }
        public int Completados { get; set; }
        public int Cancelados { get; set; }
        public int Pendientes { get; set; }

        public List<CitasPorEspecialidadVM> CitasPorEspecialidad { get; set; }
        public List<CitasPorDoctorVM> CitasPorDoctor { get; set; }
        public List<CitasPorMesVM> CitasPorMes { get; set; }
    }
}

public class CitasPorEspecialidadVM
{
    public string Especialidad { get; set; }
    public int Total { get; set; }
}

public class CitasPorDoctorVM
{
    public string Doctor { get; set; }
    public int Total { get; set; }
}

public class CitasPorMesVM
{
    public string Mes { get; set; }
    public int Total { get; set; }
}