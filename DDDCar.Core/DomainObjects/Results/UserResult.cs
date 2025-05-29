namespace DDDCar.Core.DomainObjects.Results;

public class CreateUserResult
{
    public bool IsSuccess { get; init; }
    
    public Guid Id { get; init; }
}