# Lukdrasil.Result

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A modern, extensible, and strongly-typed Result type for .NET, designed for functional error handling, API responses, and robust domain logic. Includes advanced support for both value and void results, error propagation, and seamless integration with ASP.NET Core Minimal APIs.

## Features

- **Strongly-typed Result<T, TError>**: Expresses success or failure with value and error types.
- **Void Result**: Use `Result<TError>` for operations that do not return a value, with full error and state support.
- **Stateful Results**: Built-in `State` enum for HTTP and domain status (e.g., Ok, Created, Error, Forbidden, etc.).
- **Extension Methods**: Rich set of LINQ-like and async extensions for mapping, binding, error transformation, and pattern matching.
- **Minimal API Integration**: Convert results to ASP.NET Core HTTP responses with `ToHttpResult()`.
- **ProblemDetails Support**: Easily convert results to RFC 7807 ProblemDetails for API error responses.
- **Test Coverage**: Comprehensive xUnit test suite for all result types and extensions.

## Getting Started

1. **Install**
dotnet add package Lukdrasil.Result
2. **Basic Usage**
```csharp
using Lukdrasil.Result;

Result<int, string> Divide(int a, int b)
    => b == 0 ? Result<int, string>.Failure("Division by zero") : Result<int, string>.Success(a / b);

var result = Divide(10, 2)
    .Map(x => x * 2)
    .Match(
        value => $"Result: {value}",
        error => $"Error: {error}"
    );
// result == "Result: 10"
```
3. **Void Result Example**
```csharp
Result<string> DoSomething(bool ok)
    => ok ? Result<string>.Success() : Result<string>.Failure("Failed");

var result = DoSomething(true)
    .Map(_ => "Done")
    .Match(
        value => value,
        error => $"Error: {error}"
    );
// result == "Done"
```
4. **Minimal API Integration**
```csharp
app.MapGet("/resource", () => Result<string, string>.Success("Hello", State.OkWithContent).ToHttpResult());
```
## API Overview

### Core Types
- `Result<T, TError>`: Main result type for value-returning operations.
- `Result<TError>`: Void result type for operations without a value.
- `State`: Enum representing operation/API status (Ok, Created, Error, etc.).
- `ProblemDetails`: RFC 7807 error details (from ASP.NET Core).

### Extensions
- `Map`, `Bind`, `MapError`, `Match` (sync/async) for both value and void results.
- `ToHttpResult()`: Converts result to ASP.NET Core HTTP response.
- `ToProblemDetails()`: Converts result to ProblemDetails.

## Core Extension Methods

### Map
**Transforms the success value** while preserving errors. Use `Map` to apply a function to a successful result.

- **Behavior**: If successful, applies the transform function to the value; if failed, propagates error unchanged.
- **Chainable**: Combine multiple `Map` calls for fluent transformations.
- **Short-circuits**: Errors skip all subsequent operations until handled.

```csharp
var result = Result<int, string>.Success(5)
    .Map(x => x * 2)           // Success(10)
    .Map(x => x + 5)           // Success(15)
    .Map(x => $"Value: {x}");  // Success("Value: 15")

// Error propagation
var failure = Result<int, string>.Failure("error")
    .Map(x => x * 2)           // Failure("error") - short-circuits
    .Map(x => $"Value: {x}");  // Failure("error") - never executed
```

### Bind
**Chains operations that return results** for composable failure handling. Also known as "flatMap" or ">>=" in other functional languages.

- **Behavior**: If successful, executes the bind function and returns its Result; if failed, short-circuits without executing.
- **Perfect for**: Validations, database checks, API calls, and multi-step operations.
- **Prevents nesting**: Avoids deeply nested result types by flattening the operation.

```csharp
var result = Result<int, string>.Success(10)
    .Bind(x => x > 5 
        ? Result<int, string>.Success(x * 2)
        : Result<int, string>.Failure("Too small"))      // Success(20)
    .Bind(x => x < 100 
        ? Result<int, string>.Success(x + 10)
        : Result<int, string>.Failure("Too large"))      // Success(30)
    .Map(x => $"Final: {x}");                            // Success("Final: 30")

// Error short-circuits the chain
var failed = Result<int, string>.Success(2)
    .Bind(x => x > 5 
        ? Result<int, string>.Success(x * 2)
        : Result<int, string>.Failure("Too small"))      // Failure("Too small")
    .Bind(x => Result<int, string>.Success(x + 10))      // Never executed
    .Map(x => $"Result: {x}");                           // Failure("Too small")
```

### Match
**Pattern matching** to extract and handle both success and failure paths. Use at the end of a chain to produce a final value.

- **Behavior**: Applies one function to success, another to failure, both returning the same type.
- **Final operation**: Typically the last step in a result chain.
- **Flexible handling**: Both paths can be synchronous or asynchronous.

```csharp
var result = Result<int, string>.Success(10)
    .Map(x => x * 2)
    .Map(x => x + 5);

string outcome = result.Match(
    value => $"Calculated: {value}",     // For success
    error => $"Failed: {error}");        // For failure
// "Calculated: 25"

// Handling failure
var errorResult = Result<int, string>.Failure("Invalid input")
    .Map(x => x * 2)
    .Map(x => x + 5);

string errorOutcome = errorResult.Match(
    value => $"Calculated: {value}",
    error => $"Failed: {error}");
// "Failed: Invalid input"
```

### Testing
- xUnit test suite covers all result types, states, and extension methods.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contributing

Contributions, issues, and feature requests are welcome! Feel free to open an issue or submit a pull request.
