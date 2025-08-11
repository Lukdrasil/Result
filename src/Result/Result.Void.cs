using Microsoft.AspNetCore.Mvc;

namespace Lukdrasil.Result;


public record Result<TError> : IApiResult<VoidResult, TError>
{
    private VoidResult _value;
    private TError? _error;

    public bool IsSuccess { get; }

    public VoidResult Value
    {
        get => IsSuccess ? _value : throw new InvalidOperationException("Result is not successful");
        private set => _value = value;
    }

    public TError Error
    {
        get => !IsSuccess ? _error! : throw new InvalidOperationException("Result is successful");
        private set => _error = value;
    }
    public State State { get; init; } = State.Unknown;

    private Result(bool isSuccess, TError? error, State state) => (IsSuccess, Error, State) = (isSuccess, error, state);
    public Result<TError> WithState(State state) => this with { State = state };

    public static Result<TError> Success(State state = State.Ok) => new Result<TError>(true, default, state);

    public static Result<TError> Failure(TError error, State state = State.Error) => new Result<TError>(false, error, state);

    public ProblemDetails ToProblemDetails() => new ProblemDetails
    {
        Title = State.ToTitle(),
        Status = State.ToHttpStatusCode(),
        Detail = State.ToDescription()
    };
}