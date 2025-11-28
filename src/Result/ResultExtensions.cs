using Lukdrasil.Result;

namespace Lukdrasil.Result;

/// <summary>
/// Provides extension methods for working with <see cref="Result{T, TError}"/> and <see cref="Result{TError}"/> types.
/// These extensions implement common functional programming patterns like Map, Bind, and Match, 
/// supporting both synchronous and asynchronous operations.
/// </summary>
public static class ResultExtensions
{
    extension<T1, T2, TError>(Result<T1, TError> result)
    {
        /// <summary>
        /// Projects the value of a successful result into a new result, transforming the value type.
        /// If the result is a failure, the error is propagated unchanged.
        /// </summary>
        /// <typeparam name="T1">The original value type.</typeparam>
        /// <typeparam name="T2">The new value type.</typeparam>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="map">A function to transform the value.</param>
        /// <returns>
        /// A new result with the transformed value if successful; 
        /// otherwise, a result with the original error.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This is the fundamental functor operation. Use it to transform the success value while preserving errors.
        /// </para>
        /// <para><strong>Fluent Chaining Example:</strong></para>
        /// <code>
        /// var result = Result&lt;int, string&gt;.Success(5)
        ///     .Map(x => x * 2)           // Success(10)
        ///     .Map(x => x + 5)           // Success(15)
        ///     .Map(x => $"Value: {x}");  // Success("Value: 15")
        /// </code>
        /// <para><strong>Key Behavior:</strong></para>
        /// <list type="bullet">
        /// <item>If successful: applies the transform function</item>
        /// <item>If failed: propagates error unchanged (short-circuits)</item>
        /// <item>Chainable: combine multiple Maps for complex transformations</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Example 1: Single transformation
        /// var result = Result&lt;int, string&gt;.Success(5);
        /// var mapped = result.Map(x => x * 2); // Success(10)
        /// 
        /// // Example 2: Chaining multiple transformations (fluent syntax)
        /// var result1 = Result&lt;int, string&gt;.Success(5);
        /// var result2 = result1
        ///     .Map(x => x * 2)           // Result(10)
        ///     .Map(x => x + 5)           // Result(15)
        ///     .Map(x => $"Value: {x}");  // Success("Value: 15")
        /// 
        /// // Example 3: Error propagation through the chain
        /// var failure = Result&lt;int, string&gt;.Failure("error");
        /// var stillFailure = failure
        ///     .Map(x => x * 2)           // Failure("error") - short-circuits
        ///     .Map(x => x + 5)           // Failure("error") - still short-circuits
        ///     .Map(x => $"Value: {x}");  // Failure("error") - never executed
        /// </code>
        /// </example>
        public Result<T2, TError> Map(Func<T1, T2> map) =>
            result.IsSuccess
                ? Result<T2, TError>.Success(map(result.Value))
                : Result<T2, TError>.Failure(result.Error);

        /// <summary>
        /// Chains operations that return results, allowing for composable failure handling.
        /// If the result is a failure, the bind function is not executed and the error is propagated.
        /// </summary>
        /// <typeparam name="T1">The original value type.</typeparam>
        /// <typeparam name="T2">The new value type.</typeparam>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="bind">A function that takes the value and returns a new result.</param>
        /// <returns>
        /// The result returned by the bind function if the source is successful; 
        /// otherwise, a result with the original error.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Also known as "flatMap" or ">>=" in other functional languages. Use this to chain operations 
        /// that themselves return Results, automatically handling short-circuiting on errors.
        /// </para>
        /// <para><strong>Fluent Chaining Example:</strong></para>
        /// <code>
        /// var result = Result&lt;int, string&gt;.Success(10)
        ///     .Bind(x => x > 5 
        ///         ? Result&lt;int, string&gt;.Success(x * 2)  // Success(20)
        ///         : Result&lt;int, string&gt;.Failure("Too small"))
        ///     .Bind(x => x &lt; 100 
        ///         ? Result&lt;int, string&gt;.Success(x + 10)  // Success(30)
        ///         : Result&lt;int, string&gt;.Failure("Too large"))
        ///     .Map(x => $"Final: {x}");  // Success("Final: 30")
        /// </code>
        /// <para><strong>Key Behavior:</strong></para>
        /// <list type="bullet">
        /// <item>If successful: executes bind function and returns its Result</item>
        /// <item>If failed: short-circuits and propagates error (bind not executed)</item>
        /// <item>Perfect for: validations, database checks, API calls</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Example 1: Single bind operation
        /// var result = Result&lt;int, string&gt;.Success(5);
        /// var bound = result.Bind(x => 
        ///     x > 0 
        ///         ? Result&lt;int, string&gt;.Success(x * 2)
        ///         : Result&lt;int, string&gt;.Failure("Value must be positive"));
        /// // Success(10)
        /// 
        /// // Example 2: Chaining multiple binds (fluent syntax)
        /// var result1 = Result&lt;int, string&gt;.Success(10);
        /// var result2 = result1
        ///     .Bind(x => 
        ///         x > 5 
        ///             ? Result&lt;int, string&gt;.Success(x * 2)
        ///             : Result&lt;int, string&gt;.Failure("Too small"))
        ///     .Bind(x => 
        ///         x &lt; 100 
        ///             ? Result&lt;int, string&gt;.Success(x + 10)
        ///             : Result&lt;int, string&gt;.Failure("Too large"))
        ///     .Map(x => $"Result: {x}");  // Success("Result: 30")
        /// 
        /// // Example 3: Error short-circuits the chain
        /// var result3 = Result&lt;int, string&gt;.Success(2);
        /// var failed = result3
        ///     .Bind(x => 
        ///         x > 5 
        ///             ? Result&lt;int, string&gt;.Success(x * 2)
        ///             : Result&lt;int, string&gt;.Failure("Too small"))  // Failure("Too small")
        ///     .Bind(x => Result&lt;int, string&gt;.Success(x + 10))      // Never executed
        ///     .Map(x => $"Result: {x}");  // Failure("Too small")
        /// </code>
        /// </example>
        public Result<T2, TError> Bind(Func<T1, Result<T2, TError>> bind) =>
            result.IsSuccess
                ? bind(result.Value)
                : Result<T2, TError>.Failure(result.Error);
    }

