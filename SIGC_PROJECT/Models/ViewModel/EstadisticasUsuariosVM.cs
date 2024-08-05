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
    public List<Usuario> UsuariosHoy { get; set; }
    public List<Usuario> UsuariosEsteMes { get; set; }
    public int CantidadUsuariosHoy { get; set; }
    public int CantidadUsuariosEsteMes { get; set; }
}