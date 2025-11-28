namespace Lukdrasil.Result;

/// <summary>
/// Provides extension methods for the <see cref="State"/> enumeration to convert states 
/// to HTTP status codes and human-readable descriptions for API responses.
/// </summary>
public static class StateExtensions
{
    /// <summary>
    /// Converts a <see cref="State"/> value to the corresponding HTTP status code.
    /// </summary>
    /// <param name="state">The state to convert.</param>
    /// <returns>
    /// An HTTP status code (200, 201, 400, 401, 403, 404, 422, 500, 503, etc.)
    /// corresponding to the provided state.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the state is not recognized.</exception>
    /// <example>
    /// <code>
    /// var state = State.Created;
    /// int statusCode = state.ToHttpStatusCode(); // Returns 201
    /// </code>
    /// </example>
    public static int ToHttpStatusCode(this State state)
    {
        var httpState = state switch
        {
            State.Ok => 200,
            State.OkWithContent => 200,
            State.Created => 201,
            State.Error => 400,
            State.Forbidden => 403,
            State.Unauthorized => 401,
            State.Invalid => 422,
            State.NotFound => 404,
            State.NoContent => 204,
            State.CriticalError => 500,
            State.Unavailable => 503,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };

        return httpState;
    }

    /// <summary>
    /// Converts a <see cref="State"/> value to a user-friendly title string suitable for error responses.
    /// </summary>
    /// <param name="state">The state to convert.</param>
    /// <returns>
    /// A descriptive title such as "Success", "Created", "Not Found", "Unauthorized", etc.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the state is not recognized.</exception>
    /// <example>
    /// <code>
    /// var state = State.NotFound;
    /// string title = state.ToTitle(); // Returns "Not Found"
    /// </code>
    /// </example>
    public static string ToTitle(this State state)
    {
        return state switch
        {
            State.Ok or State.OkWithContent => "Success",
            State.Created => "Created",
            State.Error => "Error",
            State.Forbidden => "Forbidden",
            State.Unauthorized => "Unauthorized",
            State.Invalid => "Invalid",
            State.NotFound => "Not Found",
            State.NoContent => "No Content",
            State.CriticalError => "Critical Error",
            State.Unavailable => "Unavailable",
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }

    /// <summary>
    /// Converts a <see cref="State"/> value to a detailed human-readable description 
    /// explaining what the state means. Useful for including in API problem details.
    /// </summary>
    /// <param name="state">The state to convert.</param>
    /// <returns>
    /// A descriptive message such as "Operation completed successfully.", 
    /// "The requested resource could not be found.", etc.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the state is not recognized.</exception>
    /// <example>
    /// <code>
    /// var state = State.Unauthorized;
    /// string description = state.ToDescription(); 
    /// // Returns "Authentication is required to access the resource."
    /// </code>
    /// </example>
    public static string ToDescription(this State state)
    {
        return state switch
        {
            State.Ok or State.OkWithContent => "Operation completed successfully.",
            State.Created => "Resource has been created successfully.",
            State.Error => "An error occurred during the operation.",
            State.Forbidden => "Access to the resource is forbidden.",
            State.Unauthorized => "Authentication is required to access the resource.",
            State.Invalid => "The request contains invalid data.",
            State.NotFound => "The requested resource could not be found.",
            State.NoContent => "The operation completed successfully, but there is no content to return.",
            State.CriticalError => "A critical error occurred on the server.",
            State.Unavailable => "The service is currently unavailable.",
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
}