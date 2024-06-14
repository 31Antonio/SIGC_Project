using System;
using System.Collections.Generic;

namespace SIGC_PROJECT.Models;

public partial class Secretarium
{
    public int SecretariaId { get; set; }

    public string Cedula { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? CorreoElectronico { get; set; }

    public string? HorarioTrabajo { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
