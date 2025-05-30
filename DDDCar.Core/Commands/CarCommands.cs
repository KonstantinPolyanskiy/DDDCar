using System.ComponentModel.DataAnnotations;
using DDDCar.Core.DomainObjects;
using DDDCar.Core.DtoObjects;
using DDDCar.Core.AnswerObjects;

namespace DDDCar.Core.Commands;

/// <summary>
/// ДТО для создания машины
/// </summary>
public record CreateCarDto
{
    public  string Brand { get; private set; } = null!;
    public  Color Color { get; private set; }
    public  decimal Price { get; private set; }
    public  string? CurrentOwner { get; private set; }
    public  int? Mileage { get; private set; }
    
    public static Result<CreateCarDto> Create(CarDto d)
    {
        var dto = new CreateCarDto();
        
        if (string.IsNullOrWhiteSpace(d.Brand))
            return Result<CreateCarDto>.ModelFail(ModelError.ValueIsNull, "Brand null");
        dto.Brand = d.Brand;
        
        if (d.Color is null)
            return Result<CreateCarDto>.ModelFail(ModelError.ValueIsNull, "Color is null");
        dto.Color = (Color)d.Color;
        
        if (!d.Price.HasValue)
            return Result<CreateCarDto>.ModelFail(ModelError.ValueIsNull, "Price is null");
        dto.Price = d.Price.Value;
        
        dto.CurrentOwner = d.CurrentOwner;
        dto.Mileage = d.Mileage;


        return Result<CreateCarDto>.Ok(dto);
    }
}

public record ReconstituteCarDto
{
    
}