using AutoMapper;
using HotelManager.Core.DTOs;
using HotelManager.Core.Entities;
using HotelManager.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ReservasController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Reservas
        [HttpGet]
        public async Task<IActionResult> GetReservas()
        {
            var reservas = await _unitOfWork.ReservaRepository.GetAllReservasAsync();

            var reservasDto = _mapper.Map<IEnumerable<ReservaDto>>(reservas);

            return Ok(reservasDto);
        }

        // GET: api/Reservas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReserva(int id)
        {
            var reserva = await _unitOfWork.ReservaRepository.GetReservaByIdAsync(id);

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
            await _unitOfWork.ReservaRepository.InsertReserva(reserva);
            await _unitOfWork.SaveChangesAsync();

            var reservaCreada = _mapper.Map<ReservaDto>(reserva);
            return Ok(reserva);
        }

        // PUT: api/Reservas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReserva(int id, ReservaDto reservaDto)
        {
            if (id != reservaDto.IdReserva)
                return BadRequest("El ID no coincide");

            var reservaExistente = await _unitOfWork.ReservaRepository.GetReservaByIdAsync(id);
            if (reservaExistente == null)
                return NotFound();

            var reserva = _mapper.Map<Reserva>(reservaDto);
            await _unitOfWork.ReservaRepository.UpdateReserva(reserva);
            await _unitOfWork.SaveChangesAsync();

            return Ok(reserva);
        }

        // DELETE: api/Reservas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            var reserva = await _unitOfWork.ReservaRepository.GetReservaByIdAsync(id);
            if (reserva == null)
                return NotFound();

            await _unitOfWork.ReservaRepository.DeleteReserva(reserva);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
}
