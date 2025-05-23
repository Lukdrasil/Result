using System;
using System.Threading.Tasks;
using Xunit;

namespace ElvenScript.Result.Tests;

public class ResultVoidExtensionsTests
{
    [Fact]
    public void Map_ShouldTransformSuccessResult()
    {
        var result = Result<string>.Success(State.Ok);
        var mapped = result.Map(_ => 42);
        Assert.True(mapped.IsSuccess);
        Assert.Equal(42, mapped.Value);
        Assert.Equal(State.Ok, mapped.State);
    }

    [Fact]
    public void Map_ShouldPropagateFailure()
    {
        var result = Result<string>.Failure("err", State.Error);
        var mapped = result.Map(_ => 42);
        Assert.False(mapped.IsSuccess);
        Assert.Equal("err", mapped.Error);
        Assert.Equal(State.Error, mapped.State);
    }

    [Fact]
    public void Bind_ShouldChainSuccessResults()
    {
        var result = Result<string>.Success(State.Ok);
        var bound = result.Bind(_ => Result<string>.Success(State.Created));
        Assert.True(bound.IsSuccess);
        // The state should be propagated from the original result, not from the bound result
        Assert.Equal(State.Ok, bound.State);
    }

    [Fact]
    public void Bind_ShouldPropagateFailure()
    {
        var result = Result<string>.Failure("err");
        var bound = result.Bind(_ => Result<string>.Success(State.Ok));
        Assert.False(bound.IsSuccess);
        Assert.Equal("err", bound.Error);
    }

    [Fact]
    public void MapError_ShouldTransformError()
    {
        var result = Result<string>.Failure("err");
        var mapped = result.MapError(e => e.Length);
        Assert.False(mapped.IsSuccess);
        Assert.Equal(3, mapped.Error);
    }

    [Fact]
    public void MapError_ShouldPropagateSuccess()
    {
        var result = Result<string>.Success(State.Ok);
        var mapped = result.MapError(e => e.Length);
        Assert.True(mapped.IsSuccess);
        Assert.Equal(default(VoidResult), mapped.Value);
    }

    [Fact]
    public void Match_ShouldMapSuccess()
    {
        var result = Result<string>.Success(State.Ok);
        var output = result.Match(_ => "ok", err => $"fail: {err}");
        Assert.Equal("ok", output);
    }

    [Fact]
    public void Match_ShouldMapError()
    {
        var result = Result<string>.Failure("err");
        var output = result.Match(_ => "ok", err => $"fail: {err}");
        Assert.Equal("fail: err", output);
    }

    [Fact]
    public async Task MapAsync_ShouldTransformSuccessResult()
    {
        var result = Task.FromResult(Result<string>.Success(State.Ok));
        var mapped = await result.MapAsync(_ => 42);
        Assert.True(mapped.IsSuccess);
        Assert.Equal(42, mapped.Value);
    }

    [Fact]
    public async Task MapAsync_ShouldPropagateFailure()
    {
        var result = Task.FromResult(Result<string>.Failure("err"));
        var mapped = await result.MapAsync(_ => 42);
        Assert.False(mapped.IsSuccess);
        Assert.Equal("err", mapped.Error);
    }

    [Fact]
    public async Task BindAsync_ShouldChainSuccessResults()
    {
        var result = Task.FromResult(Result<string>.Success(State.Ok));
        var bound = await result.BindAsync(_ => Task.FromResult(Result<string>.Success(State.Created)));
        Assert.True(bound.IsSuccess);
        Assert.Equal(State.Created, bound.State);
    }

    [Fact]
    public async Task BindAsync_ShouldPropagateFailure()
    {
        var result = Task.FromResult(Result<string>.Failure("err"));
        var bound = await result.BindAsync(_ => Task.FromResult(Result<string>.Success(State.Ok)));
        Assert.False(bound.IsSuccess);
        Assert.Equal("err", bound.Error);
    }

    [Fact]
    public async Task MapErrorAsync_ShouldTransformError()
    {
        var result = Task.FromResult(Result<string>.Failure("err"));
        var mapped = await result.MapErrorAsync(e => e.Length);
        Assert.False(mapped.IsSuccess);
        Assert.Equal(3, mapped.Error);
    }

    [Fact]
    public async Task MapErrorAsync_ShouldPropagateSuccess()
    {
        var result = Task.FromResult(Result<string>.Success(State.Ok));
        var mapped = await result.MapErrorAsync(e => e.Length);
        Assert.True(mapped.IsSuccess);
        Assert.Equal(default(VoidResult), mapped.Value);
    }

    [Fact]
    public async Task MatchAsync_ShouldMapSuccess()
    {
        var result = Task.FromResult(Result<string>.Success(State.Ok));
        var output = await result.MatchAsync(_ => Task.FromResult("ok"), err => Task.FromResult($"fail: {err}"));
        Assert.Equal("ok", output);
    }

    [Fact]
    public async Task MatchAsync_ShouldMapError()
    {
        var result = Task.FromResult(Result<string>.Failure("err"));
        var output = await result.MatchAsync(_ => Task.FromResult("ok"), err => Task.FromResult($"fail: {err}"));
        Assert.Equal("fail: err", output);
    }

    [Fact]
    public async Task MapAsync_WithAsyncFunc_ShouldWork()
    {
        var result = Result<string>.Success(State.Ok);
        var mapped = await result.MapAsync(_ => Task.FromResult(123));
        Assert.True(mapped.IsSuccess);
        Assert.Equal(123, mapped.Value);
    }

    [Fact]
    public async Task BindAsync_WithAsyncFunc_ShouldWork()
    {
        var result = Result<string>.Success(State.Ok);
        var bound = await result.BindAsync(_ => Task.FromResult(Result<string>.Success(State.Created)));
        Assert.True(bound.IsSuccess);
        Assert.Equal(State.Created, bound.State);
    }

    [Fact]
    public async Task MapErrorAsync_WithAsyncFunc_ShouldWork()
    {
        var result = Result<string>.Failure("err");
        var mapped = await result.MapErrorAsync(e => Task.FromResult(e.Length));
        Assert.False(mapped.IsSuccess);
        Assert.Equal(3, mapped.Error);
    }

    [Fact]
    public async Task MatchAsync_WithAsyncFuncs_ShouldWork()
    {
        var result = Result<string>.Success(State.Ok);
        var output = await result.MatchAsync(_ => Task.FromResult("ok"), err => Task.FromResult($"fail: {err}"));
        Assert.Equal("ok", output);
    }

    [Fact]
    public void State_ShouldPropagateThroughExtensions()
    {
        var result = Result<string>.Failure("err", State.Invalid);
        var mapped = result.Map(_ => 1);
        Assert.Equal(State.Invalid, mapped.State);
        var mappedError = result.MapError(e => e.Length);
        Assert.Equal(State.Invalid, mappedError.State);
    }
}
