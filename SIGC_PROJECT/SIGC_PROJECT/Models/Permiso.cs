using System;
using System.Collections.Generic;

namespace SIGC_PROJECT.Models;

public partial class Permiso
{
    public int IdPermiso { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Modulo> IdModulos { get; set; } = new List<Modulo>();

    public virtual ICollection<Rol> IdRols { get; set; } = new List<Rol>();
}
