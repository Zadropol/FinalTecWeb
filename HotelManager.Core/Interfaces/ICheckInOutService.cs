using HotelManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Interfaces
{
    public interface ICheckInOutService
    {
        Task<Reserva> RealizarCheckIn(int idReserva);
        Task<Reserva> RealizarCheckOut(int idReserva);
        Task<IEnumerable<Reserva>> ObtenerReservasActivas();
        Task<IEnumerable<Habitacion>> ObtenerHabitacionesDisponibles(DateTime fechaInicio, DateTime fechaFin);
    }
}
