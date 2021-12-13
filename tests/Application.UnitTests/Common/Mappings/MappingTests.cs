using System;
using System.Runtime.Serialization;
using AutoMapper;
using Hippo.Application.Apps.Queries;
using Hippo.Application.Channels.Queries;
using Hippo.Application.Common.Mappings;
using Hippo.Application.Domains.Queries;
using Hippo.Application.EnvironmentVariables.Queries;
using Hippo.Application.Revisions.Queries;
using Hippo.Core.Entities;
using Xunit;

namespace Hippo.Application.UnitTests.Common.Mappings;

public class MappingTests
{
    private readonly IConfigurationProvider _configuration;

    private readonly IMapper _mapper;

    public MappingTests()
    {
        _configuration = new MapperConfiguration(config =>
            config.AddProfile<MappingProfile>());

        _mapper = _configuration.CreateMapper();
    }

    [Fact]
    public void ShouldHaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
    }

    [Theory]
    [InlineData(typeof(App), typeof(AppDto))]
    [InlineData(typeof(App), typeof(AppRecord))]
    [InlineData(typeof(Channel), typeof(ChannelDto))]
    [InlineData(typeof(Channel), typeof(ChannelRecord))]
    [InlineData(typeof(Domain), typeof(DomainDto))]
    [InlineData(typeof(Domain), typeof(DomainRecord))]
    [InlineData(typeof(EnvironmentVariable), typeof(EnvironmentVariableDto))]
    [InlineData(typeof(EnvironmentVariable), typeof(EnvironmentVariableRecord))]
    [InlineData(typeof(Revision), typeof(RevisionDto))]
    [InlineData(typeof(Revision), typeof(RevisionRecord))]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        var instance = GetInstanceOf(source);

        _mapper.Map(instance, source, destination);
    }

    private object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;

        // Type without parameterless constructor
        return FormatterServices.GetUninitializedObject(type);
    }
}