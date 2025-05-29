using DDDCar.Core.AnswerObjects;
using DDDCar.Core.DtoObjects;

namespace DDDCar.Application.Repositories;

public interface ICarRepository
{
    Task<bool> AddAsync(CarDto car);
    Task<bool> UpdateAsync(CarDto car);
    Task<bool> DeleteAsync(Guid carId);
    
    Task<CarDto?> FindByIdAsync(Guid carId);
    Task<PageResult<CarDto>> FindByParams();
}