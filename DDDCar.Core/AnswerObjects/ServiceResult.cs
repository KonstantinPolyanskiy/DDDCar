using System.Net;

namespace DDDCar.Core.AnswerObjects;

public enum StandardErrorType
{
    SaveError,
    UpdateError,
    DeleteError,
    
    NotFoundError,
    RequestError,
    RequestWarning,
    InternalServerError,
}

/// <summary> Различные уровни ошибок </summary>
public enum ErrorSeverity
{
    /// <summary> Предупреждение </summary>
    Warn,
    
    /// <summary> Критическая ошибка - ответ 2xx (Ok) невозможен </summary>
    Critical,
}

/// <summary>
/// Модель ошибки
/// </summary>
public sealed class AppError(Enum errorType, string message, ErrorSeverity severity, HttpStatusCode? code = null)
{
    public Enum ErrorType { get; } = errorType;
    public string Message { get; } = message;
    public ErrorSeverity Severity { get; } = severity;
    public HttpStatusCode? Code { get; } = code;
}

public sealed class ServiceResult<T>
{
    private readonly List<AppError> _errors = [];
    
    /// <summary>
    /// Результат
    /// </summary>
    public T? Value { get; set; }
    
    public AppError[] Criticals => _errors.Where(x => x.Severity == ErrorSeverity.Critical).ToArray();
    public AppError[] Warnings => _errors.Where(x => x.Severity == ErrorSeverity.Warn).ToArray();
    private ServiceResult(T? value) => Value = value;

    public ServiceResult<T> WithCritical(AppError error)
    {
        if (error.Severity is not ErrorSeverity.Critical || error.Code is null)
            throw new ArgumentException("Попытка добавить Critical ошибку с недопустимым уровнем/без названия/без HttpStatusCode");
        _errors.Add(error);
        
        return this;
    }
    
    public ServiceResult<T> WithWarning(params AppError[] warning)
    {
        foreach (var w in warning)
        {
            if (w.Severity is not ErrorSeverity.Warn      
                || w.Code is not null)
                throw new ArgumentException(
                    "Попытка добавить Warning ошибку с недопустимым уровнем или c not-null HttpStatusCod'ом");
            
            _errors.Add(w);
        }
        
        return this;
    }
    
    public bool IsSuccess => _errors.All(e => e.Severity != ErrorSeverity.Critical);

    public static ServiceResult<T> Success(T value) => new(value);
    
    public static ServiceResult<T> Failure(Enum errorType, string message, HttpStatusCode code)
    {
        var r = new ServiceResult<T>(default);
        r._errors.Add(new AppError(errorType, message, ErrorSeverity.Critical, code));

        return r;
    }
    
    public static ServiceResult<T> Failure(params AppError[] errors)
    {
        var r = new ServiceResult<T>(default);
        r._errors.AddRange(errors);

        return r;
    }
    

    
    public bool ContainsError<TEnum>(TEnum errorType) where TEnum : Enum =>
        _errors.Any(e => e.ErrorType is TEnum en && en.Equals(errorType));
    
    public bool ContainsError(Enum errorType) =>
        _errors.Any(e => e.ErrorType.Equals(errorType));
}

public readonly struct Unit : IEquatable<Unit>
{
    public static readonly Unit Value = new();
    public bool Equals(Unit other) => true;
    public override bool Equals(object? obj) => obj is Unit;
    public override int GetHashCode() => 0;
    public override string ToString() => "()";
}