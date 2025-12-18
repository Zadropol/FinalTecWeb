using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HotelManager.Core.DTOs
{
    /// <summary>
    /// DTO para el inicio de sesión de usuarios
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Nombre de usuario o email
        /// </summary>
        /// <example>admin</example>
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string NombreUsuario { get; set; } = null!;

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        /// <example>Admin123!</example>
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = null!;
    }
}
