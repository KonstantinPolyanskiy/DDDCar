using System.Collections.Immutable;
using DDDCar.Core.DomainObjects;

namespace DDDCar.Core.AnswerObjects;

public record CarInfo
{
    public CarInfo() {}
    public CarInfo(Guid? carId, Car car)
    {
        Id = carId;
        
        Brand = car.Brand;
        Color = car.Color;
        Price = car.Price;
        Condition = car.Condition;
        IsAvailable = car.IsAvailable;
        CurrentOwner = car.CurrentOwner;
        Mileage = car.Mileage;
    }
    public Guid? Id { get; set; }
    
    public string? Brand { get; set; } 
    public Color? Color { get; set; } 
    public decimal? Price { get; set; } 
    
    public Condition? Condition { get; set; }
    public bool? IsAvailable { get; set; }
    
    public string? CurrentOwner { get; set; }
    public int? Mileage { get; set; }
    
    public UserInfo? Manager { get; set; }
    
    public UserInfo? Purchaser { get; set; }
    
    public PhotoInfo? Photo { get; set; }
}

public record UserInfo
{
    public Guid? Id { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public string? Email { get; set; }
    public string? Login  { get; set; }

    public ImmutableArray<Role>? Roles { get; set; } = [];
}

public record PhotoInfo
{
    public Guid? Id { get; set; }
    
    public string? Extension { get; set; }
    public ImmutableArray<byte>? Data { get; set; }
}

public static class CarInfoFluent
{
    public static CarInfo WithCar(this CarInfo info, Guid? carId, Car car)
    {
        info.Id = carId;
        
        info.Brand = car.Brand;
        info.Color = car.Color;
        info.Price = car.Price;
        info.Condition = car.Condition;
        info.IsAvailable = car.IsAvailable;
        info.CurrentOwner = car.CurrentOwner;
        info.Mileage = car.Mileage;
        
        return info;
    }
    
    public static CarInfo WithPhoto(this CarInfo info, Photo photo)
    {
        info.Photo = new PhotoInfo
        {
            Id = photo.Id,
            Extension = photo.Extension,
            Data = photo.Data.ToImmutableArray(),
        };
        
        return info;
    }

    public static CarInfo WithRestrictedManager(this CarInfo info, User manager)
    {
        info.Manager = new UserInfo
        {
            FirstName = manager.FirstName,
            LastName = manager.LastName,
            Email = manager.Email,
        };
        
        return info;
    }

    public static CarInfo WithFullManager(this CarInfo info, User manager)
    {
        info.Manager = new UserInfo
        {
            Id = manager.Id,
            FirstName = manager.FirstName,
            LastName = manager.LastName,
            Email = manager.Email,
            Login = manager.Login,
            Roles = manager.Roles.ToImmutableArray(),
        };
        
        return info;
    }

    public static CarInfo WithPurchaser(this CarInfo info, User purchaser)
    {
        info.Purchaser = new UserInfo
        {
            Id = purchaser.Id,
            FirstName = purchaser.FirstName,
            LastName = purchaser.LastName,
            Email = purchaser.Email,
            Login = purchaser.Login,
            Roles = purchaser.Roles.ToImmutableArray(),
        };
        
        return info;
    }
}