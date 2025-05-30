using AutoMapper;
using DDDCar.Application.Repositories;
using DDDCar.Core.AnswerObjects;
using DDDCar.Core.Commands;
using DDDCar.Core.DomainObjects;
using DDDCar.Core.DomainObjects.Actions;
using DDDCar.Core.DtoObjects;
using Microsoft.Extensions.Logging;
using static System.Net.HttpStatusCode;

namespace DDDCar.Application.Services;

public class CarService(ICarRepository carRepo, IPhotoRepository photoRepo, 
    IMapper map, ILogger<CarService> log)
{
    public async Task<Result<CarInfo>> AddAsync(CarDto carDto, User employer, PhotoDto? photoDto = null)
    {
        CarInfo resp = new CarInfo();

        var preparedResult = CreateCarDto.Create(carDto);
        if (!preparedResult.IsSuccess)
            return Result<CarInfo>.Fail(preparedResult.Errors.ToArray());
        
        var cd = preparedResult.Value!;

        // Создаем машину
        var car = new Car();
        
        var created = car.Create(
            cd.Brand, 
            cd.Color, 
            cd.Price, 
            cd.CurrentOwner, 
            cd.Mileage, 
            employer);

        if (created.Status is not CreateCarAction.Success)
            return Result<CarInfo>.DomainFail(created.Status, string.Empty);

        // Сохраняем машину
        if (await carRepo.AddAsync(map.Map<CarDto>(car)) is false)
            return Result<CarInfo>.AppFail(ApplicationError.SaveError, "car cant saved", InternalServerError);

        resp.WithCar(created.CarId, car)
            .WithFullManager(employer);
        
        // Создаем у машины фото если передано
        if (photoDto is not null)
        {
            // Создаем и сохраняем фото
            car.AttachPhoto(photoDto.Extension, photoDto.Data, employer);

            if (await photoRepo.AddAsync(map.Map<PhotoDto>(car.Photo)) is false)
                return Result<CarInfo>.AppWarn(resp, ApplicationError.SaveError, "photo cant saved");
            
            // Обновляем машину с новым фото
            if (await carRepo.UpdateAsync(map.Map<CarDto>(car)) is false)
                return Result<CarInfo>.AppWarn(resp, ApplicationError.UpdateError, "car dont updated with attached photo");
            
            resp.WithPhoto(car.Photo!);
        }

        return Result<CarInfo>.Ok(resp);
    }

    public async Task<Result<CarInfo>> UpdateAsync(CarUpdateDto carDto, User employer)
    {
        var carData = await carRepo.FindByIdAsync(carDto.EntityId);
        if (carData == null)
            return Result<CarInfo>.AppFail(ApplicationError.SaveError, "car not found", NotFound);
        
        var car = Car.Reconstitute(
            (Guid)carData.ManagerId!,
            carData.Brand!,
            (Color)carData.Color!,
            (decimal)carData.Price!,
            carData.CurrentOwner,
            carData.Mileage,
            (Condition)carData.Condition!,
            carData.IsAvailable);
        
        var carResult = car.Update(
            carDto.Brand,
            carDto.Color,
            carDto.Price,
            carDto.CurrentOwner,
            carDto.Mileage,
            carDto.IsAvailable,
            employer);

        if (carResult.Status is UpdateCarAction.ErrorEnoughPermission)
            return ServiceResult<CarInfo>.Failure(CarErrorHelper.CarEnoughPermissionCritical(StandardErrorType.UpdateError));

        if (await carRepo.UpdateAsync(map.Map<CarDto>(car)) is false)
            return ServiceResult<CarInfo>.Failure(CarErrorHelper.CarUpdateCritical());

        return ServiceResult<CarInfo>.Success(new CarInfo(carDto.EntityId, car));
    }

    public async Task<ServiceResult<CarInfo>> DeleteAsync(Guid carId)
    {
        
    }

    public async Task<ServiceResult<CarInfo>> SetCarPhotoAsync(Guid carId, PhotoDto photoDto, User employer)
    {
        // Получаем данные о машине из БД
        var carData = await carRepo.FindByIdAsync(carId);
        if (carData == null)
            return ServiceResult<CarInfo>.Failure(CarErrorHelper.NotFoundCarCritical(carId));

        // Восстанавливаем машину
        var car = Car.Reconstitute(
            (Guid)carData.ManagerId!,
            carData.Brand!,
            (Color)carData.Color!,
            (decimal)carData.Price!,
            carData.CurrentOwner,
            carData.Mileage,
            (Condition)carData.Condition!,
            carData.IsAvailable);
        
        // Устанавливаем фото
        if (car.AttachPhoto(photoDto.Extension, photoDto.Data, employer).Status is AttachPhotoAction.ErrorEnoughPermission)
            return ServiceResult<CarInfo>.Failure(CarErrorHelper.CarEnoughPermissionCritical(StandardErrorType.UpdateError));
        
        // Сохраняем фото 
        if (await photoRepo.AddAsync(photoDto) is false)
            return ServiceResult<CarInfo>.Failure(CarErrorHelper.UnsuccessfulSavePhotoWarn());
        
        // Обновляем машину в БД
        if (await carRepo.UpdateAsync(map.Map<CarDto>(car)) is false)
            return ServiceResult<CarInfo>.Failure(CarErrorHelper.UnsuccessfulSavePhotoWarn());

        return ServiceResult<CarInfo>.Success(new CarInfo(carId, car)
            .WithPhoto(car.Photo!)
            .WithFullManager(employer));
    }

    public async Task<ServiceResult<CarInfo>> GetCarAsync(Guid carId, User? user = null)
    {
        var warns = new List<AppError>();
        var resp = new CarInfo();
        
        // Находим в бд машину
        var carData = await carRepo.FindByIdAsync(carId);
        if (carData == null)
            return ServiceResult<CarInfo>.Failure(CarErrorHelper.NotFoundCarCritical(carId));
        
        // Восстанавливаем машину
        var car = Car.Reconstitute(
            (Guid)carData.ManagerId!,
            carData.Brand!,
            (Color)carData.Color!,
            (decimal)carData.Price!,
            carData.CurrentOwner,
            carData.Mileage,
            (Condition)carData.Condition!,
            carData.IsAvailable);

        resp.WithCar(carData.ManagerId, car);
        
        // Восстанавливаем фото машины если есть
        if (carData.PhotoId is not null)
        {
            // Находим в бд фото
            var photoData = await photoRepo.FindByIdAsync((Guid)carData.PhotoId);
            
            // Восстанавливаем если найдено
            if (photoData != null)
            {
                car.ReconstitutePhoto(photoData.Extension, photoData.Data);
                resp.WithPhoto(car.Photo!);
            } else 
                warns.Add(CarErrorHelper.NotFoundCarPhotoWarn(carData.EntityId));
        }

        switch (car.ViewLevel(user).Status)
        {
            case ViewAction.Full:
                resp.WithFullManager(user!);
                break;
            case ViewAction.Restricted:
                resp.WithRestrictedManager(user!);
                break;
            case ViewAction.Denied:
                return ServiceResult<CarInfo>.Failure(CarErrorHelper.NotFoundCarCritical(carId));
        }
        
        return ServiceResult<CarInfo>.Success(resp).WithWarning(warns.ToArray());
    }
    
}