using System;
using System.Collections.Generic;

namespace SIGC_PROJECT.Models;

public partial class DisponibilidadDoctor
{
    public int DisponibilidadDoctorId { get; set; }

    public int DoctorId { get; set; }

    public string Dia { get; set; } = null!;

    public string HoraInicio { get; set; } = null!;

    public string HoraFin { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;
}