    extension<T, TError>(Result<T, TError> result)
    {
        /// <summary>
        /// Transforms the error type of a result using a provided function.
        /// If the result is a success, the value is preserved unchanged.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <typeparam name="TError">The original error type.</typeparam>
        /// <typeparam name="TNewError">The new error type.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="map">A function to transform the error.</param>
        /// <returns>
        /// A result with the same value if successful; 
        /// otherwise, a result with the transformed error.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Use this to convert between different error types or format errors for API responses.
        /// </para>
        /// <para><strong>Typical Use Cases:</strong></para>
        /// <code>
        /// // Convert domain error to HTTP status
        /// result.MapError(err => err.ToHttpStatusCode());
        /// 
        /// // Format error for API response
        /// result.MapError(err => new ErrorResponse { Message = err.ToString() });
        /// 
        /// // Chain error transformations
        /// result
        ///     .MapError(code => $"HTTP {code}")
        ///     .MapError(msg => $"Error: {msg}");
        /// </code>
        /// <para><strong>Key Point:</strong> Only applies to failures; success values pass through unchanged.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Example 1: Simple error transformation
        /// var result = Result&lt;int, int&gt;.Failure(404);
        /// var mapped = result.MapError(code => $"Error code: {code}");
        /// // Failure("Error code: 404")
        /// 
        /// // Example 2: Chaining MapError with other operations
        /// var result1 = Result&lt;int, int&gt;.Failure(404);
        /// var result2 = result1
        ///     .MapError(code => $"HTTP {code}")              // Failure("HTTP 404")
        ///     .MapError(msg => $"Error: {msg}");             // Failure("Error: HTTP 404")
        /// 
        /// // Example 3: Combining Map and MapError in a flow
        /// var result3 = Result&lt;int, int&gt;.Success(42)
        ///     .Map(x => x * 2)                               // Success(84)
        ///     .MapError(code => $"Failed with {code}");      // Success(84) - no error
        /// 
        /// var result4 = Result&lt;int, int&gt;.Failure(500)
        ///     .Map(x => x * 2)                               // Failure(500) - short-circuits
        ///     .MapError(code => $"Server error {code}");     // Failure("Server error 500")
        /// </code>
        /// </example>
        public Result<T, TNewError> MapError<TNewError>(Func<TError, TNewError> map) =>
            result.IsSuccess
                ? Result<T, TNewError>.Success(result.Value)
                : Result<T, TNewError>.Failure(map(result.Error));

        /// <summary>
        /// Pattern matching for results. Applies one function to a successful result and another to a failure,
        /// returning a unified result of type <typeparamref name="TResult"/>.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="mapValue">A function to handle the successful value.</param>
        /// <param name="mapError">A function to handle the error.</param>
        /// <returns>
        /// The result of <paramref name="mapValue"/> if successful; 
        /// otherwise, the result of <paramref name="mapError"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Use this at the end of a chain to extract and handle both success and failure paths.
        /// </para>
        /// <para><strong>Example - Extracting a value:</strong></para>
        /// <code>
        /// var result = Result&lt;int, string&gt;.Success(10)
        ///     .Map(x => x * 2)
        ///     .Map(x => x + 5);
        /// 
        /// string outcome = result.Match(
        ///     value => $"Calculated: {value}",      // For success
        ///     error => $"Failed: {error}");         // For failure
        /// // Returns: "Calculated: 25"
        /// </code>
        /// <para><strong>Key Behavior:</strong> Both handlers must return the same type.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Example 1: Simple pattern matching
        /// var result = Result&lt;int, string&gt;.Success(10);
        /// string message = result.Match(
        ///     value => $"Success: {value}",
        ///     error => $"Error: {error}");
        /// // "Success: 10"
        /// 
        /// // Example 2: Using Match in a fluent workflow
        /// var result1 = Result&lt;int, string&gt;.Success(5)
        ///     .Map(x => x * 2)                    // Success(10)
        ///     .Map(x => x + 5);                   // Success(15)
        /// 
        /// string outcome = result1.Match(
        ///     value => $"Calculated: {value}",
        ///     error => $"Failed: {error}");
        /// // "Calculated: 15"
        /// 
        /// // Example 3: Handling failure in Match
        /// var result2 = Result&lt;int, string&gt;.Failure("Invalid input")
        ///     .Map(x => x * 2)                    // Failure("Invalid input")
        ///     .Map(x => x + 5);                   // Failure("Invalid input")
        /// 
        /// string outcome2 = result2.Match(
        ///     value => $"Calculated: {value}",
        ///     error => $"Failed: {error}");
        /// // "Failed: Invalid input"
        /// </code>
        /// </example>
        public TResult Match<TResult>(Func<T, TResult> mapValue, Func<TError, TResult> mapError) =>
            result.IsSuccess
                ? mapValue(result.Value)
                : mapError(result.Error);

