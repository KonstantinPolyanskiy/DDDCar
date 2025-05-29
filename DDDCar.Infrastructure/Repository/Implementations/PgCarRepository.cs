using DDDCar.Application.Repositories;
using DDDCar.Core.AnswerObjects;
using DDDCar.Core.DomainObjects;
using DDDCar.Core.DtoObjects;
using DDDCar.Core.EntityObjects;
using DDDCar.Infrastructure.Contexts;
using Microsoft.Extensions.Logging;

namespace DDDCar.Infrastructure.Repository.Implementations;

public class PgCarRepository(ApplicationDbContext db, ILogger<PgCarRepository> log) : ICarRepository
{
    public async Task<bool> AddAsync(CarDto car)
    {
        try
        {
            var entity = new CarEntity
            {
                EntityId = car.EntityId,
                PhotoId = car.PhotoId,
                ManagerId = car.ManagerId,
                Brand = car.Brand!,
                Color = car.Color!,
                Price = (decimal)car.Price!,
                Condition = (Condition)car.Condition!,
                CurrentOwner = car.CurrentOwner,
                Mileage = car.Mileage,
                IsAvailable = car.IsAvailable,
            };
            
            await db.Cars.AddAsync(entity);
            await db.SaveChangesAsync();
            
            log.LogInformation("Машина {id} сохранена в БД", entity.EntityId);
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdateAsync(CarDto car)
    {
        try
        {
            var entity = await db.Cars.FindAsync(car.EntityId);
            if (entity == null)
                return false;

            entity.CurrentOwner = car.CurrentOwner;
            entity.Brand = car.Brand!;
            entity.Color = car.Color!;
            entity.Price = (decimal)car.Price!;
            entity.Condition = (Condition)car.Condition!;
            entity.IsAvailable = car.IsAvailable;
            entity.Mileage = car.Mileage;
            entity.ManagerId = car.ManagerId;
            entity.PhotoId = car.PhotoId;

            db.Cars.Update(entity);
            await db.SaveChangesAsync();

            log.LogInformation("Машина {id} обновлена в БД", entity.EntityId);
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid carId)
    {
        try
        {
            var entity = await db.Cars.FindAsync(carId);
            if (entity == null)
                return false;

            db.Cars.Remove(entity);
            await db.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<CarDto?> FindByIdAsync(Guid carId)
    {
        try
        {
            var entity = await db.Cars.FindAsync(carId);
            if (entity == null)
                return null;

            var result = new CarDto
            {
                EntityId = entity.EntityId,
                ManagerId = entity.ManagerId,
                PhotoId = entity.PhotoId,
                Brand = entity.Brand,
                Color = entity.Color,
                Price = entity.Price,
                Condition = entity.Condition,
                CurrentOwner = entity.CurrentOwner,
                Mileage = entity.Mileage,
                IsAvailable = entity.IsAvailable,
            };

            log.LogInformation("Машина {id} найдена в БД", entity.EntityId);
            return result;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return null;
        }
    }

    public async Task<PageResult<CarDto>> FindByParams()
    {
        throw new NotImplementedException();
    }
}