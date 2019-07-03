using AutoMapper;
using LandonApi.Models;

namespace LandonApi.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RoomEntity, Room>()
            .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate / 100.0m)) //mapping pre automapper
            .ForMember(dest => dest.Self, opt => opt.MapFrom(src => Link.To(nameof(Controllers.RoomsController.GetRoomById), new { roomId = src.Id }))); //Url.Link()
        }   
    }
}
