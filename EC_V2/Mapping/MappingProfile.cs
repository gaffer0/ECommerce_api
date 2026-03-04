using AutoMapper;
using EC_V2.Dtos;
using EC_V2.Dtos.OrderDtos;
using EC_V2.Models;

namespace EC_V2.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product,ProductDto>();
            CreateMap<AddProductDto, Product>()
                 .ForMember(dest => dest.Categories, opt => opt.Ignore());
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ParentCategory, opt => opt.MapFrom(src => src.Parent));
            CreateMap<AddCategoryDto, Category>();
            CreateMap<Category, UpdateCategoryDto>();
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<OrderItem, OrderItemDto>();
        }

        
    }
}
