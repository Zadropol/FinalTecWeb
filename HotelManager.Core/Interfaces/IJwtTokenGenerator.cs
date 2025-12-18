using HotelManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Interfaces
{
    /// <summary>
    /// Servicio para generar tokens JWT
    /// </summary>
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Genera un token JWT para un usuario
        /// </summary>
        string GenerateToken(Usuario usuario);

        /// <summary>
        /// Obtiene la fecha de expiración del token
        /// </summary>
        DateTime GetTokenExpiration();
    }
}
