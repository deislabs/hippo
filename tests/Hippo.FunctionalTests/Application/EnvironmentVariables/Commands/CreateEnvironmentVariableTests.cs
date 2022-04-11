using System.Threading.Tasks;
using Hippo.Application.EnvironmentVariables.Commands;
using Hippo.Application.Common.Exceptions;
using Xunit;
using System;
using Hippo.Core.Entities;
using Hippo.Core.Enums;

namespace Hippo.FunctionalTests.Application.EnvironmentVariables.Commands;

public class CreateEnvironmentVariableTests : TestBase
{
    private readonly Guid _appId;

    private readonly Guid _channelId;

    public CreateEnvironmentVariableTests()
    {
        _appId = Guid.NewGuid();
        _channelId = Guid.NewGuid();

        var app = new App
        {
            Id = _appId,
            Name = RandomString(10),
            StorageId = RandomString(10),
        };

        AddAsync(new Channel
        {
            Id = _channelId,
            AppId = _appId,
            App = app,
            Name = RandomString(10),
            Domain = RandomString(10),
        }).Wait();
    }

    [Fact]
    public async Task ShouldRequireMinimumFields()
    {
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateEnvironmentVariableCommand()));
    }

    [Theory]
    [InlineData("FOO", "bar")]
    [InlineData("FOO", "")]
    public async Task ShouldCreate(string key, string value)
    {
        var command = new CreateEnvironmentVariableCommand
        {
            Key = key,
            Value = value,
            ChannelId = _channelId
        };

        await SendAsync(command);
    }

    [Theory]
    [InlineData("", "bar")]
    public async Task ShouldNotCreate(string key, string value)
    {
        var command = new CreateEnvironmentVariableCommand
        {
            Key = key,
            Value = value,
            ChannelId = _channelId
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}
