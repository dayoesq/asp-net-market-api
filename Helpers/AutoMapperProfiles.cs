using AutoMapper;
using Market.Models;
using Market.Models.Dtos;

namespace Market.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<UserCreationDto, User>().ReverseMap();
        CreateMap<UserDto, User>().ReverseMap();
    }
}