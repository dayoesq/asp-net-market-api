using AutoMapper;
using Market.Models;
using Market.Models.DTOS;

namespace Market.AutoMapperProfiles;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<RegisterDto, ApplicationUser>()
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
    }
}