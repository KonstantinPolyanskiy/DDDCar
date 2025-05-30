using System.Net;

namespace DDDCar.Core.AnswerObjects;

public enum ModelError
{
    ValueIsNull,
}

public enum ApplicationError
{
    SaveError,
    UpdateError,
    DeleteError,
    NotFound
}

public enum ErrorKind { Domain, Application, Model }

public interface IError
{
    ErrorKind       Kind      { get; } 
    Enum            Code      { get; }   
    string          Message   { get; }
    bool            Fatal     { get; }
    HttpStatusCode? HttpCode  { get; }  
}

public sealed record Error<TCode>(
    ErrorKind       Kind,
    TCode           Code,
    string          Message,
    bool            Fatal,
    HttpStatusCode? HttpCode = null)
    : IError where TCode : Enum
{
    // Явная реализация, чтобы наружу отдать Enum
    Enum IError.Code => Code;
}


public sealed class Result<T>
{
    private readonly List<IError> _errors = new();
    public IReadOnlyList<IError> Errors => _errors;
    public T? Value { get; }
    public bool IsSuccess => !_errors.Any(e => e.Fatal);

    private Result(T? value) => Value = value;

    /* ---------- фабрики ---------- */

    public static Result<T> Ok(T value) => new(value);

    public static Result<T> Fail(IError error)
    {
        var r = new Result<T>(default);
        r._errors.Add(error);
        return r;
    }

    public static Result<T> Fail(params IError[] errors)
    {
        var r = new Result<T>(default);
        r._errors.AddRange(errors);
        return r;
    }

    public Result<T> WithWarning(IError warn)
    {
        if (warn.Fatal) throw new ArgumentException("Warning cannot be fatal");
        _errors.Add(warn);
        return this;
    }
    
    public static Result<T> ModelFail<TCode>(
        TCode code,
        string message)
        where TCode : Enum =>
        Fail(new Error<TCode>(
            ErrorKind.Model,
            code,
            message,
            Fatal: true));

    public static Result<T> DomainFail<TCode>(
        TCode code,
        string message)
        where TCode : Enum =>
        Fail(new Error<TCode>(
            ErrorKind.Domain,
            code,
            message,
            Fatal: true));

    public static Result<T> AppFail<TCode>(
        TCode code,
        string message,
        HttpStatusCode http)
        where TCode : Enum =>
        Fail(new Error<TCode>(
            ErrorKind.Application,
            code,
            message,
            Fatal: true,
            http));

    // ---------- Warning ----------
    public static Result<T> DomainWarn<TCode>(
        T value,
        TCode code,
        string message)
        where TCode : Enum =>
        Ok(value).WithWarning(
            new Error<TCode>(
                ErrorKind.Domain,
                code,
                message,
                Fatal: false));

    public static Result<T> AppWarn<TCode>(
        T value,
        TCode code,
        string message)
        where TCode : Enum =>
        Ok(value).WithWarning(
            new Error<TCode>(
                ErrorKind.Application,
                code,
                message,
                Fatal: false));
}