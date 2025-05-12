namespace ElvenScript.Result.Tests;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var value = "TestValue";

        // Act
        var result = Result<string, string>.Success(value, State.Ok);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
        Assert.Equal(State.Ok, result.State);
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        // Arrange
        var error = "TestError";

        // Act
        var result = Result<string, string>.Failure(error, State.Error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
        Assert.Equal(State.Error, result.State);
    }

    [Fact]
    public void Value_ShouldThrowException_WhenResultIsFailure()
    {
        // Arrange
        var result = Result<string, string>.Failure("TestError");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }

    [Fact]
    public void Error_ShouldThrowException_WhenResultIsSuccess()
    {
        // Arrange
        var result = Result<string, string>.Success("TestValue");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _ = result.Error);
    }

    [Fact]
    public void WithState_ShouldUpdateStateCorrectly()
    {
        // Arrange
        var result = Result<string, string>.Success("TestValue");

        // Act
        var updatedResult = result.WithState(State.Created);

        // Assert
        Assert.Equal(State.Created, updatedResult.State);
    }

    [Fact]
    public void ToProblemDetails_ShouldReturnCorrectProblemDetails()
    {
        // Arrange
        var result = Result<string, string>.Failure("TestError", State.Invalid);

        // Act
        var problemDetails = result.ToProblemDetails();

        // Assert
        Assert.NotNull(problemDetails);
        Assert.Equal(State.Invalid.ToTitle(), problemDetails.Title);
        Assert.Equal(State.Invalid.ToHttpStatusCode(), problemDetails.Status);
        Assert.Equal(State.Invalid.ToDescription(), problemDetails.Detail);
    }
}