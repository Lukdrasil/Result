using ElvenScript.Result;

namespace ElvenScript.Result;


public static class ResultExtensions
{

    public static Result<T2, TError> Map<T1, T2, TError>(this Result<T1, TError> result, Func<T1, T2> map) =>
        result.IsSuccess
            ? Result<T2, TError>.Success(map(result.Value))
            : Result<T2, TError>.Failure(result.Error);

    public static Result<T2, TError> Bind<T1, T2, TError>(
        this Result<T1, TError> result, Func<T1, Result<T2, TError>> bind) =>
        result.IsSuccess
            ? bind(result.Value)
            : Result<T2, TError>.Failure(result.Error);

    public static Result<T, TNewError> MapError<T, TError, TNewError>(this Result<T, TError> result, Func<TError, TNewError> map) =>
        result.IsSuccess
            ? Result<T, TNewError>.Success(result.Value)
            : Result<T, TNewError>.Failure(map(result.Error));


    public static TResult Match<T, TError, TResult>(this Result<T, TError> result, Func<T, TResult> mapValue, Func<TError, TResult> mapError) =>
        result.IsSuccess
            ? mapValue(result.Value)
            : mapError(result.Error);

    public static async Task<Result<T2, TError>> MapAsync<T1, T2, TError>(
        this Task<Result<T1, TError>> result, Func<T1, T2> map) =>
            await result.ContinueWith(t => t.Result.Map(map));

    public static async Task<Result<T2, TError>> MapAsync<T1, T2, TError>(
        this Result<T1, TError> result, Func<T1, Task<T2>> mapAsync) =>
        result.IsSuccess ? Result<T2, TError>.Success(await mapAsync(result.Value))
                         : Result<T2, TError>.Failure(result.Error);
    public static async Task<Result<T2, TError>> MapAsync<T1, T2, TError>(
        this Task<Result<T1, TError>> result, Func<T1, Task<T2>> mapAsync) =>
        await (await result).MapAsync(mapAsync);

    public static async Task<Result<T2, TError>> BindAsync<T1, T2, TError>(
        this Task<Result<T1, TError>> result, Func<T1, Result<T2, TError>> bind) =>
            await result.ContinueWith(t => t.Result.Bind(bind));

    public static async Task<Result<T2, TError>> BindAsync<T1, T2, TError>(
        this Result<T1, TError> result, Func<T1, Task<Result<T2, TError>>> bindAsync) =>
        result.IsSuccess ? await bindAsync(result.Value)
                         : Result<T2, TError>.Failure(result.Error);

    public static async Task<Result<T2, TError>> BindAsync<T1, T2, TError>(
        this Task<Result<T1, TError>> result, Func<T1, Task<Result<T2, TError>>> bindAsync) =>
        await (await result).BindAsync(bindAsync);


    public static async Task<Result<T, TNewError>> MapErrorAsync<T, TError, TNewError>(
        this Task<Result<T, TError>> result, Func<TError, TNewError> map) =>
        (await result).MapError(map);

    public static async Task<Result<T, TNewError>> MapErrorAsync<T, TError, TNewError>(
        this Result<T, TError> result, Func<TError, Task<TNewError>> mapAsync) =>
        result.IsSuccess ? Result<T, TNewError>.Success(result.Value)
                         : Result<T, TNewError>.Failure(await mapAsync(result.Error));

    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Task<Result<T, TError>> result, Func<T, TResult> onSuccess, Func<TError, TResult> onFailure) =>
        (await result).Match(onSuccess, onFailure);


    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Result<T, TError> result, Func<T, Task<TResult>> onSuccessAsync, Func<TError, TResult> onFailure) =>
        result.IsSuccess ? await onSuccessAsync(result.Value)
                         : onFailure(result.Error);

    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Result<T, TError> result, Func<T, TResult> onSuccess, Func<TError, Task<TResult>> onFailureAsync) =>
        result.IsSuccess ? onSuccess(result.Value)
                         : await onFailureAsync(result.Error);

    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Task<Result<T, TError>> result, Func<T, Task<TResult>> onSuccessAsync, Func<TError, TResult> onFailure) =>
        await (await result).MatchAsync(onSuccessAsync, onFailure);


    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Task<Result<T, TError>> result, Func<T, TResult> onSuccess, Func<TError, Task<TResult>> onFailureAsync) =>
        await (await result).MatchAsync(onSuccess, onFailureAsync);

    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Result<T, TError> result, Func<T, Task<TResult>> onSuccessAsync, Func<TError, Task<TResult>> onFailureAsync) =>
        result.IsSuccess ? await onSuccessAsync(result.Value)
                         : await onFailureAsync(result.Error);

    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Task<Result<T, TError>> result, Func<T, Task<TResult>> onSuccessAsync, Func<TError, Task<TResult>> onFailureAsync) =>
        await (await result).MatchAsync(onSuccessAsync, onFailureAsync);


}
