namespace DDDCar.Core.DomainObjects.Actions;

public enum CreateCarAction
{
    Success,
    
    ErrorEnoughPermission
}

public enum UpdateCarAction
{
    Success,
    
    ErrorEnoughPermission,
}

public enum DeleteCarAction
{
    Success,
    
    ErrorEnoughPermission,
}

public enum AttachPhotoAction
{
    Success,
    
    ErrorEnoughPermission
}

public enum SellCarAction
{
    Success,
    
    ErrorAlreadySold,
}

public enum ChangeManagerAction
{
    Success,
    ErrorEnoughPermission,
}

public enum ViewAction
{
    Full,
    Restricted,
}
