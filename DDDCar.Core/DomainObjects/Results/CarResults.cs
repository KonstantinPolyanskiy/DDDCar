using DDDCar.Core.DomainObjects.Actions;

namespace DDDCar.Core.DomainObjects.Results;

/// <summary> Результат создания машины </summary>
public class CreateCarResult
{
    public CreateCarAction Status { get; init; }
    
    public Guid? CarId { get; init; }
    public Guid? PhotoId { get; init; }
}

/// <summary> Результат обновления машины </summary>
public class UpdateCarResult
{
    public UpdateCarAction Status { get; init; }
}

public class DeleteCarResult
{
    public DeleteCarAction Status { get; init; }
}

public class AttachPhotoResult
{
    public AttachPhotoAction Status { get; init; }
    
    public Guid? PhotoId { get; init; }
}

public class SellCarResult
{
    public SellCarAction Status { get; init; }
}

public class ChangeManagerResult
{
    public ChangeManagerAction Status { get; init; }
    
    public Guid? OldManager { get; init; }
    public Guid? NewManager { get; init; }
}

public class ViewLevelResult
{
    public ViewAction Status { get; init; }
}