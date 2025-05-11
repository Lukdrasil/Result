namespace Mannaz.Result.Tests;

public class ResultExtensionsTests
{
    [Fact]
    public void Map_ShouldTransformSuccessResult()
    {
        // Arrange
        var result = Result<int, string>.Success(5);

        // Act
        var mappedResult = result.Map(x => x * 2);

        // Assert
        Assert.True(mappedResult.IsSuccess);
        Assert.Equal(10, mappedResult.Value);
    }

    [Fact]
    public void Map_ShouldPropagateFailure()
    {
        // Arrange
        var result = Result<int, string>.Failure("Error");

        // Act
        var mappedResult = result.Map(x => x * 2);

        // Assert
        Assert.False(mappedResult.IsSuccess);
        Assert.Equal("Error", mappedResult.Error);
    }

    [Fact]
    public void Bind_ShouldChainSuccessResults()
    {
        // Arrange
        var result = Result<int, string>.Success(5);

        // Act
        var boundResult = result.Bind(x => Result<int, string>.Success(x * 2));

        // Assert
        Assert.True(boundResult.IsSuccess);
        Assert.Equal(10, boundResult.Value);
    }

    [Fact]
    public void Bind_ShouldPropagateFailure()
    {
        // Arrange
        var result = Result<int, string>.Failure("Error");

        // Act
        var boundResult = result.Bind(x => Result<int, string>.Success(x * 2));

        // Assert
        Assert.False(boundResult.IsSuccess);
        Assert.Equal("Error", boundResult.Error);
    }

    [Fact]
    public void MapError_ShouldTransformError()
    {
        // Arrange
        var result = Result<int, string>.Failure("Error");

        // Act
        var mappedErrorResult = result.MapError(error => $"Mapped {error}");

        // Assert
        Assert.False(mappedErrorResult.IsSuccess);
        Assert.Equal("Mapped Error", mappedErrorResult.Error);
    }

    [Fact]
    public void MapError_ShouldPropagateSuccess()
    {
        // Arrange
        var result = Result<int, string>.Success(5);

        // Act
        var mappedErrorResult = result.MapError(error => $"Mapped {error}");

        // Assert
        Assert.True(mappedErrorResult.IsSuccess);
        Assert.Equal(5, mappedErrorResult.Value);
    }

    [Fact]
    public void Match_ShouldMapSuccess()
    {
        // Arrange
        var result = Result<int, string>.Success(5);

        // Act
        var output = result.Match(
            value => $"Success: {value}",
            error => $"Error: {error}"
        );

        // Assert
        Assert.Equal("Success: 5", output);
    }

    [Fact]
    public void Match_ShouldMapError()
    {
        // Arrange
        var result = Result<int, string>.Failure("Error");

        // Act
        var output = result.Match(
            value => $"Success: {value}",
            error => $"Error: {error}"
        );

        // Assert
        Assert.Equal("Error: Error", output);
    }

    [Fact]
    public async Task MapAsync_ShouldTransformSuccessResult()
    {
        // Arrange
        var result = Task.FromResult(Result<int, string>.Success(5));

        // Act
        var mappedResult = await result.MapAsync(x => x * 2);

        // Assert
        Assert.True(mappedResult.IsSuccess);
        Assert.Equal(10, mappedResult.Value);
    }

    [Fact]
    public async Task MapAsync_ShouldPropagateFailure()
    {
        // Arrange
        var result = Task.FromResult(Result<int, string>.Failure("Error"));

        // Act
        var mappedResult = await result.MapAsync(x => x * 2);

        // Assert
        Assert.False(mappedResult.IsSuccess);
        Assert.Equal("Error", mappedResult.Error);
    }

    [Fact]
    public async Task BindAsync_ShouldChainSuccessResults()
    {
        // Arrange
        var result = Task.FromResult(Result<int, string>.Success(5));

        // Act
        var boundResult = await result.BindAsync(x => Task.FromResult(Result<int, string>.Success(x * 2)));

        // Assert
        Assert.True(boundResult.IsSuccess);
        Assert.Equal(10, boundResult.Value);
    }

    [Fact]
    public async Task BindAsync_ShouldPropagateFailure()
    {
        // Arrange
        var result = Task.FromResult(Result<int, string>.Failure("Error"));

        // Act
        var boundResult = await result.BindAsync(x => Task.FromResult(Result<int, string>.Success(x * 2)));

        // Assert
        Assert.False(boundResult.IsSuccess);
        Assert.Equal("Error", boundResult.Error);
    }

    [Fact]
    public async Task MatchAsync_ShouldMapSuccess()
    {
        // Arrange
        var result = Task.FromResult(Result<int, string>.Success(5));

        // Act
        var output = await result.MatchAsync(
            value => Task.FromResult($"Success: {value}"),
            error => Task.FromResult($"Error: {error}")
        );

        // Assert
        Assert.Equal("Success: 5", output);
    }

    [Fact]
    public async Task MatchAsync_ShouldMapError()
    {
        // Arrange
        var result = Task.FromResult(Result<int, string>.Failure("Error"));

        // Act
        var output = await result.MatchAsync(
            value => Task.FromResult($"Success: {value}"),
            error => Task.FromResult($"Error: {error}")
        );

        // Assert
        Assert.Equal("Error: Error", output);
    }
}