using Microsoft.AspNetCore.Http.HttpResults;

namespace ElvenScript.Result.Tests;

public class MinimalApiExtensionsTests
{
    [Fact]
    public void ToHttpResult_ShouldThrowNotSupportedException_ForUnknownState()
    {
        // Arrange
        var result = Result<string, string>.Failure("Error", State.Unknown);

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => result.ToHttpResult());
    }

    [Fact]
    public void ToHttpResult_ShouldReturnOk_ForStateOk()
    {
        // Arrange
        var result = Result<string, string>.Success("Value", State.Ok);

        // Act
        var httpResult = result.ToHttpResult();

        // Assert
        Assert.IsType<Ok>(httpResult);
    }

    [Fact]
    public void ToHttpResult_ShouldReturnOkWithContent_ForStateOkWithContent()
    {
        // Arrange
        var result = Result<string, string>.Success("Value", State.OkWithContent);

        // Act
        var httpResult = result.ToHttpResult();

        // Assert
        var okResult = Assert.IsType<Ok<string>>(httpResult);
        Assert.Equal("Value", okResult.Value);
    }

    [Fact]
    public void ToHttpResult_ShouldReturnCreated_ForStateCreated()
    {
        // Arrange
        var createdUri = "http://example.com/resource";
        var result = Result<string, string>.Success("Value", State.Created);

        // Act
        var httpResult = result.ToHttpResult(createdUri);

        // Assert
        var createdResult = Assert.IsType<Created>(httpResult);
        Assert.Equal(createdUri, createdResult.Location);
    }

    [Fact]
    public void ToHttpResult_ShouldReturnProblem_ForStateError()
    {
        // Arrange
        var result = Result<string, string>.Failure("Error", State.Error);

        // Act
        var httpResult = result.ToHttpResult();

        // Assert
        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.NotNull(problemResult.ProblemDetails);
        Assert.Equal(State.Error.ToTitle(), problemResult.ProblemDetails.Title);
    }

    [Fact]
    public void ToHttpResult_ShouldReturnForbid_ForStateForbidden()
    {
        // Arrange
        var result = Result<string, string>.Failure("Error", State.Forbidden);

        // Act
        var httpResult = result.ToHttpResult();

        // Assert
        Assert.IsType<ForbidHttpResult>(httpResult);
    }

    [Fact]
    public void ToHttpResult_ShouldReturnUnauthorized_ForStateUnauthorized()
    {
        // Arrange
        var result = Result<string, string>.Failure("Error", State.Unauthorized);

        // Act
        var httpResult = result.ToHttpResult();

        // Assert
        Assert.IsType<UnauthorizedHttpResult>(httpResult);
    }

    [Fact]
    public void ToHttpResult_ShouldReturnNotFound_ForStateNotFound()
    {
        // Arrange
        var result = Result<string, string>.Failure("Error", State.NotFound);

        // Act
        var httpResult = result.ToHttpResult();

        // Assert
        Assert.IsType<NotFound>(httpResult);
    }

    [Fact]
    public void ToHttpResult_ShouldReturnNoContent_ForStateNoContent()
    {
        // Arrange
        var result = Result<string, string>.Success("Value", State.NoContent);

        // Act
        var httpResult = result.ToHttpResult();

        // Assert
        Assert.IsType<NoContent>(httpResult);
    }

    [Fact]
    public void ToHttpResult_ShouldThrowException_ForUndefinedState()
    {
        // Arrange
        var undefinedState = (State)999; // Example: Undefined state
        var result = Result<string, string>.Failure("Error", undefinedState);

        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(() => result.ToHttpResult());
        Assert.Equal($"Result {undefinedState} conversion is not supported.", exception.Message);
    }
}