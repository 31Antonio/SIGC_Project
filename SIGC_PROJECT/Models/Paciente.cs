using System;
using System.Collections.Generic;

namespace SIGC_PROJECT.Models;

public partial class Paciente
{
    public int PacienteId { get; set; }

    public string Cedula { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public DateTime? FechaNacimiento { get; set; }

    public int? Edad { get; set; }

    public string? Genero { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public string? CorreoElectronico { get; set; }

    public string? HistorialMedico { get; set; }

    public virtual ICollection<Analisi> Analisis { get; set; } = new List<Analisi>();

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual ICollection<Consulta> Consulta { get; set; } = new List<Consulta>();

    public virtual ICollection<Receta> Receta { get; set; } = new List<Receta>();
}
