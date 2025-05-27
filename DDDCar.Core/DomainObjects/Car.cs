namespace DDDCar.Core.DomainObjects;

/// <summary> Цвет машины </summary>
public enum Colors
{
    Black,
    Blue,
    Green,
    White,
}

/// <summary> Состояние машины </summary>
public enum Condition
{
    New,
    Used,
    NotWorking
}

public class Car
{
    public string? Brand { get; set; }
    
    public Colors? Color { get; set; }
    
    public Condition? Condition { get; set; }
}