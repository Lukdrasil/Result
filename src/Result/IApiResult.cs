using Microsoft.AspNetCore.Mvc;

namespace Mannaz.Result;

public interface IApiResult<T, TError> : IResult<T, TError>
{
    State State { get; }
    ProblemDetails ToProblemDetails();
}