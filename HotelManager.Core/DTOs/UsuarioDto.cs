using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.DTOs
{
    /// <summary>
    /// DTO para información de usuario
    /// </summary>
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
