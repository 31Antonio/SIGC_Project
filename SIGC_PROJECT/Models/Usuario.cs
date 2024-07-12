using System;
using System.Collections.Generic;

namespace SIGC_PROJECT.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string? Contrasena { get; set; }

    public DateTime? FechaUltimoAcceso { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual ICollection<Paciente> Pacientes { get; set; } = new List<Paciente>();

    public virtual ICollection<Secretarium> Secretaria { get; set; } = new List<Secretarium>();

    public virtual ICollection<Rol> IdRols { get; set; } = new List<Rol>();

    public virtual ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();
}
