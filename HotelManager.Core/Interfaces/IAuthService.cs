using HotelManager.Core.DTOs;
using HotelManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Interfaces
{
    /// <summary>
    /// Servicio de autenticación y autorización
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        Task<Usuario> Register(RegisterDto registerDto);

        /// <summary>
        /// Autentica un usuario y genera un token JWT
        /// </summary>
        Task<AuthResponseDto> Login(LoginDto loginDto);

        /// <summary>
        /// Valida si un usuario existe por nombre de usuario
        /// </summary>
        Task<bool> UserExists(string nombreUsuario);

        /// <summary>
        /// Valida si un email ya está registrado
        /// </summary>
        Task<bool> EmailExists(string email);
    }
}
