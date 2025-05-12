namespace ElvenScript.Result;

public enum State
{
    Unknown,
    Ok,
    OkWithContent,
    Created,
    Error,
    Forbidden,
    Unauthorized,
    Invalid,
    NotFound,
    NoContent,
    CriticalError,
    Unavailable
}
