using AutoMapper;
using generic_repo_uow_pattern_api.Entity;
using generic_repo_uow_pattern_api.ViewModel;

namespace generic_repo_uow_pattern_api.MapperProfile
{
    public class YourMappingProfile : Profile
    {
        public YourMappingProfile()
        {
            CreateMap<ProductRequest, Product>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Name));

            CreateMap<Product, ProductRequest>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.ProductName));
        }
    }
}