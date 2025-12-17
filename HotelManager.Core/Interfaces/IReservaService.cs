using HotelManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Interfaces
{
    public interface IReservaService
    {
        Task<IEnumerable<Reserva>> GetAllReservasAsync();
        Task<Reserva?> GetReservaByIdAsync(int id);
        Task<Reserva?> GetReservaByCodigoAsync(string codigo);
        Task InsertReserva(Reserva reserva);
        Task UpdateReserva(Reserva reserva);
        Task DeleteReserva(int id);
    }
}
