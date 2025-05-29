using System.Net;
using System.Security.Claims;
using DDDCar.Application.Helpers;
using DDDCar.Application.Repositories;
using DDDCar.Core.AnswerObjects;
using DDDCar.Core.DomainObjects;
using DDDCar.Core.DomainObjects.Actions;
using DDDCar.Core.DtoObjects;
using Microsoft.Extensions.Logging;

namespace DDDCar.Application.Services;

public class CarService(ICarRepository carRepo, IPhotoRepository photoRepo, ILogger<CarService> log)
{
    public async Task<ServiceResult<CarInfo>> AddAsync(CarDto carDto, User employer, PhotoDto? photoDto = null)
    {
        var warns = new List<AppError>();
        
        var car = new Car();
        
        var carResult = car.Create(
            carDto.Brand ?? "empty", 
            Enum.Parse<Color>(carDto.Color ?? "unknown", true), 
            (decimal)carDto.Price!, 
            carDto.CurrentOwner, 
            carDto.Mileage, 
            employer);
        
        if (carResult.Status is not CreateCarAction.Success)
            return ServiceResult<CarInfo>.Failure(CarErrorHelper.CarCreateCritical(carResult.Status));

        var photoId = photoDto is not null ?
            await AttachAndSaveCarPhoto(car, photoDto, employer) :
            null;
        
        if (photoId is null)
            warns.Add(CarErrorHelper.UnsuccessfulSavePhotoWarn());

        var saved = await carRepo.AddAsync(new CarDto
        {
            EntityId = carResult.CarId!.Value,
            ManagerId = car.ManagerId,
            PhotoId = photoId,
            Brand = car.Brand,
            Color = car.Color.ToString(),
            Price = car.Price,
            Condition = car.Condition,
            CurrentOwner = car.CurrentOwner,
            Mileage = car.Mileage,
            IsAvailable = car.IsAvailable,
        });
        
        if (!saved)
            return ServiceResult<CarInfo>.Failure(CarErrorHelper.CarCreateCritical(carResult.Status));

        return ServiceResult<CarInfo>.Success(new CarInfo()).WithWarning(warns.ToArray());
    }

    /// <summary>
    /// Прикрепляет к машине фото и сохраняет данные фото в хранилище
    /// </summary>
    /// <returns>Id сохраненной фотографии</returns>
    private async Task<Guid?> AttachAndSaveCarPhoto(Car car, PhotoDto photoDto, User employer)
    {
        var photoId = car.AttachPhoto(photoDto.Extension, photoDto.Data, employer).PhotoId!.Value;
        
        var saved = await photoRepo.AddAsync(new PhotoDto
        {
            EntityId = photoId,
            Data = photoDto.Data,
            Extension = photoDto.Extension,
        });

        if (!saved)
            return null;
        
        return photoId;
    }
}