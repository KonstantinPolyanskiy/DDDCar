using DDDCar.Core.EntityObjects;

namespace DDDCar.Core.DtoObjects;

public record PhotoDto : IEntity
{
    public Guid EntityId { get; init; }
    
    public required byte[] Data { get; init; }
    public required string Extension { get; init; }
}