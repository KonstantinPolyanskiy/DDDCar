using System.Security.Principal;
using Microsoft.AspNetCore.Identity;

namespace DDDCar.Core.EntityObjects;

public class UserEntity : IdentityUser, IEntity
{
    public Guid EntityId { get; set; }
    
    public string? Password { get; set; }
    
    public string? RefreshToken { get; set; }
}