using FluentValidation;
using HotelManager.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Infrastructure.Validators
{
    public class ReservaDtoValidator : AbstractValidator<ReservaDto>
    {
        public ReservaDtoValidator()
        {
            // Validar IdHuesped
            RuleFor(x => x.IdHuesped)
                .GreaterThan(0)
                .WithMessage("El ID del huésped debe ser mayor que 0");

            // Validar IdHabitacion
            RuleFor(x => x.IdHabitacion)
                .GreaterThan(0)
                .WithMessage("El ID de la habitación debe ser mayor que 0");

            // Validar IdTipoHabitacion
            RuleFor(x => x.IdTipoHabitacion)
                .GreaterThan(0)
                .WithMessage("El ID del tipo de habitación debe ser mayor que 0");

            // Validar CodigoReserva
            RuleFor(x => x.CodigoReserva)
                .NotEmpty().WithMessage("El código de reserva es requerido")
                .MaximumLength(20).WithMessage("El código de reserva no puede exceder 20 caracteres")
                .Matches(@"^RES-\d{4}-\d{4}$").WithMessage("El código debe tener el formato RES-YYYY-####");

            // Validar FechaCheckIn
            RuleFor(x => x.FechaCheckIn)
                .NotEmpty().WithMessage("La fecha de check-in es requerida");
                //.GreaterThanOrEqualTo(DateTime.Today).WithMessage("La fecha de check-in no puede ser en el pasado");

            // Validar FechaCheckOut
            RuleFor(x => x.FechaCheckOut)
                .NotEmpty().WithMessage("La fecha de check-out es requerida")
                .GreaterThan(x => x.FechaCheckIn).WithMessage("La fecha de check-out debe ser posterior al check-in");

            // Validar NumeroNoches
            RuleFor(x => x.NumeroNoches)
                .GreaterThan(0).WithMessage("El número de noches debe ser mayor que 0")
                .LessThanOrEqualTo(30).WithMessage("No se pueden reservar más de 30 noches");

            // Validar MontoTotal
            RuleFor(x => x.MontoTotal)
                .GreaterThan(0).WithMessage("El monto total debe ser mayor que 0")
                .LessThanOrEqualTo(50000).WithMessage("El monto total no puede exceder 50,000");

            // Validar Estado
            RuleFor(x => x.Estado)
                .NotEmpty().WithMessage("El estado es requerido")
                .Must(BeValidEstado).WithMessage("Estado no válido. Use: Pendiente, Confirmada, En curso, Completada, Cancelada");

            // Validar Observaciones (opcional pero con límite)
            RuleFor(x => x.Observaciones)
                .MaximumLength(1000).WithMessage("Las observaciones no pueden exceder 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Observaciones));
        }

        private bool BeValidEstado(string estado)
        {
            var estadosValidos = new[] { "Pendiente", "Confirmada", "En curso", "Completada", "Cancelada" };
            return estadosValidos.Contains(estado);
        }
    }
}
