using AutoMapper;
using MTracking.BLL.DTOs.User.Requests;
using MTracking.BLL.DTOs.User.Responses;
using MTracking.Core.Entities;

namespace MTracking.BLL.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponseDto>().ReverseMap();

            CreateMap<User, UserRegisterDto>().ReverseMap();
        }
    }
}
