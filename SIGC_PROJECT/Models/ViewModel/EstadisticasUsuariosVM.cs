using SIGC_PROJECT.Models;
using System.Numerics;

namespace SIGC_PROJECT.Models.ViewModel
{
    public class EstadisticasUsuariosVM
    {
        public int TotalUsuarios { get; set; } 
        public List<UsuariosPorRolVM> UsuariosPorRol { get; set; }
        public UsuariosAccesoVM UsuariosAcceso { get; set; }
    }
}

public class UsuariosPorRolVM
{
    public string Rol { get; set; }
    public int CantidadUsuarios { get; set; }
}

public class UsuariosAccesoVM
{
    public List<UsuarioConRolVM> UsuariosHoy { get; set; }
    public int CantidadUsuariosHoy { get; set; }
    public List<RolCantidadVM> RolesHoy { get; set; }
}

public class UsuarioConRolVM
{
    public Usuario Usuario { get; set; }
    public string Rol { get; set; }
}

public class RolCantidadVM
{
    public string Rol { get; set; }
    public int Cantidad { get; set; }
}