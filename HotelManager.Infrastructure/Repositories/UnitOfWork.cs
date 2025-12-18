using HotelManager.Core.Entities;
using HotelManager.Core.Interfaces;
using HotelManager.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HotelContext _context;
        private readonly IReservaRepository? _reservaRepository;
        private readonly IBaseRepository<Huesped>? _huespedRepository;
        private readonly IHabitacionRepository? _habitacionRepository;
        private readonly IBaseRepository<TipoHabitacion>? _tipoHabitacionRepository;
        private readonly IBaseRepository<Pago>? _pagoRepository;
        private readonly IBaseRepository<Servicio>? _servicioRepository;
        private readonly IBaseRepository<Usuario>? _usuarioRepository;

        public UnitOfWork(HotelContext context)
        {
            _context = context;
        }

        public IReservaRepository ReservaRepository =>
            _reservaRepository ?? new ReservaRepository(_context);

        public IBaseRepository<Huesped> HuespedRepository =>
            _huespedRepository ?? new BaseRepository<Huesped>(_context);

        public IHabitacionRepository HabitacionRepository =>
            _habitacionRepository ?? new HabitacionRepository(_context);

        public IBaseRepository<TipoHabitacion> TipoHabitacionRepository =>
            _tipoHabitacionRepository ?? new BaseRepository<TipoHabitacion>(_context);

        public IBaseRepository<Pago> PagoRepository =>
            _pagoRepository ?? new BaseRepository<Pago>(_context);

        public IBaseRepository<Servicio> ServicioRepository =>
            _servicioRepository ?? new BaseRepository<Servicio>(_context);

        public IBaseRepository<Usuario> UsuarioRepository =>
           _usuarioRepository ?? new BaseRepository<Usuario>(_context);

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }
    }
}
