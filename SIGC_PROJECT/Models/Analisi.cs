using System;
using System.Collections.Generic;

namespace SIGC_PROJECT.Models;

public partial class Analisi
{
    public int AnalisisId { get; set; }

    public int PacienteId { get; set; }

    public int DoctorId { get; set; }

    public DateTime FechaAnalisis { get; set; }

    public string? TipoAnalisis { get; set; }

    public string? Resultados { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Paciente Paciente { get; set; } = null!;
}
