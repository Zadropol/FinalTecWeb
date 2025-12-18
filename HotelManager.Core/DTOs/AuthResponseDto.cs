using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.DTOs
{
    /// <summary>
    /// Respuesta de autenticación con token JWT
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Token JWT generado
        /// </summary>
        public string Token { get; set; } = null!;

        /// <summary>
        /// Fecha de expiración del token
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Nombre de usuario autenticado
        /// </summary>
        public string NombreUsuario { get; set; } = null!;

        /// <summary>
        /// Email del usuario
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Rol del usuario
        /// </summary>
        public string Rol { get; set; } = null!;

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        public string NombreCompleto { get; set; } = null!;
    }
}
