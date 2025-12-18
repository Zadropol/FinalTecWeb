using HotelManager.Core.DTOs;
using HotelManager.Core.Entities;
using HotelManager.Core.Exceptions;
using HotelManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Infrastructure.Services
{
    /// <summary>
    /// Servicio de autenticación y registro de usuarios
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Usuario> Register(RegisterDto registerDto)
        {
            // Validar que el usuario no exista
            if (await UserExists(registerDto.NombreUsuario))
            {
                throw new BusinessException(
                    "El nombre de usuario ya está registrado",
                    "USER_ALREADY_EXISTS",
                    409
                );
            }

            // Validar que el email no exista
            if (await EmailExists(registerDto.Email))
            {
                throw new BusinessException(
                    "El email ya está registrado",
                    "EMAIL_ALREADY_EXISTS",
                    409
                );
            }

            // Validar rol
            var rolesValidos = new[] { "Administrador", "Recepcionista", "Auditor" };
            if (!rolesValidos.Contains(registerDto.Rol))
            {
                throw new BusinessException(
                    $"Rol inválido. Roles válidos: {string.Join(", ", rolesValidos)}",
                    "INVALID_ROLE",
                    400
                );
            }

            // Crear nuevo usuario
            var usuario = new Usuario
            {
                NombreUsuario = registerDto.NombreUsuario,
                Email = registerDto.Email,
                PasswordHash = _passwordHasher.HashPassword(registerDto.Password),
                Nombre = registerDto.Nombre,
                Apellido = registerDto.Apellido,
                Rol = registerDto.Rol,
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            await _unitOfWork.UsuarioRepository.Add(usuario);
            await _unitOfWork.SaveChangesAsync();

            return usuario;
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            // Buscar usuario por nombre de usuario
            var usuarios = _unitOfWork.UsuarioRepository.GetAll();
            var usuario = usuarios.FirstOrDefault(u =>
                u.NombreUsuario.ToLower() == loginDto.NombreUsuario.ToLower()
            );

            if (usuario == null)
            {
                throw new BusinessException(
                    "Usuario o contraseña incorrectos",
                    "INVALID_CREDENTIALS",
                    401
                );
            }

            // Verificar que el usuario esté activo
            if (!usuario.Activo)
            {
                throw new BusinessException(
                    "El usuario está inactivo. Contacte al administrador",
                    "USER_INACTIVE",
                    403
                );
            }

            // Verificar contraseña
            if (!_passwordHasher.VerifyPassword(loginDto.Password, usuario.PasswordHash))
            {
                throw new BusinessException(
                    "Usuario o contraseña incorrectos",
                    "INVALID_CREDENTIALS",
                    401
                );
            }

            // Generar token
            var token = _jwtTokenGenerator.GenerateToken(usuario);
            var expiration = _jwtTokenGenerator.GetTokenExpiration();

            return new AuthResponseDto
            {
                Token = token,
                Expiration = expiration,
                NombreUsuario = usuario.NombreUsuario,
                Email = usuario.Email,
                Rol = usuario.Rol,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}"
            };
        }

        public async Task<bool> UserExists(string nombreUsuario)
        {
            var usuarios = _unitOfWork.UsuarioRepository.GetAll();
            return usuarios.Any(u => u.NombreUsuario.ToLower() == nombreUsuario.ToLower());
        }

        public async Task<bool> EmailExists(string email)
        {
            var usuarios = _unitOfWork.UsuarioRepository.GetAll();
            return usuarios.Any(u => u.Email.ToLower() == email.ToLower());
        }
    }
}
