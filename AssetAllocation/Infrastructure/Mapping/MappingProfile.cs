using AutoMapper;

namespace AssetAllocation.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AllocateCarCommand, CarAllocation>().ReverseMap();
        CreateMap<CarAllocation, CarAllocatedResponse>().ReverseMap();
        
        CreateMap<CarAllocation, CarDeallocatedResponse>().ReverseMap();

        CreateMap<Car, GetCarsDto>()
            .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Model.Make.Name))
            .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model.Name))
            .ReverseMap();
        CreateMap<PaginatedList<GetCarsDto>, GetListResponse<GetCarsDto>>().ReverseMap();

        CreateMap<Car, GetCarsByDynamicDto>()
            .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Model.Make.Name))
            .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model.Name))
            .ReverseMap();
        CreateMap<PaginatedList<GetCarsByDynamicDto>, GetListResponse<GetCarsByDynamicDto>>().ReverseMap();

        CreateMap<Car, GetCarByIdResponse>()
            .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Model.Make.Name))
            .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model.Name))
            .ReverseMap();

        CreateMap<CarMake, CarMakeDeletedResponse>().ReverseMap();

        CreateMap<CarModel, CarModelDeletedResponse>().ReverseMap();

        CreateMap<Car, CarCreatedResponse>().ReverseMap();

        CreateMap<Car, CarDeletedResponse>().ReverseMap();

        CreateMap<Car, CarMileageUpdatedResponse>().ReverseMap();

        CreateMap<CarMake, CarMakeCreatedResponse>().ReverseMap();
        CreateMap<CarModel, CarModelCreatedResponse>().ReverseMap();

        CreateMap<CreatePersonCommand, Person>().ReverseMap();
        CreateMap<Person, PersonCreatedResponse>().ReverseMap();
    }
}