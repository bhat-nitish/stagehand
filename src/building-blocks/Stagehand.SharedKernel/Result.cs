namespace Stagehand.SharedKernel;

public class Result
{
    protected Result(bool isSuccess, ResultError error)
    {
        if (isSuccess && error != ResultError.None)
        {
            throw new InvalidOperationException("A successful result cannot carry an error.");
        }

        if (!isSuccess && error == ResultError.None)
        {
            throw new InvalidOperationException("A failure result must carry an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public ResultError Error { get; }

    public static Result Success() => new(true, ResultError.None);

    public static Result Failure(ResultError error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, ResultError.None);

    public static Result<TValue> Failure<TValue>(ResultError error) => new(default, false, error);
}

public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    internal Result(TValue? value, bool isSuccess, ResultError error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result cannot be accessed.");

    public static implicit operator Result<TValue>(TValue value) => Success(value);

    public static implicit operator Result<TValue>(ResultError error) => Failure<TValue>(error);
}
