using System;
using System.Collections.Generic;

namespace SIGC_PROJECT.Models;

public partial class Receta
{
    public int RecetaId { get; set; }

    public int PacienteId { get; set; }

    public int DoctorId { get; set; }

    public DateTime? FechaEmision { get; set; }

    public string? Medicamentos { get; set; }

    public string? Indicaciones { get; set; }

    public virtual ICollection<Consulta> Consulta { get; set; } = new List<Consulta>();

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Paciente Paciente { get; set; } = null!;
}
