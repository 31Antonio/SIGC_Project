using System;
using System.Collections.Generic;

namespace SIGC_PROJECT.Models;

public partial class Modulo
{
    public int IdModulo { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Permiso> IdPermisos { get; set; } = new List<Permiso>();
}
