using AutoMapper;

namespace AssetAllocation.Api;

public class GetCarAllocationsByUserMapperProfile : Profile
{
    public GetCarAllocationsByUserMapperProfile()
    {
        CreateMap<CarAllocation, GetCarAllocationsByUserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CarMake, opt => opt.MapFrom(src => src.Car!.Model.Make.Name))
            .ForMember(dest => dest.CarModel, opt => opt.MapFrom(src => src.Car!.Model.Name))
            .ForMember(dest => dest.CarYear, opt => opt.MapFrom(src => src.Car!.Year))
            .ForMember(dest => dest.CarColor, opt => opt.MapFrom(src => src.Car!.Color))
            .ForMember(dest => dest.Plate, opt => opt.MapFrom(src => src.Car!.Plate))
            .ForMember(dest => dest.AllocationDate, opt => opt.MapFrom(src => src.AllocationDate))
            .ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => src.ReturnDate));
    }
}
