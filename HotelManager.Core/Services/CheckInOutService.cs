using HotelManager.Core.Entities;
using HotelManager.Core.Exceptions;
using HotelManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Services
{
    public class CheckInOutService : ICheckInOutService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckInOutService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Reserva> RealizarCheckIn(int idReserva)
        {
            // Obtener la reserva
            var reserva = await _unitOfWork.ReservaRepository.GetReservaByIdAsync(idReserva);
            if (reserva == null)
            {
                throw new BusinessException("La reserva no existe", "RESERVA_NOT_FOUND", 404);
            }

            // Validar que la reserva esté confirmada
            if (reserva.Estado != "Confirmada")
            {
                throw new BusinessException(
                    $"Solo se puede hacer check-in a reservas confirmadas. Estado actual: {reserva.Estado}",
                    "ESTADO_INVALIDO",
                    400
                );
            }

            // Validar que la fecha de check-in sea hoy o anterior
            if (reserva.FechaCheckIn > DateTime.Today)
            {
                throw new BusinessException(
                    $"La fecha de check-in es {reserva.FechaCheckIn:dd/MM/yyyy}. No puede hacer check-in antes de esa fecha.",
                    "FECHA_CHECKIN_FUTURA",
                    400
                );
            }

            // Verificar que la habitación esté disponible
            var habitacion = await _unitOfWork.HabitacionRepository.GetById(reserva.IdHabitacion);
            if (habitacion == null)
            {
                throw new BusinessException("La habitación no existe", "HABITACION_NOT_FOUND", 404);
            }

            if (habitacion.Estado != "Disponible")
            {
                throw new BusinessException(
                    $"La habitación {habitacion.Numero} no está disponible. Estado: {habitacion.Estado}",
                    "HABITACION_NO_DISPONIBLE",
                    400
                );
            }

            // Realizar check-in
            reserva.Estado = "En curso";
            reserva.FechaCheckInReal = DateTime.Now;

            // Actualizar estado de la habitación a "Ocupada"
            habitacion.Estado = "Ocupada";

            _unitOfWork.HabitacionRepository.Update(habitacion);
            await _unitOfWork.ReservaRepository.UpdateReserva(reserva);
            await _unitOfWork.SaveChangesAsync();

            return reserva;
        }

        public async Task<Reserva> RealizarCheckOut(int idReserva)
        {
            // Obtener la reserva
            var reserva = await _unitOfWork.ReservaRepository.GetReservaByIdAsync(idReserva);
            if (reserva == null)
            {
                throw new BusinessException("La reserva no existe", "RESERVA_NOT_FOUND", 404);
            }

            // Validar que la reserva esté en curso
            if (reserva.Estado != "En curso")
            {
                throw new BusinessException(
                    $"Solo se puede hacer check-out a reservas en curso. Estado actual: {reserva.Estado}",
                    "ESTADO_INVALIDO",
                    400
                );
            }

            // Verificar si hay pagos pendientes
            var pagos = await ObtenerPagosPorReserva(reserva.IdReserva);
            var totalPagado = pagos.Where(p => p.EstadoPago == "Completado").Sum(p => p.Monto);
            var totalConServicios = await CalcularTotalConServicios(reserva.IdReserva);

            if (totalPagado < totalConServicios)
            {
                throw new BusinessException(
                    $"Hay pagos pendientes. Total: {totalConServicios:C}, Pagado: {totalPagado:C}",
                    "PAGOS_PENDIENTES",
                    400
                );
            }

            // Realizar check-out
            reserva.Estado = "Completada";
            reserva.FechaCheckOutReal = DateTime.Now;

            // Actualizar estado de la habitación a "Limpieza"
            var habitacion = await _unitOfWork.HabitacionRepository.GetById(reserva.IdHabitacion);
            if (habitacion != null)
            {
                habitacion.Estado = "Limpieza";
                _unitOfWork.HabitacionRepository.Update(habitacion);
            }

            await _unitOfWork.ReservaRepository.UpdateReserva(reserva);
            await _unitOfWork.SaveChangesAsync();

            return reserva;
        }

        public async Task<IEnumerable<Reserva>> ObtenerReservasActivas()
        {
            var todasReservas = await _unitOfWork.ReservaRepository.GetAllReservasAsync();
            return todasReservas.Where(r => r.Estado == "En curso" || r.Estado == "Confirmada");
        }

        public async Task<IEnumerable<Habitacion>> ObtenerHabitacionesDisponibles(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            var todasHabitaciones = await _unitOfWork.HabitacionRepository.GetAllConTipoAsync();
            var habitacionesDisponibles = new List<Habitacion>();

            var todasReservas = await _unitOfWork.ReservaRepository.GetAllReservasAsync();

            foreach (var habitacion in todasHabitaciones)
            {
                if (habitacion.Estado != "Disponible")
                    continue;

                // Verificar si hay reservas en conflicto
                var tieneConflicto = todasReservas.Any(r =>
                    r.IdHabitacion == habitacion.IdHabitacion &&
                    r.Estado != "Cancelada" &&
                    r.Estado != "Completada" &&
                    (
                        (r.FechaCheckIn <= fechaInicio && r.FechaCheckOut > fechaInicio) ||
                        (r.FechaCheckIn < fechaFin && r.FechaCheckOut >= fechaFin) ||
                        (r.FechaCheckIn >= fechaInicio && r.FechaCheckOut <= fechaFin)
                    )
                );

                if (!tieneConflicto)
                {
                    habitacionesDisponibles.Add(habitacion);
                }
            }

            return habitacionesDisponibles;
        }

        // Métodos auxiliares privados
        private async Task<IEnumerable<Pago>> ObtenerPagosPorReserva(int idReserva)
        {
            var todosPagos = _unitOfWork.PagoRepository.GetAll();
            return todosPagos.Where(p => p.IdReserva == idReserva);
        }

        private async Task<decimal> CalcularTotalConServicios(int idReserva)
        {
            var reserva = await _unitOfWork.ReservaRepository.GetReservaByIdAsync(idReserva);
            if (reserva == null)
                return 0;

            var servicios = _unitOfWork.ServicioRepository.GetAll();
            var reservaServicios = servicios.SelectMany(s => s.ReservaServicios)
                .Where(rs => rs.IdReserva == idReserva);

            var totalServicios = reservaServicios.Sum(rs => rs.SubTotal);
            return reserva.MontoTotal + totalServicios;
        }



    }
}
