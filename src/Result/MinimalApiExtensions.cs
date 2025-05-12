using Microsoft.AspNetCore.Http;

namespace ElvenScript.Result;

public static partial class MinimalApiExtensions
{
    public static IResult ToHttpResult<T, TError>(this Result<T, TError> result, string? createdUri = null)
    {
        return result.State switch
        {
            State.Ok => TypedResults.Ok(),
            State.OkWithContent => TypedResults.Ok(result.Value),
            State.Created => TypedResults.Created(createdUri),
            State.Error or State.Unavailable or State.CriticalError => TypedResults.Problem(result.ToProblemDetails()),
            State.Forbidden => TypedResults.Forbid(),
            State.Unauthorized => TypedResults.Unauthorized(),
            State.NotFound => TypedResults.NotFound(),
            State.NoContent => TypedResults.NoContent(),
            _ => throw new NotSupportedException($"Result {result.State} conversion is not supported."),

        };

    }
}

