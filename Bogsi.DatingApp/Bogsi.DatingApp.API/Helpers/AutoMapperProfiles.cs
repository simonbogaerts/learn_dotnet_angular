using System.Linq;
using AutoMapper;
using Bogsi.DatingApp.API.Dtos;
using Bogsi.DatingApp.API.Models;

namespace Bogsi.DatingApp.API.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            // User
            CreateMap<User, UserForListDto>()
                .ForMember(
                    dest => dest.PhotoUrl, 
                    options => options.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age,
                    options => options.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<User, UserForDetailDto>()
                .ForMember(
                    dest => dest.PhotoUrl,
                    options => options.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age,
                    options => options.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<UserForUpdateDto, User>();

            CreateMap<UserForRegisterDto, User>();

            // Photo 
            CreateMap<PhotoForCreationDto, Photo>();

            CreateMap<Photo, PhotoForDetailDto>();

            CreateMap<Photo, PhotoForReturnDto>();
        }
    }
}