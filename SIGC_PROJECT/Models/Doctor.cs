using System;
using System.Collections.Generic;

namespace SIGC_PROJECT.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public string Cedula { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Especialidad { get; set; } = null!;

    public string NumeroExequatur { get; set; } = null!;

    public int? SecretariaId { get; set; }

    public string? Telefono { get; set; }

    public string? CorreoElectronico { get; set; }

    public int? IdUsuario { get; set; }

    public virtual ICollection<Analisi> Analisis { get; set; } = new List<Analisi>();

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual ICollection<Consulta> Consulta { get; set; } = new List<Consulta>();

    public virtual ICollection<DisponibilidadDoctor> DisponibilidadDoctors { get; set; } = new List<DisponibilidadDoctor>();

    public virtual Usuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<Receta> Receta { get; set; } = new List<Receta>();

    public virtual Secretarium? Secretaria { get; set; }
}
