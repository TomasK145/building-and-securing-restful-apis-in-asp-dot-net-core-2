﻿using AutoMapper;
using LandonApi.Models;

namespace LandonApi.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Automapper -> priklad mapovania properties aj s customizaciou hodnot

            CreateMap<RoomEntity, Room>()
            .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate / 100.0m)) //mapping pre automapper
            .ForMember(dest => dest.Self, opt => opt.MapFrom(src => Link.To(nameof(Controllers.RoomsController.GetRoomById), new { roomId = src.Id }))) //Url.Link()
            .ForMember(dest => dest.Book, opt => opt.MapFrom(src => FormMetadata.FromModel(new BookingForm()
                , Link.ToForm(nameof(Controllers.RoomsController.CreateBookingForRoom), new { roomId = src.Id }, Link.PostMethod, Form.CreateRelation))));

            CreateMap<OpeningEntity, Opening>()
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate / 100m))
                .ForMember(dest => dest.StartAt, opt => opt.MapFrom(src => src.StartAt.UtcDateTime))
                .ForMember(dest => dest.EndAt, opt => opt.MapFrom(src => src.EndAt.UtcDateTime))
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src =>
                    Link.To(
                        nameof(Controllers.RoomsController.GetRoomById),
                        new { roomId = src.RoomId })));

            CreateMap<BookingEntity, Booking>()
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total / 100m))
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src =>
                    Link.To(
                        nameof(Controllers.BookingsController.GetBookingById),
                        new { bookingId = src.Id })))
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src =>
                    Link.To(
                        nameof(Controllers.RoomsController.GetRoomById),
                        new { roomId = src.Id })));

            CreateMap<UserEntity, User>()
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src => Link.To(nameof(Controllers.UsersController.GetUserById), new { userId = src.Id })));
        }   
    }
}
