using DDDCar.Core.DomainObjects;
using DDDCar.Core.EntityObjects;

namespace DDDCar.Core.DtoObjects;

public record CarDto : IEntity
{
    public Guid EntityId { get; init; }
    
    public Guid? ManagerId { get; init; }
    
    public Guid? PhotoId { get; init; }

    public string? Brand { get; init; } 
    public Color? Color { get; init; } 
    public decimal? Price { get; init; }
    
    public Condition? Condition { get; init; }
    
    public string? CurrentOwner { get; init; }
    public int? Mileage { get; init; }
    
    public bool IsAvailable { get; init; }
}

public record CarUpdateDto : IEntity
{
    public required Guid EntityId { get; init; }
    
    public required string Brand { get; init; }
    public required Color Color { get; init; }
    public required decimal Price { get; init; }
    
    public string? CurrentOwner { get; init; }
    public int? Mileage { get; init; }
    
    public required bool IsAvailable { get; init; }
}