        /// <summary>
        /// Transforms the error type of a synchronous result using an asynchronous function.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <typeparam name="TError">The original error type.</typeparam>
        /// <typeparam name="TNewError">The new error type.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="mapAsync">An asynchronous function to transform the error.</param>
        /// <returns>A task representing the asynchronous transformation.</returns>
        /// <example>
        /// <code>
        /// // Example 1: Transform error from async result
        /// var errorResult = await GetResultAsync();  // Task&lt;Result&lt;int, int&gt;&gt;
        /// var mapped = errorResult.MapErrorAsync(code => $"Error: {code}");
        /// 
        /// // Example 2: Async error transformation in a chain
        /// var result = await GetUserAsync()  // Task&lt;Result&lt;User, int&gt;&gt;
        ///     .BindAsync(async user => 
        ///         await ValidateUserAsync(user))
        ///     .MapErrorAsync(async code => 
        ///         await LookupErrorMessageAsync(code))  // Transform error async
        ///     .MatchAsync(
        ///         user => $"User valid: {user.Id}",
        ///         errorMsg => $"Validation failed: {errorMsg}");
        /// </code>
        /// </example>
        public async Task<Result<T, TNewError>> MapErrorAsync<TNewError>(
    Func<TError, Task<TNewError>> mapAsync) =>
            result.IsSuccess ? Result<T, TNewError>.Success(result.Value)
                             : Result<T, TNewError>.Failure(await mapAsync(result.Error));

        /// <summary>
        /// Pattern matching on a synchronous result with an asynchronous success handler.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="onSuccessAsync">An asynchronous function to handle the successful value.</param>
        /// <param name="onFailure">A function to handle the error.</param>
        /// <returns>A task representing the pattern matching operation.</returns>
        /// <remarks>
        /// <para>
        /// Use when success requires async work but failure handling is simple.
        /// </para>
        /// <para><strong>Example:</strong></para>
        /// <code>
        /// var result = Result&lt;int, string&gt;.Success(10);
        /// var outcome = await result.MatchAsync(
        ///     async value => await SaveToDbAsync(value),
        ///     error => $"Error: {error}");
        /// </code>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Example: Async success handler
        /// var result = Result&lt;int, string&gt;.Success(10);
        /// var message = await result.MatchAsync(
        ///     async value => await LogSuccessAsync($"Value: {value}"),
        ///     error => $"Error: {error}");
        /// </code>
        /// </example>
        public async Task<TResult> MatchAsync<TResult>(
    Func<T, Task<TResult>> onSuccessAsync, Func<TError, TResult> onFailure) =>
            result.IsSuccess ? await onSuccessAsync(result.Value)
                             : onFailure(result.Error);

        /// <summary>
        /// Pattern matching on a synchronous result with an asynchronous failure handler.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="onSuccess">A function to handle the successful value.</param>
        /// <param name="onFailureAsync">An asynchronous function to handle the error.</param>
        /// <returns>A task representing the pattern matching operation.</returns>
        /// <remarks>
        /// <para>
        /// Use when failure requires async work (e.g., logging, notifications) but success is simple.
        /// </para>
        /// <para><strong>Example:</strong></para>
        /// <code>
        /// var result = Result&lt;int, string&gt;.Failure("Not found");
        /// var response = await result.MatchAsync(
        ///     value => $"Found: {value}",
        ///     async error => await NotifyAndLogAsync(error));
        /// </code>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Example: Async failure handler with error logging
        /// var result = Result&lt;int, string&gt;.Failure("Not found");
        /// var message = await result.MatchAsync(
        ///     value => $"Success: {value}",
        ///     async error => await LogErrorAndNotifyAsync(error));
        /// </code>
        /// </example>
        public async Task<TResult> MatchAsync<TResult>(
    Func<T, TResult> onSuccess, Func<TError, Task<TResult>> onFailureAsync) =>
            result.IsSuccess ? onSuccess(result.Value)
                             : await onFailureAsync(result.Error);

        /// <summary>
        /// Pattern matching on a synchronous result with both asynchronous handlers.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="onSuccessAsync">An asynchronous function to handle the successful value.</param>
        /// <param name="onFailureAsync">An asynchronous function to handle the error.</param>
        /// <returns>A task representing the pattern matching operation.</returns>
        /// <remarks>
        /// <para>
        /// The most flexible version - both success and failure paths are async. 
        /// Use when either path requires async operations.
        /// </para>
        /// <para><strong>Complete Example:</strong></para>
        /// <code>
        /// await GetDataAsync()
        ///     .BindAsync(async data => await ValidateAsync(data))
        ///     .BindAsync(async data => await TransformAsync(data))
        ///     .MatchAsync(
        ///         async value => await SaveAndNotifyAsync(value),
        ///         async error => await LogFailureAndRetryAsync(error));
        /// </code>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Example 1: Both handlers are async
        /// var result = Result&lt;int, string&gt;.Success(5);
        /// string message = await result.MatchAsync(
        ///     async value => await ProcessSuccessAsync(value),
        ///     async error => await ProcessErrorAsync(error));
        /// 
        /// // Example 2: Complex workflow combining multiple Result operations
        /// var final = await GetDataAsync()
        ///     .BindAsync(async data => await ValidateAsync(data))
        ///     .BindAsync(async data => await TransformAsync(data))
        ///     .MatchAsync(
        ///         async value => await SaveAndNotifyAsync(value),
        ///         async error => await LogFailureAndRetryAsync(error));
        /// </code>
        /// </example>
        public async Task<TResult> MatchAsync<TResult>(
    Func<T, Task<TResult>> onSuccessAsync, Func<TError, Task<TResult>> onFailureAsync) =>
            result.IsSuccess ? await onSuccessAsync(result.Value)
                             : await onFailureAsync(result.Error);
    }

