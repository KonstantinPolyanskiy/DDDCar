using System.Net;
using DDDCar.Core.AnswerObjects;
using DDDCar.Core.DomainObjects.Actions;

namespace DDDCar.Application.Helpers;

public static class CarErrorHelper
{
    public static AppError AddCarWithoutPhotoWarn()
        => new(StandardErrorType.RequestWarning,
            "У добавляемой машины отсутствует фото",
            ErrorSeverity.Warn);
    
    public static AppError UnsuccessfulSavePhotoWarn()
    => new(StandardErrorType.InternalServerError,
        "Возникла ошибка при сохранении фотографии машины",
        ErrorSeverity.Warn);

    public static AppError CarCreateCritical(CreateCarAction action)
        => new(action, "Ошибка при создании машины", ErrorSeverity.Critical, ToHttpCode(action));


    private static HttpStatusCode ToHttpCode(CreateCarAction action)
        => action switch
        {
            CreateCarAction.Success => throw new ArgumentException("Попытка создать ошибку при успешном результате от домена"),
            
            CreateCarAction.ErrorEnoughPermission => HttpStatusCode.Forbidden,
            
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };
}