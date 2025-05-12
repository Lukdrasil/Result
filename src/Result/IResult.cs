namespace ElvenScript.Result;

public interface IResult<T, TError>
{
    TError Error { get; }
    bool IsSuccess { get; }
    T Value { get; }
}
