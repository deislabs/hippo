using System.Text.Json;
using Hippo.Application.Apps.Queries;
using Hippo.Application.Channels.Queries;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Domains.Queries;
using Hippo.Application.EnvironmentVariables.Queries;
using Hippo.Application.Revisions.Queries;

namespace Hippo.Infrastructure.Files;

public class JsonFileBuilder : IJsonFileBuilder
{
    public byte[] BuildAppsFile(IEnumerable<AppRecord> records)
    {
        using var memoryStream = new MemoryStream();

        using (var streamWriter = new StreamWriter(memoryStream))
        {
            string jsonString = JsonSerializer.Serialize(records);
            streamWriter.WriteLine(jsonString);
        }

        return memoryStream.ToArray();
    }

    public byte[] BuildChannelsFile(IEnumerable<ChannelRecord> records)
    {
        using var memoryStream = new MemoryStream();

        using (var streamWriter = new StreamWriter(memoryStream))
        {
            string jsonString = JsonSerializer.Serialize(records);
            streamWriter.WriteLine(jsonString);
        }

        return memoryStream.ToArray();
    }

    public byte[] BuildDomainsFile(IEnumerable<DomainRecord> records)
    {
        using var memoryStream = new MemoryStream();

        using (var streamWriter = new StreamWriter(memoryStream))
        {
            string jsonString = JsonSerializer.Serialize(records);
            streamWriter.WriteLine(jsonString);
        }

        return memoryStream.ToArray();
    }

    public byte[] BuildEnvironmentVariablesFile(IEnumerable<EnvironmentVariableRecord> records)
    {
        using var memoryStream = new MemoryStream();

        using (var streamWriter = new StreamWriter(memoryStream))
        {
            string jsonString = JsonSerializer.Serialize(records);
            streamWriter.WriteLine(jsonString);
        }

        return memoryStream.ToArray();
    }

    public byte[] BuildRevisionsFile(IEnumerable<RevisionRecord> records)
    {
        using var memoryStream = new MemoryStream();

        using (var streamWriter = new StreamWriter(memoryStream))
        {
            string jsonString = JsonSerializer.Serialize(records);
            streamWriter.WriteLine(jsonString);
        }

        return memoryStream.ToArray();
    }
}