    /// <summary>
    /// Projects a task result into a new task result by applying a synchronous transformation function.
    /// </summary>
    /// <typeparam name="T1">The original value type.</typeparam>
    /// <typeparam name="T2">The new value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">A task that returns the source result.</param>
    /// <param name="map">A function to transform the value.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// <para>
    /// Awaits the task and applies synchronous transformation. Use when you have an async Result 
    /// and need to transform the value without async operations.
    /// </para>
    /// <para><strong>Fluent Example:</strong></para>
    /// <code>
    /// var result = await GetUserAsync()     // Task&lt;Result&lt;User, string&gt;&gt;
    ///     .MapAsync(user => user.Id * 2)
    ///     .MapAsync(id => id + 10);
    /// </code>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example 1: Task-based result mapping
    /// Task&lt;Result&lt;int, string&gt;&gt; asyncResult = GetResultAsync();
    /// var mapped = asyncResult.MapAsync(x => x * 2);
    /// 
    /// // Example 2: Chaining async operations with fluent syntax
    /// var result = await GetUserIdAsync()  // Task&lt;Result&lt;int, string&gt;&gt;
    ///     .MapAsync(id => id * 2)           // MapAsync with sync function
    ///     .MapAsync(x => x + 10)            // Chain multiple MapAsync calls
    ///     .MatchAsync(
    ///         value => $"Final: {value}",
    ///         error => $"Error: {error}");
    /// </code>
    /// </example>
    public static async Task<Result<T2, TError>> MapAsync<T1, T2, TError>(
        this Task<Result<T1, TError>> result, Func<T1, T2> map) =>
            await result.ContinueWith(t => t.Result.Map(map));

    /// <summary>
    /// Projects a synchronous result into a new task result by applying an asynchronous transformation function.
    /// </summary>
    /// <typeparam name="T1">The original value type.</typeparam>
    /// <typeparam name="T2">The new value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The source result.</param>
    /// <param name="mapAsync">An asynchronous function to transform the value.</param>
    /// <returns>A task representing the asynchronous transformation.</returns>
    /// <remarks>
    /// <para>
    /// Use this when you have a synchronous Result and need to apply an async operation to the value.
    /// </para>
    /// <para><strong>Example - API Call:</strong></para>
    /// <code>
    /// var userId = Result&lt;int, string&gt;.Success(123);
    /// var user = await userId.MapAsync(id => FetchUserFromApiAsync(id));
    /// </code>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example 1: Async transformation on success
    /// var result = Result&lt;int, string&gt;.Success(5);
    /// var mapped = result.MapAsync(async x => await FetchDataAsync(x));
    /// 
    /// // Example 2: Chaining sync and async maps
    /// var result1 = Result&lt;int, string&gt;.Success(5);
    /// var final = await result1
    ///     .MapAsync(async x => await LookupValueAsync(x))  // Result&lt;string, string&gt;
    ///     .MatchAsync(
    ///         value => $"Result: {value}",
    ///         error => $"Error: {error}");
    /// </code>
    /// </example>
    public static async Task<Result<T2, TError>> MapAsync<T1, T2, TError>(
        this Result<T1, TError> result, Func<T1, Task<T2>> mapAsync) =>
        result.IsSuccess ? Result<T2, TError>.Success(await mapAsync(result.Value))
                         : Result<T2, TError>.Failure(result.Error);

    /// <summary>
    /// Projects a task result into a new task result by applying an asynchronous transformation function.
    /// </summary>
    /// <typeparam name="T1">The original value type.</typeparam>
    /// <typeparam name="T2">The new value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">A task that returns the source result.</param>
    /// <param name="mapAsync">An asynchronous function to transform the value.</param>
    /// <returns>A task representing the asynchronous transformation.</returns>
    public static async Task<Result<T2, TError>> MapAsync<T1, T2, TError>(
        this Task<Result<T1, TError>> result, Func<T1, Task<T2>> mapAsync) =>
        await (await result).MapAsync(mapAsync);

