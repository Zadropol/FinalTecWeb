using HotelManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Infrastructure.Data
{
    public class HotelContext :DbContext
    {
        public HotelContext(DbContextOptions<HotelContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Huesped> Huespedes { get; set; }
        public DbSet<TipoHabitacion> TiposHabitacion { get; set; }
        public DbSet<Habitacion> Habitaciones { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<ReservaServicio> ReservaServicios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones adicionales si las necesitas
            // Aquí puedes usar Fluent API para configuraciones más complejas
        }
    }
}
