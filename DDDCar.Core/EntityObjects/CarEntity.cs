using System.ComponentModel.DataAnnotations;
using DDDCar.Core.DomainObjects;

namespace DDDCar.Core.EntityObjects;

public class CarEntity : IEntity
{
    [Key]
    public Guid EntityId { get; set; }
    
    public Guid? PhotoId { get; set; }
    
    public Guid? ManagerId { get; set; }
    
    public required string Brand { get; set; }
    public required string Color { get; set; }
    public required decimal Price { get; set; }
    
    public Condition Condition { get; set; }
    
    public string? CurrentOwner { get; set; }
    public int? Mileage { get; set; }
    
    public bool IsAvailable { get; set; }
}