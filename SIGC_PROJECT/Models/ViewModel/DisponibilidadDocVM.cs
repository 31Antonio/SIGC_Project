namespace SIGC_PROJECT.Models.ViewModel
{
    public class DisponibilidadDocVM
    {
        public int DoctorId { get; set; }
        public Dictionary<string, bool> Dias { get; set; }
        public Dictionary<string, string> HorasInicio { get; set; }
        public Dictionary<string, string> HorasFin { get; set; }

        public DisponibilidadDocVM()
        {
            Dias = new Dictionary<string, bool>()
            {
                { "Lunes", false },
                { "Martes", false },
                { "Miercoles", false },
                { "Jueves", false },
                { "Viernes", false },
                { "Sabado", false },
                { "Domingo", false }
            };

            HorasInicio = new Dictionary<string, string>();
            HorasFin = new Dictionary<string, string>();
        }
    }
}
