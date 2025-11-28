using Microsoft.AspNetCore.Http;

namespace Lukdrasil.Result;

/// <summary>
/// Provides extension methods for converting <see cref="Result{T, TError}"/> instances 
/// into ASP.NET Core Minimal API <see cref="IResult"/> responses.
/// </summary>
public static partial class MinimalApiExtensions
{
    /// <summary>
    /// Converts a <see cref="Result{T, TError}"/> instance into an appropriate ASP.NET Core Minimal API response.
    /// The response type is determined by the <see cref="State"/> of the result.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="createdUri">
    /// Optional URI of the created resource. Required when the result state is <see cref="State.Created"/>.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> representing the HTTP response with appropriate status code and body.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when the result state is not a recognized or supported state.
    /// </exception>
    /// <remarks>
    /// State mappings:
    /// <list type="table">
    /// <listheader>
    ///     <term>Result State</term>
    ///     <description>HTTP Response</description>
    /// </listheader>
    /// <item>
    ///     <term><see cref="State.Ok"/></term>
    ///     <description>200 OK with no content</description>
    /// </item>
    /// <item>
    ///     <term><see cref="State.OkWithContent"/></term>
    ///     <description>200 OK with result value in body</description>
    /// </item>
    /// <item>
    ///     <term><see cref="State.Created"/></term>
    ///     <description>201 Created with Location header set to <paramref name="createdUri"/></description>
    /// </item>
    /// <item>
    ///     <term><see cref="State.NoContent"/></term>
    ///     <description>204 No Content</description>
    /// </item>
    /// <item>
    ///     <term><see cref="State.Unauthorized"/></term>
    ///     <description>401 Unauthorized</description>
    /// </item>
    /// <item>
    ///     <term><see cref="State.Forbidden"/></term>
    ///     <description>403 Forbidden</description>
    /// </item>
    /// <item>
    ///     <term><see cref="State.NotFound"/></term>
    ///     <description>404 Not Found</description>
    /// </item>
    /// <item>
    ///     <term><see cref="State.Invalid"/></term>
    ///     <description>422 Unprocessable Entity with problem details</description>
    /// </item>
    /// <item>
    ///     <term><see cref="State.Error"/></term>
    ///     <description>400 Bad Request with problem details</description>
    /// </item>
    /// <item>
    ///     <term><see cref="State.CriticalError"/></term>
    ///     <description>500 Internal Server Error with problem details</description>
    /// </item>
    /// <item>
    ///     <term><see cref="State.Unavailable"/></term>
    ///     <description>503 Service Unavailable with problem details</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// app.MapPost("/users", (UserRequest request) =>
    /// {
    ///     if (string.IsNullOrEmpty(request.Name))
    ///         return Result&lt;User, string&gt;.Failure("Name is required").ToHttpResult();
    ///     
    ///     var user = new User { Name = request.Name };
    ///     var created = Result&lt;User, string&gt;.Success(user, State.Created);
    ///     return created.ToHttpResult("/users/123");
    /// });
    /// </code>
    /// </example>
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

