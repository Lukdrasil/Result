# ElvenScript.Result

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
dotnet add package ElvenScript.Result
2. **Basic Usage**
```csharp
using ElvenScript.Result;

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

### Testing
- xUnit test suite covers all result types, states, and extension methods.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contributing

Contributions, issues, and feature requests are welcome! Feel free to open an issue or submit a pull request.
