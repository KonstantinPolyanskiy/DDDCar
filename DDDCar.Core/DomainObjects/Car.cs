using DDDCar.Core.DomainObjects.Actions;
using DDDCar.Core.DomainObjects.Results;

namespace DDDCar.Core.DomainObjects;

/// <summary> Цвет машины </summary>
public enum Color
{
    Black,
    Blue,
    Green,
    White,
    Unknown
}

/// <summary> Состояние машины </summary>
public enum Condition
{
    New,
    Used,
    NotWorking
}

/// <summary> Фото машины </summary>
public sealed class Photo
{
    public string Extension { get; private set; } = null!;
    public byte[] Data { get; private set; } = null!;

    public CreatePhotoResult Create(string extension, byte[] data)
    {
        Guid photoId = Guid.NewGuid();
        
        Extension = extension;
        Data = data;
        
        return new CreatePhotoResult {Status = CreatePhotoAction.Success, Id = photoId};
    }
}

public sealed class Car
{
    public Guid ManagerId { get; private set; }
    public Photo? Photo { get; private set; }
    public string Brand { get; private set; } = null!;
    public Color Color { get; private set; }
    public decimal Price { get; private set; }
    public string? CurrentOwner { get; private set; }
    public int? Mileage { get; private set; }
    
    public bool IsAvailable { get; private set; }
    
    public Condition Condition { get; private set; }
    
    /// <summary>
    /// Создать машину. Если данные и расширение фото не указаны - машина будет создана без фото
    /// </summary>
    public CreateCarResult Create(string brand, Color color, decimal price, string? currentOwner, int? mileage, 
        User creator)
    {
        Guid carId = Guid.NewGuid();
        
        if (!HaveCreatePermissions(creator))
            return new CreateCarResult { Status = CreateCarAction.ErrorEnoughPermission };

        ManagerId = creator.Id;
        Brand = brand;
        Color = color;
        Price = price;
        CurrentOwner = currentOwner;
        Mileage = mileage;
        IsAvailable = true;
        
        Condition = CalculateCondition(currentOwner, mileage);

        return new CreateCarResult { Status = CreateCarAction.Success, CarId = carId };
    }
    
    /// <summary>
    /// Обновить данные машины, кроме ответственного менеджера и состояние (перерасчитывается на основе переданных данных)  
    /// </summary>
    public UpdateCarResult Update(string brand, Color color, decimal price, string? currentOwner, int? mileage,
        bool isAvailable, User user)
    {
        if (!HaveUpdatePermissions(user))
            return new UpdateCarResult { Status = UpdateCarAction.ErrorEnoughPermission };
        
        Brand = brand;
        Color = color;
        Price = price;
        CurrentOwner = currentOwner; 
        Mileage = mileage;
        IsAvailable = isAvailable;
        
        Condition = CalculateCondition(currentOwner, mileage);

        return new UpdateCarResult { Status = UpdateCarAction.Success };
    }

    /// <summary>
    /// Удалить машину
    /// </summary>
    public DeleteCarResult Delete(User user)
    {
        if (!HaveUpdatePermissions(user))
            return new DeleteCarResult { Status = DeleteCarAction.ErrorEnoughPermission };
        
        return new DeleteCarResult { Status = DeleteCarAction.Success };
    }
    
    /// <summary>
    /// Продать машину. Делает ее недоступной и записывает во владельцы покупателя
    /// </summary>
    public SellCarResult Sell(User buyer)
    {
        if (!IsAvailable)
            return new SellCarResult { Status = SellCarAction.ErrorAlreadySold };
        
        IsAvailable = false;
        CurrentOwner = buyer.FirstName + " " + buyer.LastName;

        return new SellCarResult() { Status = SellCarAction.Success };
    }
    
    /// <summary>
    /// Прикрепить к машине фотографию
    /// </summary>
    public AttachPhotoResult AttachPhoto(string photoExtension, byte[] photoData, User user)
    {
        if (!HaveUpdatePermissions(user))
            return new AttachPhotoResult { Status = AttachPhotoAction.ErrorEnoughPermission };
        
        Photo photo = new Photo();
        var result = photo.Create(photoExtension, photoData);
        
        Photo = photo;
        
        return new AttachPhotoResult { Status = AttachPhotoAction.Success, PhotoId = result.Id };
    }

    /// <summary>
    /// Сменить ответственного менеджера
    /// </summary>
    public ChangeManagerResult ChangeManager(User changer, Guid newManagerId)
    {
        if (!IsAdmin(changer))
            return new ChangeManagerResult { Status = ChangeManagerAction.ErrorEnoughPermission };

        var old = ManagerId;
        
        ManagerId = newManagerId;
        
        return new ChangeManagerResult { Status = ChangeManagerAction.Success, OldManager = old, NewManager = newManagerId };
    }
    
    /// <summary>
    /// Уровень доступа к машине
    /// </summary>
    public ViewLevelResult ViewLevel(User user)
    {
        if (IsAdmin(user))
            return new ViewLevelResult { Status = ViewAction.Full };
        
        if (IsResponsiveManager(user))
            return new ViewLevelResult { Status = ViewAction.Full };
        
        return new ViewLevelResult { Status = ViewAction.Restricted };
    }
    
    /// <summary>
    /// Рассчитывает состояние автомобиля
    /// </summary>
    private static Condition CalculateCondition(string? owner, int? mileage)
    {
        if (string.IsNullOrWhiteSpace(owner) && mileage is null or <= 0)
            return Condition.New;
        
        if (mileage >= 10000)
            return Condition.NotWorking;
        
        return Condition.Used;
    }
    
    /// <summary>
    ///Может ли пользователь создать машину
    /// </summary>
    private bool HaveCreatePermissions(User user)
        => user.Roles.Any(r => r is Role.Manager or Role.Admin);

    /// <summary>
    /// Может ли пользователь обновить машину
    /// </summary>
    private bool HaveUpdatePermissions(User updater)
    {
        // Администратор может все
        if (updater.Roles.Contains(Role.Admin))
            return true;
        
        // Это точно не администратор, если и не ответственный сотрудник - не может
        if (ManagerId != updater.Id)
            return false;
        
        return true;
    }
    
    /// <summary>
    /// Может ли пользователь удалить машину
    /// </summary>
    private bool HaveDeletePermissions(User user)
    {
        // Администратор может все
        if (user.Roles.Contains(Role.Admin))
            return true;
        
        // Это точно не администратор, если и не ответственный сотрудник - не может
        if (ManagerId != user.Id)
            return false;
        
        return true;
    }
    
    /// <summary>
    /// Является ли пользователь администратором
    /// </summary>
    private bool IsAdmin(User user)
     => user.Roles.Contains(Role.Admin);
    
    /// <summary>
    /// Является ли пользователь менеджером, отвественным за машину
    /// </summary>
    private bool IsResponsiveManager(User user)
        => user.Roles.Contains(Role.Manager) && ManagerId == user.Id;

}