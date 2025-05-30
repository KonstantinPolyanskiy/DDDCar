using AutoMapper;
using DDDCar.Core.DomainObjects;
using DDDCar.Core.DtoObjects;

namespace DDDCar.Application.Mappers;

public sealed class PhotoProfile : Profile
{
    public PhotoProfile()
    {
        CreateMap<PhotoDto, Photo>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.EntityId));
    }
}