    /// <summary>
    /// Chains operations on a task result, allowing for composable asynchronous failure handling.
    /// If the result is a failure, the bind function is not executed.
    /// </summary>
    /// <typeparam name="T1">The original value type.</typeparam>
    /// <typeparam name="T2">The new value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">A task that returns the source result.</param>
    /// <param name="bind">A function that returns a new result.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// <para>
    /// The async counterpart to Bind. Use when chaining operations that return Task&lt;Result&gt;.
    /// Handles error short-circuiting automatically.
    /// </para>
    /// <para><strong>Typical Flow:</strong></para>
    /// <code>
    /// await GetUserAsync()
    ///     .BindAsync(user => ValidateUserAsync(user))
    ///     .BindAsync(user => UpdateDatabaseAsync(user))
    ///     .MatchAsync(
    ///         user => $"Updated: {user.Id}",
    ///         error => $"Failed: {error}");
    /// </code>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example 1: Simple async bind
    /// Task&lt;Result&lt;int, string&gt;&gt; asyncResult = GetResultAsync();
    /// var bound = asyncResult.BindAsync(x => 
    ///     Result&lt;int, string&gt;.Success(x * 2));
    /// 
    /// // Example 2: Fluent chaining of async binds with validation
    /// var result = await GetUserAsync()  // Task&lt;Result&lt;User, string&gt;&gt;
    ///     .BindAsync(user => 
    ///         ValidateUserAsync(user))    // Task&lt;Result&lt;User, string&gt;&gt;
    ///     .BindAsync(user => 
    ///         UpdateDatabaseAsync(user))  // Task&lt;Result&lt;User, string&gt;&gt;
    ///     .MatchAsync(
    ///         user => $"User saved: {user.Id}",
    ///         error => $"Operation failed: {error}");
    /// </code>
    /// </example>
    public static async Task<Result<T2, TError>> BindAsync<T1, T2, TError>(
        this Task<Result<T1, TError>> result, Func<T1, Result<T2, TError>> bind) =>
            await result.ContinueWith(t => t.Result.Bind(bind));

    /// <summary>
    /// Chains operations on a synchronous result with an asynchronous bind function.
    /// </summary>
    /// <typeparam name="T1">The original value type.</typeparam>
    /// <typeparam name="T2">The new value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The source result.</param>
    /// <param name="bindAsync">An asynchronous function that returns a new result.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// <para>
    /// Use this to combine synchronous and asynchronous operations in a single fluent chain.
    /// </para>
    /// <para><strong>Pattern:</strong></para>
    /// <code>
    /// Result&lt;int, string&gt;.Success(10)
    ///     .Bind(x => CheckLocal(x))        // Sync validation
    ///     .BindAsync(x => CheckApiAsync(x))// Async validation
    ///     .MapAsync(x => FetchDataAsync(x)); // Async operation
    /// </code>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example 1: Sync result with async bind
    /// var result = Result&lt;int, string&gt;.Success(5);
    /// var bound = result.BindAsync(async x => 
    ///     await ValidateAndProcessAsync(x));
    /// 
    /// // Example 2: Combining sync and async binds in a workflow
    /// var result1 = Result&lt;int, string&gt;.Success(10);
    /// var final = await result1
    ///     .Bind(x => 
    ///         x > 0 
    ///             ? Result&lt;int, string&gt;.Success(x * 2)
    ///             : Result&lt;int, string&gt;.Failure("Must be positive"))
    ///     .BindAsync(async x => 
    ///         await CheckQuotaAsync(x))       // Async validation
    ///     .MapAsync(async x => 
    ///         await FetchDetailsAsync(x))     // Async transformation
    ///     .MatchAsync(
    ///         value => $"Success: {value}",
    ///         error => $"Failed: {error}");
    /// </code>
    /// </example>
    public static async Task<Result<T2, TError>> BindAsync<T1, T2, TError>(
        this Result<T1, TError> result, Func<T1, Task<Result<T2, TError>>> bindAsync) =>
        result.IsSuccess ? await bindAsync(result.Value)
                         : Result<T2, TError>.Failure(result.Error);

