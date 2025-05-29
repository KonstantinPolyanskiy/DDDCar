using System.ComponentModel.DataAnnotations;

namespace DDDCar.Core.EntityObjects;

public class PhotoEntity : IEntity
{
    [Key]
    public Guid EntityId { get; set; }
    
    public byte[] Data { get; set; } = null!;
    
    public string Extension { get; set; } = null!;
}