using System;
using System.Collections.Generic;

namespace SIGC_PROJECT.Models;

public partial class SolicitudesRegistro
{
    public int IdSolicitud { get; set; }

    public string? Nombre { get; set; }

    public string? Clave { get; set; }

    public DateTime? FechaSolicitud { get; set; }

    public int? IdSecretaria { get; set; }
}
