using AutoMapper;
using HotelManager.API.Responses;
using HotelManager.Core.DTOs;
using HotelManager.Core.Entities;
using HotelManager.Core.Interfaces;
using HotelManager.Core.QueryFilters;
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
        //[HttpGet]
        //[ProducesResponseType(typeof(ApiResponse<IEnumerable<ReservaDto>>), (int)HttpStatusCode.OK)]
        //public async Task<IActionResult> GetReservas()
        //{
        //    var reservas = await _reservaService.GetAllReservasAsync();
        //    var reservasDto = _mapper.Map<IEnumerable<ReservaDto>>(reservas);

        //    return Ok(new ApiResponse<IEnumerable<ReservaDto>>(
        //        "Reservas obtenidas correctamente",
        //        reservasDto
        //    ));
        //}
        [HttpGet]
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



        // GET: api/Reservas/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ReservaDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetReserva(int id)
        {
            var reserva = await _reservaService.GetReservaByIdAsync(id);
            var reservaDto = _mapper.Map<ReservaDto>(reserva);

            return Ok(new ApiResponse<ReservaDto>(
                "Reserva obtenida correctamente",
                reservaDto
            ));
        }

        // POST: api/Reservas
        [ProducesResponseType(typeof(ApiResponse<ReservaDto>), (int)HttpStatusCode.Created)]
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

        // PUT: api/Reservas/5
        [HttpPut("{id:int}")]
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

        // DELETE: api/Reservas/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            await _reservaService.DeleteReserva(id);

            return Ok(new ApiResponse<bool>(
                "Reserva eliminada correctamente",
                true
            ));
        }

    }
}
