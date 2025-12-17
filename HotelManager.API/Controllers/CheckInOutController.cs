using AutoMapper;
using HotelManager.API.Responses;
using HotelManager.Core.DTOs;
using HotelManager.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HotelManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInOutController : ControllerBase
    {
        private readonly ICheckInOutService _checkInOutService;
        private readonly IMapper _mapper;

        public CheckInOutController(ICheckInOutService checkInOutService, IMapper mapper)
        {
            _checkInOutService = checkInOutService;
            _mapper = mapper;
        }

        // POST: api/CheckInOut/checkin/5
        [HttpPost("checkin/{idReserva:int}")]
        [ProducesResponseType(typeof(ApiResponse<ReservaDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> RealizarCheckIn(int idReserva)
        {
            var reserva = await _checkInOutService.RealizarCheckIn(idReserva);
            var reservaDto = _mapper.Map<ReservaDto>(reserva);

            return Ok(new ApiResponse<ReservaDto>(
                $"Check-in realizado exitosamente. Habitación {reserva.Habitacion?.Numero} ocupada.",
                reservaDto
            ));
        }

        // POST: api/CheckInOut/checkout/5
        [HttpPost("checkout/{idReserva:int}")]
        [ProducesResponseType(typeof(ApiResponse<ReservaDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> RealizarCheckOut(int idReserva)
        {
            var reserva = await _checkInOutService.RealizarCheckOut(idReserva);
            var reservaDto = _mapper.Map<ReservaDto>(reserva);

            return Ok(new ApiResponse<ReservaDto>(
                $"Check-out realizado exitosamente. Habitación {reserva.Habitacion?.Numero} lista para limpieza.",
                reservaDto
            ));
        }

        // GET: api/CheckInOut/activas
        [HttpGet("activas")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReservaDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ObtenerReservasActivas()
        {
            var reservas = await _checkInOutService.ObtenerReservasActivas();
            var reservasDto = _mapper.Map<IEnumerable<ReservaDto>>(reservas);

            return Ok(new ApiResponse<IEnumerable<ReservaDto>>(
                $"Se encontraron {reservasDto.Count()} reservas activas",
                reservasDto
            ));
        }

        // GET: api/CheckInOut/disponibles?fechaInicio=2025-01-20&fechaFin=2025-01-25
        [HttpGet("disponibles")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<dynamic>>), (int)HttpStatusCode.OK)]
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