    /// <summary>
    /// Chains operations on a task result with an asynchronous bind function.
    /// </summary>
    /// <typeparam name="T1">The original value type.</typeparam>
    /// <typeparam name="T2">The new value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">A task that returns the source result.</param>
    /// <param name="bindAsync">An asynchronous function that returns a new result.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task<Result<T2, TError>> BindAsync<T1, T2, TError>(
        this Task<Result<T1, TError>> result, Func<T1, Task<Result<T2, TError>>> bindAsync) =>
        await (await result).BindAsync(bindAsync);

    extension<T, TError>(Task<Result<T, TError>> result)
    {
        /// <summary>
        /// Transforms the error type of a task result using a synchronous function.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <typeparam name="TError">The original error type.</typeparam>
        /// <typeparam name="TNewError">The new error type.</typeparam>
        /// <param name="result">A task that returns the source result.</param>
        /// <param name="map">A function to transform the error.</param>
        /// <returns>A task representing the asynchronous transformation.</returns>
        public async Task<Result<T, TNewError>> MapErrorAsync<TNewError>(
    Func<TError, TNewError> map) =>
            (await result).MapError(map);

        /// <summary>
        /// Pattern matching on a task result with synchronous handlers.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">A task that returns the source result.</param>
        /// <param name="onSuccess">A function to handle the successful value.</param>
        /// <param name="onFailure">A function to handle the error.</param>
        /// <returns>A task representing the pattern matching operation.</returns>
        /// <remarks>
        /// <para>
        /// Use this to extract and handle both paths from an async Result chain.
        /// </para>
        /// <para><strong>Typical End-of-Chain Pattern:</strong></para>
        /// <code>
        /// var message = await GetOrderAsync()
        ///     .BindAsync(async o => await ValidateAsync(o))
        ///     .MatchAsync(
        ///         order => $"Order {order.Id} OK",
        ///         error => $"Error: {error}");
        /// </code>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Example 1: Complex async workflow with multiple transformations
        /// var result = await GetOrderAsync()     // Task&lt;Result&lt;Order, string&gt;&gt;
        ///     .BindAsync(async order => 
        ///         await ValidateOrderAsync(order))
        ///     .BindAsync(async order => 
        ///         await CheckInventoryAsync(order))
        ///     .BindAsync(async order => 
        ///         await ProcessPaymentAsync(order))
        ///     .MapAsync(async order => 
        ///         await SendConfirmationAsync(order))
        ///     .MatchAsync(
        ///         order => $"Order placed: {order.Id}",
        ///         error => $"Order failed: {error}");
        /// 
        /// // Example 2: Short-circuit on first error
        /// var result2 = await GetOrderAsync()
        ///     .BindAsync(async order => 
        ///         await ValidateOrderAsync(order))      // May fail
        ///     .BindAsync(async order => 
        ///         await CheckInventoryAsync(order))    // Not called if validation fails
        ///     .MatchAsync(
        ///         order => $"Success",
        ///         error => $"Failed: {error}");
        /// </code>
        /// </example>
        public async Task<TResult> MatchAsync<TResult>(
    Func<T, TResult> onSuccess, Func<TError, TResult> onFailure) =>
            (await result).Match(onSuccess, onFailure);

        /// <summary>
        /// Pattern matching on a task result with an asynchronous success handler.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">A task that returns the source result.</param>
        /// <param name="onSuccessAsync">An asynchronous function to handle the successful value.</param>
        /// <param name="onFailure">A function to handle the error.</param>
        /// <returns>A task representing the pattern matching operation.</returns>
        public async Task<TResult> MatchAsync<TResult>(
    Func<T, Task<TResult>> onSuccessAsync, Func<TError, TResult> onFailure) =>
            await (await result).MatchAsync(onSuccessAsync, onFailure);

        /// <summary>
        /// Pattern matching on a task result with an asynchronous failure handler.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">A task that returns the source result.</param>
        /// <param name="onSuccess">A function to handle the successful value.</param>
        /// <param name="onFailureAsync">An asynchronous function to handle the error.</param>
        /// <returns>A task representing the pattern matching operation.</returns>
        public async Task<TResult> MatchAsync<TResult>(
    Func<T, TResult> onSuccess, Func<TError, Task<TResult>> onFailureAsync) =>
            await (await result).MatchAsync(onSuccess, onFailureAsync);

        /// <summary>
        /// Pattern matching on a task result with both asynchronous handlers.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">A task that returns the source result.</param>
        /// <param name="onSuccessAsync">An asynchronous function to handle the successful value.</param>
        /// <param name="onFailureAsync">An asynchronous function to handle the error.</param>
        /// <returns>A task representing the pattern matching operation.</returns>
        public async Task<TResult> MatchAsync<TResult>(
    Func<T, Task<TResult>> onSuccessAsync, Func<TError, Task<TResult>> onFailureAsync) =>
            await (await result).MatchAsync(onSuccessAsync, onFailureAsync);
    }

    // --- Result<TError> extensions ---

    /// <summary>
    /// Projects a void result into a new result with a value type, transforming the void value.
    /// This is useful for chaining operations on results that don't carry a value.
    /// </summary>
    /// <typeparam name="T2">The new value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The source void result.</param>
    /// <param name="map">A function that transforms the void value into a new value.</param>
    /// <returns>
    /// A result with the transformed value if successful;
    /// otherwise, a result with the original error.
    /// </returns>
    /// <example>
    /// <code>
    /// // Example 1: Simple void to value transformation
    /// var voidResult = Result&lt;string&gt;.Success();
    /// var mapped = voidResult.Map(_ => 42); // Success(42)
    /// 
    /// // Example 2: Chaining operations on void result
    /// var result1 = Result&lt;string&gt;.Success();
    /// var final = result1
    ///     .Map(_ => "initialized")
    ///     .Map(x => x.ToUpper())
    ///     .Map(x => $"Status: {x}");  // Success("Status: INITIALIZED")
    /// </code>
    /// </example>
    public static Result<T2, TError> Map<T2, TError>(this Result<TError> result, Func<VoidResult, T2> map)
        => result.IsSuccess
            ? Result<T2, TError>.Success(map(result.Value), result.State)
            : Result<T2, TError>.Failure(result.Error, result.State);

    extension<TError>(Result<TError> result)
    {
        /// <summary>
        /// Chains operations on a void result, allowing for composable failure handling.
        /// If the result is a failure, the bind function is not executed.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <param name="result">The source void result.</param>
        /// <param name="bind">A function that returns a new void result.</param>
        /// <returns>
        /// The result returned by the bind function if the source is successful;
        /// otherwise, a result with the original error.
        /// </returns>
        /// <example>
        /// <code>
        /// // Example 1: Sequential void operations
        /// var result1 = Result&lt;string&gt;.Success();
        /// var result2 = result1.Bind(_ => Result&lt;string&gt;.Success());
        /// 
        /// // Example 2: Chaining void binds with validations
        /// var final = Result&lt;string&gt;.Success()
        ///     .Bind(_ => ValidateConfigAsync())   // Must execute successfully
        ///     .Bind(_ => InitializeDatabaseAsync()) // Short-circuits if previous fails
        ///     .Bind(_ => LoadServicesAsync())     // Never reached if any previous step fails
        ///     .MatchAsync(
        ///         _ => "Initialization complete",
        ///         error => $"Failed: {error}");
        /// </code>
        /// </example>
        public Result<TError> Bind(Func<VoidResult, Result<TError>> bind)
            => result.IsSuccess
                ? bind(result.Value).WithState(result.State)
                : Result<TError>.Failure(result.Error, result.State);

        /// <summary>
        /// Transforms the error type of a void result using a provided function.
        /// If the result is a success, it returns a success with a void value.
        /// </summary>
        /// <typeparam name="TError">The original error type.</typeparam>
        /// <typeparam name="TNewError">The new error type.</typeparam>
        /// <param name="result">The source void result.</param>
        /// <param name="map">A function to transform the error.</param>
        /// <returns>
        /// A result with a void value if successful;
        /// otherwise, a result with the transformed error.
        /// </returns>
        /// <example>
        /// <code>
        /// var result = Result&lt;int&gt;.Failure(404);
        /// var mapped = result.MapError(code => $"Error: {code}");
        /// // Failure("Error: 404")
        /// </code>
        /// </example>
        public Result<VoidResult, TNewError> MapError<TNewError>(Func<TError, TNewError> map)
            => result.IsSuccess
                ? Result<VoidResult, TNewError>.Success(result.Value, result.State)
                : Result<VoidResult, TNewError>.Failure(map(result.Error), result.State);

        /// <summary>
        /// Pattern matching for void results. Applies one function to a successful result and another to a failure,
        /// returning a unified result of type <typeparamref name="TResult"/>.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">The source void result.</param>
        /// <param name="onSuccess">A function to handle the successful void value.</param>
        /// <param name="onFailure">A function to handle the error.</param>
        /// <returns>
        /// The result of <paramref name="onSuccess"/> if successful;
        /// otherwise, the result of <paramref name="onFailure"/>.
        /// </returns>
        /// <example>
        /// <code>
        /// var result = Result&lt;string&gt;.Success();
        /// string message = result.Match(
        ///     _ => "Success",
        ///     error => $"Error: {error}");
        /// </code>
        /// </example>
        public TResult Match<TResult>(Func<VoidResult, TResult> onSuccess, Func<TError, TResult> onFailure)
            => result.IsSuccess
                ? onSuccess(result.Value)
                : onFailure(result.Error);

        /// <summary>
        /// Chains operations on a void result with an asynchronous bind function.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <param name="result">The source void result.</param>
        /// <param name="bindAsync">An asynchronous function that returns a new void result.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<Result<TError>> BindAsync(Func<VoidResult, Task<Result<TError>>> bindAsync)
            => result.IsSuccess ? await bindAsync(result.Value) : Result<TError>.Failure(result.Error, result.State);

        /// <summary>
        /// Transforms the error type of a void result using an asynchronous function.
        /// </summary>
        /// <typeparam name="TError">The original error type.</typeparam>
        /// <typeparam name="TNewError">The new error type.</typeparam>
        /// <param name="result">The source void result.</param>
        /// <param name="mapAsync">An asynchronous function to transform the error.</param>
        /// <returns>A task representing the asynchronous transformation.</returns>
        public async Task<Result<VoidResult, TNewError>> MapErrorAsync<TNewError>(Func<TError, Task<TNewError>> mapAsync)
            => result.IsSuccess
                ? Result<VoidResult, TNewError>.Success(result.Value, result.State)
                : Result<VoidResult, TNewError>.Failure(await mapAsync(result.Error), result.State);

        /// <summary>
        /// Pattern matching on a void result with an asynchronous success handler.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">The source void result.</param>
        /// <param name="onSuccessAsync">An asynchronous function to handle the successful void value.</param>
        /// <param name="onFailure">A function to handle the error.</param>
        /// <returns>A task representing the pattern matching operation.</returns>
        public async Task<TResult> MatchAsync<TResult>(Func<VoidResult, Task<TResult>> onSuccessAsync, Func<TError, TResult> onFailure)
            => result.IsSuccess ? await onSuccessAsync(result.Value) : onFailure(result.Error);

        /// <summary>
        /// Pattern matching on a void result with an asynchronous failure handler.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">The source void result.</param>
        /// <param name="onSuccess">A function to handle the successful void value.</param>
        /// <param name="onFailureAsync">An asynchronous function to handle the error.</param>
        /// <returns>A task representing the pattern matching operation.</returns>
        public async Task<TResult> MatchAsync<TResult>(Func<VoidResult, TResult> onSuccess, Func<TError, Task<TResult>> onFailureAsync)
            => result.IsSuccess ? onSuccess(result.Value) : await onFailureAsync(result.Error);

        /// <summary>
        /// Pattern matching on a void result with both asynchronous handlers.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">The source void result.</param>
        /// <param name="onSuccessAsync">An asynchronous function to handle the successful void value.</param>
        /// <param name="onFailureAsync">An asynchronous function to handle the error.</param>
        /// <returns>A task representing the pattern matching operation.</returns>
        public async Task<TResult> MatchAsync<TResult>(Func<VoidResult, Task<TResult>> onSuccessAsync, Func<TError, Task<TResult>> onFailureAsync)
            => result.IsSuccess ? await onSuccessAsync(result.Value) : await onFailureAsync(result.Error);
    }

    /// <summary>
    /// Projects a task of a void result into a new task result by applying a synchronous transformation function.
    /// </summary>
    /// <typeparam name="T2">The new value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">A task that returns the source void result.</param>
    /// <param name="map">A function to transform the void value.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task<Result<T2, TError>> MapAsync<T2, TError>(this Task<Result<TError>> result, Func<VoidResult, T2> map)
        => (await result).Map(map);

    /// <summary>
    /// Projects a void result into a new task result by applying an asynchronous transformation function.
    /// </summary>
    /// <typeparam name="T2">The new value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The source void result.</param>
    /// <param name="mapAsync">An asynchronous function to transform the void value.</param>
    /// <returns>A task representing the asynchronous transformation.</returns>
    public static async Task<Result<T2, TError>> MapAsync<T2, TError>(this Result<TError> result, Func<VoidResult, Task<T2>> mapAsync)
        => result.IsSuccess
            ? Result<T2, TError>.Success(await mapAsync(result.Value), result.State)
            : Result<T2, TError>.Failure(result.Error, result.State);

    /// <summary>
    /// Projects a task of a void result into a new task result by applying an asynchronous transformation function.
    /// </summary>
    /// <typeparam name="T2">The new value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">A task that returns the source void result.</param>
    /// <param name="mapAsync">An asynchronous function to transform the void value.</param>
    /// <returns>A task representing the asynchronous transformation.</returns>
    public static async Task<Result<T2, TError>> MapAsync<T2, TError>(this Task<Result<TError>> result, Func<VoidResult, Task<T2>> mapAsync)
        => await (await result).MapAsync(mapAsync);

    extension<TError>(Task<Result<TError>> result)
    {
        /// <summary>
        /// Chains operations on a task of a void result, allowing for composable failure handling.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <param name="result">A task that returns the source void result.</param>
        /// <param name="bind">A function that returns a new void result.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<Result<TError>> BindAsync(Func<VoidResult, Result<TError>> bind)
            => (await result).Bind(bind);

        /// <summary>
        /// Chains operations on a task of a void result with an asynchronous bind function.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <param name="result">A task that returns the source void result.</param>
        /// <param name="bindAsync">An asynchronous function that returns a new void result.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<Result<TError>> BindAsync(Func<VoidResult, Task<Result<TError>>> bindAsync)
            => await (await result).BindAsync(bindAsync);

        /// <summary>
        /// Transforms the error type of a task of a void result using a synchronous function.
        /// </summary>
        /// <typeparam name="TError">The original error type.</typeparam>
        /// <typeparam name="TNewError">The new error type.</typeparam>
        /// <param name="result">A task that returns the source void result.</param>
        /// <param name="map">A function to transform the error.</param>
        /// <returns>A task representing the asynchronous transformation.</returns>
        public async Task<Result<VoidResult, TNewError>> MapErrorAsync<TNewError>(Func<TError, TNewError> map)
            => (await result).MapError(map);

        /// <summary>
        /// Pattern matching on a task of a void result with synchronous handlers.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">A task that returns the source void result.</param>
        /// <param name="onSuccess">A function to handle the successful void value.</param>
        /// <param name="onFailure">A function to handle the error.</param>
        /// <returns>A task representing the pattern matching operation.</returns>
        public async Task<TResult> MatchAsync<TResult>(Func<VoidResult, TResult> onSuccess, Func<TError, TResult> onFailure)
            => (await result).Match(onSuccess, onFailure);

        /// <summary>
        /// Pattern matching on a task of a void result with an asynchronous success handler.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">A task that returns the source void result.</param>
        /// <param name="onSuccessAsync">An asynchronous function to handle the successful void value.</param>
        /// <param name="onFailure">A function to handle the error.</param>
        /// <returns>A task representing the pattern matching operation.</returns>
        public async Task<TResult> MatchAsync<TResult>(Func<VoidResult, Task<TResult>> onSuccessAsync, Func<TError, TResult> onFailure)
            => await (await result).MatchAsync(onSuccessAsync, onFailure);

        /// <summary>
        /// Pattern matching on a task of a void result with an asynchronous failure handler.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">A task that returns the source void result.</param>
        /// <param name="onSuccess">A function to handle the successful void value.</param>
        /// <param name="onFailureAsync">An asynchronous function to handle the error.</param>
        /// <returns>A task representing the pattern matching operation.</returns>
        public async Task<TResult> MatchAsync<TResult>(Func<VoidResult, TResult> onSuccess, Func<TError, Task<TResult>> onFailureAsync)
            => await (await result).MatchAsync(onSuccess, onFailureAsync);

        /// <summary>
        /// Pattern matching on a task of a void result with both asynchronous handlers.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <typeparam name="TResult">The unified result type.</typeparam>
        /// <param name="result">A task that returns the source void result.</param>
        /// <param name="onSuccessAsync">An asynchronous function to handle the successful void value.</param>
        /// <param name="onFailureAsync">An asynchronous function to handle the error.</param>
        /// <returns>A task representing the pattern matching operation.</returns>
        public async Task<TResult> MatchAsync<TResult>(Func<VoidResult, Task<TResult>> onSuccessAsync, Func<TError, Task<TResult>> onFailureAsync)
            => await (await result).MatchAsync(onSuccessAsync, onFailureAsync);
    }
}
