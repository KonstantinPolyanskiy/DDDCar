using System.Collections.Immutable;
using DDDCar.Core.DomainObjects;

namespace DDDCar.Core.AnswerObjects;

public record CarInfo
{
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

    public ImmutableArray<Guid>? CarsIds { get; set; } = [];
}

public record PhotoInfo
{
    public Guid? Id { get; set; }
    
    public string? Extension { get; set; }
    public ImmutableArray<byte>? Data { get; set; }
}
