namespace Lukdrasil.Result;

public static class StateExtensions
{
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