using System;
using System.Collections.Generic;

namespace SIGC_PROJECT.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public string FechaUltimoAcceso { get; set; } = null!;

    public virtual Rol IdRolNavigation { get; set; } = null!;
}
