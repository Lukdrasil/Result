using Microsoft.AspNetCore.Mvc;

namespace ElvenScript.Result;

public interface IApiResult<T, TError> : IResult<T, TError>
{
    State State { get; }
    ProblemDetails ToProblemDetails();
}