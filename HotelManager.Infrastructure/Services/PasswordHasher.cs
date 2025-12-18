using HotelManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Infrastructure.Services
{
    /// <summary>
    /// Implementación de hash de contraseñas usando BCrypt
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
