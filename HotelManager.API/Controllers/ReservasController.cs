using AutoMapper;
using HotelManager.API.Responses;
using HotelManager.Core.CustomEntities;
using HotelManager.Core.DTOs;
using HotelManager.Core.Entities;
using HotelManager.Core.Interfaces;
using HotelManager.Core.QueryFilters;
using HotelManager.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HotelManager.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de reservas de habitaciones del hotel
    /// </summary>
    /// <remarks>
    /// Este controlador permite realizar operaciones CRUD sobre las reservas,
    /// así como consultar habitaciones disponibles para un período específico.
    /// Incluye validaciones de negocio y manejo de conflictos de fechas.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ReservasController : ControllerBase
    {
        private readonly IReservaService _reservaService;
        private readonly IMapper _mapper;
        private readonly IReservaQueryService _reservaQueryService;

        /// <summary>
        /// Constructor del controlador de reservas
        /// </summary>
        /// <param name="reservaService">Servicio de lógica de negocio para reservas</param>
        /// <param name="reservaQueryService">Servicio de consultas optimizadas con Dapper</param>
        /// <param name="mapper">Mapeador de entidades a DTOs</param>
        public ReservasController(
            IReservaService reservaService,
            IReservaQueryService reservaQueryService,
            IMapper mapper)
        {
            _reservaService = reservaService;
            _mapper = mapper;
            _reservaQueryService = reservaQueryService;
        }

        /// <summary>
        /// Obtiene una lista paginada de reservas con filtros opcionales
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para optimizar las consultas y permite filtrar por:
        /// - Estado de la reserva
        /// - Huésped específico
        /// - Rango de fechas
        /// 
        /// La paginación es obligatoria para mejorar el rendimiento.
        /// 
        /// Ejemplo de request:
        /// 
        ///     GET /api/Reservas?Estado=Confirmada&amp;Page=1&amp;PageSize=10
        ///     GET /api/Reservas?IdHuesped=5&amp;FechaDesde=2024-12-01&amp;FechaHasta=2024-12-31
        /// 
        /// </remarks>
        /// <param name="filters">Filtros de búsqueda y parámetros de paginación</param>
        /// <response code="200">Retorna la lista paginada de reservas</response>
        /// <response code="400">Si los parámetros de paginación son inválidos</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Lista paginada de reservas con metadata de paginación</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<ReservaDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetReservas([FromQuery] ReservaQueryFilter filters)
        {
            var pagedResult = await _reservaService.GetReservasAsync(filters);
            var reservasDto = _mapper.Map<IEnumerable<ReservaDto>>(pagedResult.Items);

            var pagination = new PaginationMetadata
            {
                Page = filters.Page,
                PageSize = filters.PageSize,
                TotalRecords = pagedResult.TotalRecords
            };

            return Ok(new PagedResponse<IEnumerable<ReservaDto>>(
                "Reservas obtenidas correctamente",
                reservasDto,
                pagination
            ));
        }

        /// <summary>
        /// Obtiene una reserva específica por su ID
        /// </summary>
        /// <remarks>
        /// Retorna toda la información de la reserva incluyendo:
        /// - Datos del huésped
        /// - Información de la habitación
        /// - Tipo de habitación y tarifas
        /// - Estado actual de la reserva
        /// 
        /// Ejemplo de request:
        /// 
        ///     GET /api/Reservas/5
        /// 
        /// </remarks>
        /// <param name="id">ID único de la reserva</param>
        /// <response code="200">Retorna la reserva encontrada</response>
        /// <response code="404">Si la reserva no existe</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Información completa de la reserva</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ReservaDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetReserva(int id)
        {
            var reserva = await _reservaService.GetReservaByIdAsync(id);
            var reservaDto = _mapper.Map<ReservaDto>(reserva);

            return Ok(new ApiResponse<ReservaDto>(
                "Reserva obtenida correctamente",
                reservaDto
            ));
        }

        /// <summary>
        /// Crea una nueva reserva de habitación
        /// </summary>
        /// <remarks>
        /// Valida y crea una nueva reserva con las siguientes verificaciones:
        /// 
        /// - El huésped debe existir en el sistema
        /// - La habitación debe existir y estar disponible
        /// - No debe haber conflictos de fechas con otras reservas
        /// - Las fechas deben ser válidas (check-out posterior a check-in)
        /// - Calcula automáticamente el número de noches
        /// - Calcula el monto total según la tarifa de la habitación
        /// 
        /// El código de reserva se genera automáticamente con formato: RES-YYYY-####
        /// 
        /// Ejemplo de request body:
        /// 
        ///     POST /api/Reservas
        ///     {
        ///        "idHuesped": 1,
        ///        "idHabitacion": 5,
        ///        "idTipoHabitacion": 2,
        ///        "codigoReserva": "RES-2024-0001",
        ///        "fechaCheckIn": "2024-12-20",
        ///        "fechaCheckOut": "2024-12-23",
        ///        "numeroNoches": 3,
        ///        "montoTotal": 750.00,
        ///        "estado": "Pendiente",
        ///        "observaciones": "Cliente prefiere piso alto"
        ///     }
        /// 
        /// </remarks>
        /// <param name="reservaDto">Datos de la nueva reserva</param>
        /// <response code="201">Reserva creada exitosamente</response>
        /// <response code="400">Si los datos son inválidos o hay conflicto de fechas</response>
        /// <response code="404">Si el huésped o habitación no existen</response>
        /// <response code="409">Si hay conflicto con otra reserva existente</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>La reserva creada con su código de reserva generado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ReservaDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateReserva([FromBody] ReservaDto reservaDto)
        {
            var reserva = _mapper.Map<Reserva>(reservaDto);
            await _reservaService.InsertReserva(reserva);

            var resultDto = _mapper.Map<ReservaDto>(reserva);

            return StatusCode((int)HttpStatusCode.Created,
                new ApiResponse<ReservaDto>(
                    "Reserva creada correctamente",
                    resultDto
                ));
        }

        /// <summary>
        /// Actualiza una reserva existente
        /// </summary>
        /// <remarks>
        /// Permite modificar los datos de una reserva existente con las siguientes restricciones:
        /// 
        /// - No se pueden modificar reservas completadas
        /// - No se pueden modificar reservas canceladas
        /// - Si se cambian las fechas, se valida disponibilidad
        /// - El ID en la URL debe coincidir con el ID en el body
        /// 
        /// Estados válidos para modificación: Pendiente, Confirmada, En curso
        /// 
        /// Ejemplo de request:
        /// 
        ///     PUT /api/Reservas/5
        ///     {
        ///        "idReserva": 5,
        ///        "idHuesped": 1,
        ///        "idHabitacion": 5,
        ///        "idTipoHabitacion": 2,
        ///        "codigoReserva": "RES-2024-0001",
        ///        "fechaCheckIn": "2024-12-21",
        ///        "fechaCheckOut": "2024-12-24",
        ///        "numeroNoches": 3,
        ///        "montoTotal": 750.00,
        ///        "estado": "Confirmada",
        ///        "observaciones": "Cambio de fecha solicitado por cliente"
        ///     }
        /// 
        /// </remarks>
        /// <param name="id">ID de la reserva a actualizar</param>
        /// <param name="reservaDto">Datos actualizados de la reserva</param>
        /// <response code="200">Reserva actualizada correctamente</response>
        /// <response code="400">Si los datos son inválidos o la reserva no se puede modificar</response>
        /// <response code="404">Si la reserva no existe</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Confirmación de actualización exitosa</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateReserva(int id, [FromBody] ReservaDto reservaDto)
        {
            reservaDto.IdReserva = id;
            var reserva = _mapper.Map<Reserva>(reservaDto);

            await _reservaService.UpdateReserva(reserva);

            return Ok(new ApiResponse<bool>(
                "Reserva actualizada correctamente",
                true
            ));
        }

        /// <summary>
        /// Elimina (cancela) una reserva
        /// </summary>
        /// <remarks>
        /// Elimina lógicamente una reserva del sistema con las siguientes restricciones:
        /// 
        /// - No se pueden eliminar reservas en curso (deben completarse primero)
        /// - Las reservas completadas pueden eliminarse para limpieza de datos
        /// - Las reservas pendientes y confirmadas pueden cancelarse libremente
        /// 
        /// Nota: Esta operación NO libera automáticamente la habitación si está ocupada.
        /// Para finalizar una reserva en curso, usar el endpoint de check-out.
        /// 
        /// Ejemplo de request:
        /// 
        ///     DELETE /api/Reservas/5
        /// 
        /// </remarks>
        /// <param name="id">ID de la reserva a eliminar</param>
        /// <response code="200">Reserva eliminada correctamente</response>
        /// <response code="400">Si la reserva está en curso y no puede eliminarse</response>
        /// <response code="404">Si la reserva no existe</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Confirmación de eliminación exitosa</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            await _reservaService.DeleteReserva(id);

            return Ok(new ApiResponse<bool>(
                "Reserva eliminada correctamente",
                true
            ));
        }

        /// <summary>
        /// Obtiene las habitaciones disponibles para un período específico
        /// </summary>
        /// <remarks>
        /// Consulta optimizada con Dapper que retorna todas las habitaciones disponibles
        /// que NO tienen reservas conflictivas en el rango de fechas especificado.
        /// 
        /// Criterios de disponibilidad:
        /// - La habitación debe estar en estado "Disponible"
        /// - No debe tener reservas confirmadas o en curso en las fechas solicitadas
        /// - Se puede filtrar opcionalmente por tipo de habitación
        /// 
        /// Útil para el proceso de creación de reservas y consultas de disponibilidad.
        /// 
        /// Ejemplo de requests:
        /// 
        ///     GET /api/Reservas/disponibles?fechaEntrada=2024-12-20&amp;fechaSalida=2024-12-23
        ///     GET /api/Reservas/disponibles?fechaEntrada=2024-12-20&amp;fechaSalida=2024-12-23&amp;idTipoHabitacion=2
        /// 
        /// </remarks>
        /// <param name="fechaEntrada">Fecha de entrada (check-in) deseada</param>
        /// <param name="fechaSalida">Fecha de salida (check-out) deseada</param>
        /// <param name="idTipoHabitacion">Filtro opcional por tipo de habitación</param>
        /// <response code="200">Retorna la lista de habitaciones disponibles</response>
        /// <response code="400">Si las fechas son inválidas</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Lista de habitaciones disponibles con sus características y precios</returns>
        [HttpGet("disponibles")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<HabitacionDisponibleDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetHabitacionesDisponibles(
            [FromQuery] DateTime fechaEntrada,
            [FromQuery] DateTime fechaSalida,
            [FromQuery] int? idTipoHabitacion)
        {
            var habitaciones = await _reservaQueryService
                .GetHabitacionesDisponibles(fechaEntrada, fechaSalida, idTipoHabitacion);

            return Ok(new ApiResponse<IEnumerable<HabitacionDisponibleDto>>(
                $"Se encontraron {habitaciones.Count()} habitaciones disponibles",
                habitaciones
            ));
        }
    }
}
