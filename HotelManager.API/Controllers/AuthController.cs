using HotelManager.API.Responses;
using HotelManager.Core.DTOs;
using HotelManager.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HotelManager.API.Controllers
{
    /// <summary>
    /// Controlador de autenticación y registro de usuarios
    /// </summary>
    /// <remarks>
    /// Maneja el registro de nuevos usuarios y el inicio de sesión con JWT.
    /// Genera tokens JWT para acceso seguro a la API.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Constructor del controlador de autenticación
        /// </summary>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        /// <remarks>
        /// Crea un nuevo usuario con las siguientes validaciones:
        /// 
        /// **Validaciones:**
        /// - Nombre de usuario único (no puede estar duplicado)
        /// - Email único (no puede estar duplicado)
        /// - Contraseña mínimo 6 caracteres
        /// - Las contraseñas deben coincidir
        /// - Rol válido: Administrador, Recepcionista, Auditor
        /// 
        /// **Seguridad:**
        /// - La contraseña se almacena con hash BCrypt
        /// - No se devuelve la contraseña en la respuesta
        /// - El usuario se crea como activo por defecto
        /// 
        /// **Roles disponibles:**
        /// - **Administrador**: Acceso completo al sistema
        /// - **Recepcionista**: Gestión de reservas y check-in/out
        /// - **Auditor**: Solo lectura de reportes
        /// 
        /// Ejemplo de request:
        /// 
        ///     POST /api/Auth/register
        ///     {
        ///       "nombreUsuario": "recepcionista1",
        ///       "email": "recepcionista@hotel.com",
        ///       "password": "Password123!",
        ///       "confirmPassword": "Password123!",
        ///       "nombre": "María",
        ///       "apellido": "González",
        ///       "rol": "Recepcionista"
        ///     }
        /// 
        /// </remarks>
        /// <param name="registerDto">Datos del nuevo usuario a registrar</param>
        /// <response code="201">Usuario registrado exitosamente</response>
        /// <response code="400">Datos inválidos o contraseñas no coinciden</response>
        /// <response code="409">Usuario o email ya registrado</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Información del usuario creado (sin contraseña)</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<UsuarioDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var usuario = await _authService.Register(registerDto);

            var usuarioDto = new UsuarioDto
            {
                IdUsuario = usuario.IdUsuario,
                NombreUsuario = usuario.NombreUsuario,
                Email = usuario.Email,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Rol = usuario.Rol,
                Activo = usuario.Activo,
                FechaCreacion = usuario.FechaCreacion
            };

            return StatusCode((int)HttpStatusCode.Created, new ApiResponse<UsuarioDto>(
                "Usuario registrado exitosamente",
                usuarioDto
            ));
        }

        /// <summary>
        /// Inicia sesión y genera un token JWT
        /// </summary>
        /// <remarks>
        /// Autentica un usuario y genera un token JWT para acceso a la API.
        /// 
        /// **Proceso de autenticación:**
        /// 1. Verifica que el usuario existe
        /// 2. Valida que el usuario está activo
        /// 3. Verifica la contraseña (hash BCrypt)
        /// 4. Genera token JWT con información del usuario
        /// 5. Retorna token con expiración
        /// 
        /// **Información del Token JWT:**
        /// - Expira en 60 minutos (configurable)
        /// - Contiene: ID, nombre de usuario, email, rol
        /// - Firmado con clave secreta del servidor
        /// - Debe enviarse en header: `Authorization: Bearer {token}`
        /// 
        /// **Uso del Token:**
        /// - Incluir en todas las peticiones protegidas
        /// - Formato: `Authorization: Bearer eyJhbGciOiJIUzI1...`
        /// - Válido hasta la fecha de expiración
        /// 
        /// Ejemplo de request:
        /// 
        ///     POST /api/Auth/login
        ///     {
        ///       "nombreUsuario": "admin",
        ///       "password": "Admin123!"
        ///     }
        /// 
        /// Ejemplo de respuesta exitosa:
        /// 
        ///     {
        ///       "message": "Login exitoso",
        ///       "data": {
        ///         "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///         "expiration": "2024-12-18T15:30:00Z",
        ///         "nombreUsuario": "admin",
        ///         "email": "admin@hotel.com",
        ///         "rol": "Administrador",
        ///         "nombreCompleto": "Admin Sistema"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <param name="loginDto">Credenciales de inicio de sesión</param>
        /// <response code="200">Login exitoso, retorna token JWT</response>
        /// <response code="401">Credenciales inválidas (usuario o contraseña incorrectos)</response>
        /// <response code="403">Usuario inactivo</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Token JWT con información del usuario y fecha de expiración</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var authResponse = await _authService.Login(loginDto);

            return Ok(new ApiResponse<AuthResponseDto>(
                "Login exitoso",
                authResponse
            ));
        }

        /// <summary>
        /// Verifica si un nombre de usuario está disponible
        /// </summary>
        /// <remarks>
        /// Endpoint útil para validación en tiempo real durante el registro.
        /// Permite verificar disponibilidad antes de enviar el formulario completo.
        /// 
        /// Ejemplo de request:
        /// 
        ///     GET /api/Auth/check-username/admin
        /// 
        /// </remarks>
        /// <param name="nombreUsuario">Nombre de usuario a verificar</param>
        /// <response code="200">Retorna si el usuario está disponible o no</response>
        /// <returns>Boolean indicando disponibilidad (true = disponible, false = ya existe)</returns>
        [HttpGet("check-username/{nombreUsuario}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckUsername(string nombreUsuario)
        {
            var exists = await _authService.UserExists(nombreUsuario);
            return Ok(new ApiResponse<bool>(
                exists ? "Nombre de usuario no disponible" : "Nombre de usuario disponible",
                !exists // Retorna true si está disponible (no existe)
            ));
        }

        /// <summary>
        /// Verifica si un email está disponible
        /// </summary>
        /// <remarks>
        /// Endpoint útil para validación en tiempo real durante el registro.
        /// Permite verificar disponibilidad del email antes de enviar el formulario.
        /// 
        /// Ejemplo de request:
        /// 
        ///     GET /api/Auth/check-email/admin@hotel.com
        /// 
        /// </remarks>
        /// <param name="email">Email a verificar</param>
        /// <response code="200">Retorna si el email está disponible o no</response>
        /// <returns>Boolean indicando disponibilidad (true = disponible, false = ya existe)</returns>
        [HttpGet("check-email/{email}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var exists = await _authService.EmailExists(email);
            return Ok(new ApiResponse<bool>(
                exists ? "Email no disponible" : "Email disponible",
                !exists // Retorna true si está disponible (no existe)
            ));
        }

        /// <summary>
        /// Endpoint de prueba para verificar autenticación JWT
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere autenticación con token JWT válido.
        /// Útil para probar que el token funciona correctamente.
        /// 
        /// **Cómo usar:**
        /// 1. Obtén un token con /api/Auth/login
        /// 2. Incluye el token en el header: `Authorization: Bearer {tu_token}`
        /// 3. Llama a este endpoint
        /// 
        /// Ejemplo de request:
        /// 
        ///     GET /api/Auth/test-auth
        ///     Header: Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
        /// 
        /// </remarks>
        /// <response code="200">Token válido, autenticación exitosa</response>
        /// <response code="401">Token inválido, expirado o ausente</response>
        /// <returns>Información del usuario autenticado</returns>
        [Authorize]
        [HttpGet("test-auth")]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public IActionResult TestAuth()
        {
            var userName = User.Identity?.Name;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
            var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            return Ok(new ApiResponse<object>(
                "Autenticación exitosa",
                new
                {
                    Usuario = userName,
                    Rol = userRole,
                    UserId = userId,
                    Mensaje = "Token JWT válido. Usuario autenticado correctamente."
                }
            ));
        }
    }
}
