using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDTO>()
                .ForMember(dest => dest.PhotoUrl, opt => 
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(scr => scr.DateOfBirth.CalculateAge()));
            
            CreateMap<User, UserForDetailedDTO>()
                .ForMember(dest => dest.PhotoUrl, opt => 
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(scr => scr.DateOfBirth.CalculateAge()));
            
            CreateMap<Photo, PhotoForDetailedDTO>();
            
            CreateMap<UserForUpdateDTO, User>();
            
            CreateMap<Photo, PhotoForReturnDTO>();
            
            CreateMap<PhotoForCreationDTO, Photo>();
            
            CreateMap<UserForRegisterDTO, User>();
            
            CreateMap<MessageForCreationDTO, Message>().ReverseMap();

            CreateMap<Message, MessageToReturnDTO>()
                .ForMember(dest => dest.SenderPhotoUrl, opt => 
                    opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.RecipientPhotoUrl, opt => 
                    opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}