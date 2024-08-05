namespace SIGC_PROJECT.Models.ViewModel
{
    public class EstadisticasDoctoresVM
    {
        public int TotalDoctores { get; set; }
        public List<EspecialidadDoctorVM> DoctoresPorEspecialidad { get; set; }
    }
}

public class EspecialidadDoctorVM
{
    public string Especialidad { get; set; }
    public int TotalDoctores { get; set; }
}
