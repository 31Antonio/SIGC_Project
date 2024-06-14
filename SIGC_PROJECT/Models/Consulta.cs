using System;
using System.Collections.Generic;

namespace SIGC_PROJECT.Models;

public partial class Consulta
{
    public int ConsultaId { get; set; }

    public int PacienteId { get; set; }

    public int DoctorId { get; set; }

    public DateTime FechaConsulta { get; set; }

    public string MotivoConsulta { get; set; } = null!;

    public string? Diagnostico { get; set; }

    public string? Tratamiento { get; set; }

    public int? RecetaId { get; set; }

    public string? Observaciones { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Paciente Paciente { get; set; } = null!;

    public virtual Receta? Receta { get; set; }
}
