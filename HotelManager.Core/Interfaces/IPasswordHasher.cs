using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Interfaces
{
    /// <summary>
    /// Servicio para hash de contraseñas
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Genera un hash de la contraseña
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verifica si una contraseña coincide con su hash
        /// </summary>
        bool VerifyPassword(string password, string hash);
    }
}
