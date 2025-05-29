using DDDCar.Core.DomainObjects;
using DDDCar.Core.EntityObjects;

namespace DDDCar.Core.DtoObjects;

public record CarDto : IEntity
{
    public Guid EntityId { get; init; }
    
    public Guid? ManagerId { get; init; }
    
    public Guid? PhotoId { get; init; }

    public string? Brand { get; init; } 
    public string? Color { get; init; } 
    public decimal? Price { get; init; }
    
    public Condition? Condition { get; init; }
    
    public string? CurrentOwner { get; init; }
    public int? Mileage { get; init; }
    
    public bool IsAvailable { get; init; }
}

