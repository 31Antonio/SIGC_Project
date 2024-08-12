namespace SIGC_PROJECT.Models.ViewModel
{
    public class Doctor_DisponibilidadVM
    {
        public int idDoctor { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Especialidad { get; set; }
        public string Consultorio { get; set; }

        public List<DisponibilidadDoctor> Disponibilidades { get; set; }

        public Doctor_DisponibilidadVM() 
        {
            Disponibilidades = new List<DisponibilidadDoctor>();
        }
    }
}
