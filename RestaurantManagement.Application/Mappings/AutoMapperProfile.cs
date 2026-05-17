using System;
using AutoMapper;
using RestaurantManagement.Application.DTOs.Auth;
using RestaurantManagement.Application.DTOs.Menu;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<RegisterDto, User>().ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
        CreateMap<Category, CategoryDto>();
        CreateMap<FoodItem, FoodItemDto>().ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        CreateMap<CreateFoodItemDto, FoodItem>();
        CreateMap<UpdateFoodItemDto, FoodItem>();

    }
}
