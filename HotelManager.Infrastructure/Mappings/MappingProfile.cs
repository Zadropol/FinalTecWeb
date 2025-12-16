using AutoMapper;
using HotelManager.Core.DTOs;
using HotelManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo de Reserva a ReservaDto
            _ = CreateMap<Reserva, ReservaDto>()
                .ForMember(dest => dest.NombreHuesped,
                    opt => opt.MapFrom(src => $"{src.Huesped.Nombre} {src.Huesped.Apellido}"))
                .ForMember(dest => dest.NumeroHabitacion,
                    opt => opt.MapFrom(src => src.Habitacion.Numero))
                .ForMember(dest => dest.TipoHabitacion,
                    opt => opt.MapFrom(src => src.TipoHabitacion.Nombre));

            // Mapeo de ReservaDto a Reserva (para POST/PUT)
            CreateMap<ReservaDto, Reserva>();
        }
    }
}
