using Microsoft.AspNetCore.Mvc;

namespace ElvenScript.Result;

public record Result<T, TError> : IApiResult<T, TError>
{
    private T? _value;
    private TError? _error;

    public bool IsSuccess { get; }

    public T Value
    {
        get => IsSuccess ? _value! : throw new InvalidOperationException("Result is not successful");
        private set => _value = value;
    }

    public TError Error
    {
        get => !IsSuccess ? _error! : throw new InvalidOperationException("Result is successful");
        private set => _error = value;
    }
    public State State { get; init; } = State.Unknown;

    private Result(bool isSuccess, T? value, TError? error, State state) => (IsSuccess, Value, Error, State) = (isSuccess, value, error, state);
    public Result<T, TError> WithState(State state) => this with { State = state };

    public static Result<T, TError> Success(T value, State state = State.Ok) => new Result<T, TError>(true, value, default, state);

    public static Result<T, TError> Failure(TError error, State state = State.Error) => new Result<T, TError>(false, default, error, state);

    public ProblemDetails ToProblemDetails() => new ProblemDetails
    {
        Title = State.ToTitle(),
        Status = State.ToHttpStatusCode(),
        Detail = State.ToDescription()
    };
}
