using AutoMapper;
using HotelManager.API.Responses;
using HotelManager.Core.CustomEntities;
using HotelManager.Core.DTOs;
using HotelManager.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HotelManager.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de check-in y check-out de huéspedes
    /// </summary>
    /// <remarks>
    /// Este controlador maneja el proceso completo de entrada y salida de huéspedes:
    /// - Check-in: Registro de llegada y asignación de habitación
    /// - Check-out: Proceso de salida y liberación de habitación
    /// - Consulta de reservas activas
    /// - Consulta de disponibilidad de habitaciones
    /// 
    /// Incluye validaciones de estado y actualización automática de habitaciones.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CheckInOutController : ControllerBase
    {
        private readonly ICheckInOutService _checkInOutService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor del controlador de check-in/check-out
        /// </summary>
        /// <param name="checkInOutService">Servicio de lógica de negocio para check-in/check-out</param>
        /// <param name="mapper">Mapeador de entidades a DTOs</param>
        public CheckInOutController(ICheckInOutService checkInOutService, IMapper mapper)
        {
            _checkInOutService = checkInOutService;
            _mapper = mapper;
        }

        /// <summary>
        /// Realiza el check-in de un huésped (registro de entrada)
        /// </summary>
        /// <remarks>
        /// Proceso de check-in que incluye:
        /// 
        /// - Validación de que la reserva existe y está confirmada
        /// - Verificación de que es la fecha correcta de check-in
        /// - Confirmación de disponibilidad de la habitación
        /// - Actualización del estado de la reserva a "En curso"
        /// - Cambio del estado de la habitación a "Ocupada"
        /// - Registro de fecha y hora real de entrada
        /// 
        /// Validaciones:
        /// - Solo se puede hacer check-in de reservas en estado "Confirmada"
        /// - La fecha de check-in debe ser hoy o anterior
        /// - La habitación debe estar disponible
        /// 
        /// Ejemplo de request:
        /// 
        ///     POST /api/CheckInOut/checkin/5
        /// 
        /// </remarks>
        /// <param name="idReserva">ID de la reserva para hacer check-in</param>
        /// <response code="200">Check-in realizado exitosamente</response>
        /// <response code="400">Si la reserva no está en estado válido o la fecha es incorrecta</response>
        /// <response code="404">Si la reserva o habitación no existe</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Información actualizada de la reserva después del check-in</returns>
        [HttpPost("checkin/{idReserva:int}")]
        [ProducesResponseType(typeof(ApiResponse<ReservaDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RealizarCheckIn(int idReserva)
        {
            var reserva = await _checkInOutService.RealizarCheckIn(idReserva);
            var reservaDto = _mapper.Map<ReservaDto>(reserva);

            return Ok(new ApiResponse<ReservaDto>(
                $"Check-in realizado exitosamente. Habitación {reserva.Habitacion?.Numero} ocupada.",
                reservaDto
            ));
        }

        /// <summary>
        /// Realiza el check-out de un huésped (registro de salida)
        /// </summary>
        /// <remarks>
        /// Proceso de check-out que incluye:
        /// 
        /// - Validación de que la reserva existe y está en curso
        /// - Verificación de pagos pendientes (reserva + servicios adicionales)
        /// - Actualización del estado de la reserva a "Completada"
        /// - Cambio del estado de la habitación a "Limpieza"
        /// - Registro de fecha y hora real de salida
        /// - Generación de factura final
        /// 
        /// Validaciones:
        /// - Solo se puede hacer check-out de reservas en estado "En curso"
        /// - No debe haber pagos pendientes
        /// - Se incluyen en el cálculo los servicios adicionales consumidos
        /// 
        /// El proceso actualiza automáticamente la habitación para que pase a limpieza
        /// antes de estar disponible para la próxima reserva.
        /// 
        /// Ejemplo de request:
        /// 
        ///     POST /api/CheckInOut/checkout/5
        /// 
        /// </remarks>
        /// <param name="idReserva">ID de la reserva para hacer check-out</param>
        /// <response code="200">Check-out realizado exitosamente</response>
        /// <response code="400">Si la reserva no está en curso o hay pagos pendientes</response>
        /// <response code="404">Si la reserva no existe</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Información final de la reserva después del check-out</returns>
        [HttpPost("checkout/{idReserva:int}")]
        [ProducesResponseType(typeof(ApiResponse<ReservaDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RealizarCheckOut(int idReserva)
        {
            var reserva = await _checkInOutService.RealizarCheckOut(idReserva);
            var reservaDto = _mapper.Map<ReservaDto>(reserva);

            return Ok(new ApiResponse<ReservaDto>(
                $"Check-out realizado exitosamente. Habitación {reserva.Habitacion?.Numero} lista para limpieza.",
                reservaDto
            ));
        }

        /// <summary>
        /// Obtiene todas las reservas activas del hotel
        /// </summary>
        /// <remarks>
        /// Retorna una lista de todas las reservas que están actualmente:
        /// - En curso (huéspedes que ya hicieron check-in)
        /// - Confirmadas (próximas llegadas)
        /// 
        /// Útil para:
        /// - Panel de control de recepción
        /// - Verificar ocupación actual
        /// - Gestionar llegadas del día
        /// - Consultar huéspedes hospedados
        /// 
        /// No incluye reservas completadas, canceladas o pendientes.
        /// 
        /// Ejemplo de request:
        /// 
        ///     GET /api/CheckInOut/activas
        /// 
        /// </remarks>
        /// <response code="200">Lista de reservas activas obtenida correctamente</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Colección de reservas en estado "En curso" o "Confirmada"</returns>
        [HttpGet("activas")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReservaDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ObtenerReservasActivas()
        {
            var reservas = await _checkInOutService.ObtenerReservasActivas();
            var reservasDto = _mapper.Map<IEnumerable<ReservaDto>>(reservas);

            return Ok(new ApiResponse<IEnumerable<ReservaDto>>(
                $"Se encontraron {reservasDto.Count()} reservas activas",
                reservasDto
            ));
        }

        /// <summary>
        /// Obtiene las habitaciones disponibles en un rango de fechas
        /// </summary>
        /// <remarks>
        /// Consulta las habitaciones que están disponibles para reserva en el período especificado.
        /// 
        /// Criterios de disponibilidad:
        /// - Estado de habitación: "Disponible"
        /// - Sin reservas conflictivas en el rango de fechas
        /// - Habitación activa en el sistema
        /// 
        /// La respuesta incluye información completa de cada habitación:
        /// - Número y piso
        /// - Tipo de habitación
        /// - Capacidad máxima
        /// - Precio por noche
        /// - Descripción y amenidades
        /// 
        /// Validación de fechas:
        /// - La fecha de inicio debe ser anterior a la fecha de fin
        /// - Ambas fechas son requeridas
        /// 
        /// Ejemplo de request:
        /// 
        ///     GET /api/CheckInOut/disponibles?fechaInicio=2025-01-20&amp;fechaFin=2025-01-25
        /// 
        /// </remarks>
        /// <param name="fechaInicio">Fecha de inicio del período a consultar (check-in)</param>
        /// <param name="fechaFin">Fecha de fin del período a consultar (check-out)</param>
        /// <response code="200">Lista de habitaciones disponibles obtenida correctamente</response>
        /// <response code="400">Si las fechas son inválidas (fecha inicio >= fecha fin)</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Colección de habitaciones disponibles con su información completa</returns>
        [HttpGet("disponibles")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<dynamic>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ObtenerHabitacionesDisponibles(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            if (fechaInicio >= fechaFin)
            {
                return BadRequest(new ApiResponse<object>(
                    "La fecha de inicio debe ser anterior a la fecha de fin",
                    null
                ));
            }

            var habitaciones = await _checkInOutService.ObtenerHabitacionesDisponibles(
                fechaInicio,
                fechaFin
            );

            var habitacionesInfo = habitaciones.Select(h => new
            {
                h.IdHabitacion,
                h.Numero,
                h.Piso,
                TipoHabitacion = h.TipoHabitacion?.Nombre,
                Capacidad = h.TipoHabitacion?.Capacidad,
                PrecioPorNoche = h.TipoHabitacion?.PrecioPorNoche,
                h.Descripcion,
                h.Estado
            });

            return Ok(new ApiResponse<IEnumerable<dynamic>>(
                $"Se encontraron {habitacionesInfo.Count()} habitaciones disponibles",
                habitacionesInfo
            ));
        }
    }
}
