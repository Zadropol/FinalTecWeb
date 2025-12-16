using HotelManager.Core.Entities;
using HotelManager.Core.Interfaces;
using HotelManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Infrastructure.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly HotelContext _context;

        public ReservaRepository(HotelContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reserva>> GetAllReservasAsync()
        {
            return await _context.Reservas
                .Include(r => r.Huesped)
                .Include(r => r.Habitacion)
                .Include(r => r.TipoHabitacion)
                .ToListAsync();
        }

        public async Task<Reserva?> GetReservaByIdAsync(int id)
        {
            return await _context.Reservas
                .Include(r => r.Huesped)
                .Include(r => r.Habitacion)
                .Include(r => r.TipoHabitacion)
                .FirstOrDefaultAsync(r => r.IdReserva == id);
        }

        public async Task<Reserva?> GetReservaByCodigoAsync(string codigo)
        {
            return await _context.Reservas
                .Include(r => r.Huesped)
                .Include(r => r.Habitacion)
                .Include(r => r.TipoHabitacion)
                .FirstOrDefaultAsync(r => r.CodigoReserva == codigo);
        }

        public async Task InsertReserva(Reserva reserva)
        {
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReserva(Reserva reserva)
        {
            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReserva(Reserva reserva)
        {
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
        }
    }
}
