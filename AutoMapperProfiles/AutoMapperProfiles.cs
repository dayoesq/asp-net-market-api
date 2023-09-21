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
        CreateMap<ProductCreateDto, Product>().ReverseMap();
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
            .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLogin))
            .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.IsVerified))
            .ForMember(dest => dest.VerificationCode, opt => opt.MapFrom(src => src.VerificationCode))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ReverseMap(); 
        CreateMap<DiscountCreateDto, Discount>().ReverseMap();
        CreateMap<ProductImageCreateDto, ProductImage>().ReverseMap();
        CreateMap<CategoryCreateDto, Category>().ReverseMap();
        CreateMap<CategoryUpdateDto, Category>().ReverseMap();
        CreateMap<Discount, DiscountDto>().ReverseMap();
        CreateMap<DiscountUpdateDto, Discount>().ReverseMap();
    }
}