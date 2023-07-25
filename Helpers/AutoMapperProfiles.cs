using AutoMapper;
using Market.Models;
using Market.Models.DTOS;

namespace Market.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<ApplicationUser, UserDTO>().ReverseMap();
    }
}