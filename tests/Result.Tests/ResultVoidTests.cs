namespace Lukdrasil.Result.Tests;

public class ResultVoidTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        // Act
        var result = Result<string>.Success(State.Ok);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(State.Ok, result.State);
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        // Arrange
        var error = "TestError";

        // Act
        var result = Result<string>.Failure(error, State.Error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
        Assert.Equal(State.Error, result.State);
    }

    [Fact]
    public void Value_ShouldThrowException_WhenResultIsFailure()
    {
        // Arrange
        var result = Result<string>.Failure("TestError");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }

    [Fact]
    public void Error_ShouldThrowException_WhenResultIsSuccess()
    {
        // Act
        var result = Result<string>.Success(State.Ok);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _ = result.Error);
    }

    [Fact]
    public void WithState_ShouldUpdateStateCorrectly()
    {
        // Arrange
        var result = Result<string>.Success(State.Ok);

        // Act
        var updatedResult = result.WithState(State.Created);

        // Assert
        Assert.Equal(State.Created, updatedResult.State);
    }

    [Fact]
    public void ToProblemDetails_ShouldReturnCorrectProblemDetails()
    {
        // Arrange
        var result = Result<string>.Failure("TestError", State.Invalid);

        // Act
        var problemDetails = result.ToProblemDetails();

        // Assert
        Assert.NotNull(problemDetails);
        Assert.Equal(State.Invalid.ToTitle(), problemDetails.Title);
        Assert.Equal(State.Invalid.ToHttpStatusCode(), problemDetails.Status);
        Assert.Equal(State.Invalid.ToDescription(), problemDetails.Detail);
    }
}