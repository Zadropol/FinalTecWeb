using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HotelManager.Core.DTOs
{
    /// <summary>
    /// DTO para el registro de nuevos usuarios
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Nombre de usuario único
        /// </summary>
        /// <example>recepcionista1</example>
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "El nombre de usuario debe tener entre 4 y 50 caracteres")]
        public string NombreUsuario { get; set; } = null!;

        /// <summary>
        /// Correo electrónico del usuario
        /// </summary>
        /// <example>recepcionista@hotel.com</example>
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        /// <example>Password123!</example>
        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = null!;

        /// <summary>
        /// Confirmación de contraseña
        /// </summary>
        /// <example>Password123!</example>
        [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = null!;

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        /// <example>Juan</example>
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        /// <summary>
        /// Apellido del usuario
        /// </summary>
        /// <example>Pérez</example>
        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100)]
        public string Apellido { get; set; } = null!;

        /// <summary>
        /// Rol del usuario en el sistema
        /// </summary>
        /// <example>Recepcionista</example>
        [Required(ErrorMessage = "El rol es requerido")]
        public string Rol { get; set; } = "Recepcionista"; // Default
    }
}
