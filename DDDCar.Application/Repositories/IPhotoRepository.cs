using DDDCar.Core.DtoObjects;

namespace DDDCar.Application.Repositories;

public interface IPhotoRepository
{
    Task<bool> AddAsync(PhotoDto photo);
    Task<bool> DeleteAsync(Guid photoId);
    
    Task<PhotoDto?> FindByIdAsync(Guid photoId);
}