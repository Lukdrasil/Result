using Microsoft.AspNetCore.Mvc;

namespace Lukdrasil.Result;

public interface IApiResult<T, TError> : IResult<T, TError>
{
    State State { get; }
    ProblemDetails ToProblemDetails();
}