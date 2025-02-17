namespace Agrivision.Backend.Application.Abstractions;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error = Error.None;

    public Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure() => new(false, Error.None);
    public static Result<TValue> Success<TValue>(TValue value) => new(true, Error.None, value);
    public static Result<TValue> Failure<TValue>(Error error) => new(false, error, default);
}

public class Result<TValue>(bool isSuccess, Error error, TValue? value) : Result(isSuccess, error)
{
    public TValue Value =>
        IsSuccess ? value! : throw new InvalidOperationException("Failure results cannot contain a value");  // throw the exception since he is trying to access the Value while IsSuccess is false
}