using AutoMapper;
using DDDCar.Core.DomainObjects;
using DDDCar.Core.DtoObjects;

namespace DDDCar.Application.Mappers;

public sealed class CarProfile : Profile
{
    public CarProfile()
    {
        CreateMap<Car, CarDto>()
            .ForMember(dto => dto.PhotoId,
                opt => opt.MapFrom(car => car.Photo != null
                    ? (Guid?)car.Photo.Id
                    : null))

            .ForMember(dto => dto.Price,
                opt => opt.MapFrom(car => (decimal?)car.Price))
            
            .ForMember(dto => dto.Condition,
                opt => opt.MapFrom(car => (Condition?)car.Condition))

            .ForMember(dto => dto.IsAvailable,
                opt => opt.MapFrom(car => car.IsAvailable));
    }
}