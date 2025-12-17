using HotelManager.Core.CustomEntities;
using HotelManager.Core.Entities;
using HotelManager.Core.Exceptions;
using HotelManager.Core.Interfaces;
using HotelManager.Core.QueryFilters;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReservaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<Reserva>> GetReservasAsync(ReservaQueryFilter filters)
        {
            var reservas = await _unitOfWork.ReservaRepository.GetAllReservasAsync();

            if (!string.IsNullOrEmpty(filters.Estado))
                reservas = reservas.Where(r => r.Estado == filters.Estado);

            if (filters.IdHuesped.HasValue)
                reservas = reservas.Where(r => r.IdHuesped == filters.IdHuesped);

            if (filters.FechaDesde.HasValue)
                reservas = reservas.Where(r => r.FechaCheckIn >= filters.FechaDesde);

            if (filters.FechaHasta.HasValue)
                reservas = reservas.Where(r => r.FechaCheckOut <= filters.FechaHasta);

            var totalRecords = reservas.Count();

            var paged = reservas
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            return new PagedList<Reserva>(paged, totalRecords);
        }


        public async Task<Reserva?> GetReservaByIdAsync(int id)
        {
            return await _unitOfWork.ReservaRepository.GetReservaByIdAsync(id);
        }

        public async Task<Reserva?> GetReservaByCodigoAsync(string codigo)
        {
            return await _unitOfWork.ReservaRepository.GetReservaByCodigoAsync(codigo);
        }

        public async Task InsertReserva(Reserva reserva)
        {
            // Verificar que el huésped exista
            var huesped = await _unitOfWork.HuespedRepository.GetById(reserva.IdHuesped);
            if (huesped == null)
            {
                throw new BusinessException("El huésped no existe en el sistema", "HUESPED_NOT_FOUND", 404);
            }

            // Verificar que la habitación exista
            var habitacion = await _unitOfWork.HabitacionRepository.GetById(reserva.IdHabitacion);
            if (habitacion == null)
            {
                throw new BusinessException("La habitación no existe en el sistema", "HABITACION_NOT_FOUND", 404);
            }

            // Verificar que la habitación esté disponible
            if (habitacion.Estado != "Disponible")
            {
                throw new BusinessException(
                    $"La habitación {habitacion.Numero} no está disponible. Estado actual: {habitacion.Estado}",
                    "HABITACION_NO_DISPONIBLE",
                    400
                );
            }

            // Verificar que no haya conflicto de fechas
            var reservasExistentes = await _unitOfWork.ReservaRepository.GetAllReservasAsync();
            var conflicto = reservasExistentes.Any(r =>
                r.IdHabitacion == reserva.IdHabitacion &&
                r.Estado != "Cancelada" &&
                r.Estado != "Completada" &&
                (
                    (r.FechaCheckIn <= reserva.FechaCheckIn && r.FechaCheckOut > reserva.FechaCheckIn) ||
                    (r.FechaCheckIn < reserva.FechaCheckOut && r.FechaCheckOut >= reserva.FechaCheckOut) ||
                    (r.FechaCheckIn >= reserva.FechaCheckIn && r.FechaCheckOut <= reserva.FechaCheckOut)
                )
            );

            if (conflicto)
            {
                throw new BusinessException(
                    $"La habitación {habitacion.Numero} ya tiene una reserva en esas fechas",
                    "CONFLICTO_FECHAS",
                    409
                );
            }

            // Calcular número de noches automáticamente
            var diasDiferencia = (reserva.FechaCheckOut - reserva.FechaCheckIn).Days;
            if (diasDiferencia != reserva.NumeroNoches)
            {
                reserva.NumeroNoches = diasDiferencia;
            }

            // Calcular monto total automáticamente
            var tipoHabitacion = await _unitOfWork.TipoHabitacionRepository.GetById(reserva.IdTipoHabitacion);
            if (tipoHabitacion != null)
            {
                var montoCalculado = tipoHabitacion.PrecioPorNoche * reserva.NumeroNoches;
                if (reserva.MontoTotal != montoCalculado)
                {
                    reserva.MontoTotal = montoCalculado;
                }
            }

            await _unitOfWork.ReservaRepository.InsertReserva(reserva);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateReserva(Reserva reserva)
        {
            var reservaExistente = await _unitOfWork.ReservaRepository.GetReservaByIdAsync(reserva.IdReserva);
            if (reservaExistente == null)
            {
                throw new BusinessException("La reserva no existe", "RESERVA_NOT_FOUND", 404);
            }

            if (reservaExistente.Estado == "Completada")
            {
                throw new BusinessException("No se puede modificar una reserva completada", "RESERVA_COMPLETADA", 400);
            }

            if (reservaExistente.Estado == "Cancelada")
            {
                throw new BusinessException("No se puede modificar una reserva cancelada", "RESERVA_CANCELADA", 400);
            }

            await _unitOfWork.ReservaRepository.UpdateReserva(reserva);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteReserva(int id)
        {
            var reserva = await _unitOfWork.ReservaRepository.GetReservaByIdAsync(id);
            if (reserva == null)
            {
                throw new BusinessException("La reserva no existe", "RESERVA_NOT_FOUND", 404);
            }

            if (reserva.Estado == "En curso")
            {
                throw new BusinessException(
                    "No se puede eliminar una reserva en curso. Debe completarla primero.",
                    "RESERVA_EN_CURSO",
                    400
                );
            }

            await _unitOfWork.ReservaRepository.DeleteReserva(reserva);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
