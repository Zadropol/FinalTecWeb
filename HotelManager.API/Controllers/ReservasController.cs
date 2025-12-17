using AutoMapper;
using HotelManager.Core.DTOs;
using HotelManager.Core.Entities;
using HotelManager.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HotelManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly IReservaService _reservaService;
        private readonly IMapper _mapper;
        public ReservasController(IReservaService reservaService, IMapper mapper)
        {
            _reservaService = reservaService;
            _mapper = mapper;
        }

        // GET: api/Reservas
        [HttpGet]
        public async Task<IActionResult> GetReservas()
        {
            var reservas = await _reservaService.GetAllReservasAsync();
            var reservasDto = _mapper.Map<IEnumerable<ReservaDto>>(reservas);
            return Ok(reservasDto);
        }

        // GET: api/Reservas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReserva(int id)
        {
            var reserva = await _reservaService.GetReservaByIdAsync(id);

            if (reserva == null)
                return NotFound();

            var reservaDto = _mapper.Map<ReservaDto>(reserva);
            return Ok(reservaDto);
        }

        // POST: api/Reservas
        [HttpPost]
        public async Task<IActionResult> CreateReserva(ReservaDto reservaDto)
        {
            var reserva = _mapper.Map<Reserva>(reservaDto);
            await _reservaService.InsertReserva(reserva);

            var reservaCreada = _mapper.Map<ReservaDto>(reserva);
            return Ok(reservaCreada);
        }

        // PUT: api/Reservas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReserva(int id, ReservaDto reservaDto)
        {
            if (id != reservaDto.IdReserva)
                return BadRequest("El ID no coincide");

            var reserva = _mapper.Map<Reserva>(reservaDto);
            await _reservaService.UpdateReserva(reserva);

            return Ok(reservaDto);
        }

        // DELETE: api/Reservas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            await _reservaService.DeleteReserva(id);
            return NoContent();
        }
    }
}
