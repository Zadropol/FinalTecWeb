using HotelManager.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Interfaces
{
    public interface IReservaQueryService
    {
        Task<IEnumerable<HabitacionDisponibleDto>> GetHabitacionesDisponibles(
            DateTime fechaEntrada,
            DateTime fechaSalida,
            int? idTipoHabitacion
        );
    }
}
