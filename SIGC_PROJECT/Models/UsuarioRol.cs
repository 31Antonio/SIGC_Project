using System.Data;

namespace SIGC_PROJECT.Models
{
    public class UsuarioRol
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }

        public Rol Rol { get; set; } = null!;

        public Usuario Usuario { get; set; } = null!;
    }
}
