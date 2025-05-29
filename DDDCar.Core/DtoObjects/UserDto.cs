using DDDCar.Core.EntityObjects;

namespace DDDCar.Core.DtoObjects;

public class UserDto : IEntity
{
    public Guid EntityId { get; set; }
    
    public string? Login { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
}