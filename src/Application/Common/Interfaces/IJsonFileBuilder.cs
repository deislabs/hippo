using Hippo.Application.Apps.Queries;
using Hippo.Application.Channels.Queries;
using Hippo.Application.Domains.Queries;
using Hippo.Application.EnvironmentVariables.Queries;
using Hippo.Application.Revisions.Queries;

namespace Hippo.Application.Common.Interfaces;

public interface IJsonFileBuilder
{
    byte[] BuildAppsFile(IEnumerable<AppRecord> records);

    byte[] BuildChannelsFile(IEnumerable<ChannelRecord> records);

    byte[] BuildDomainsFile(IEnumerable<DomainRecord> records);

    byte[] BuildEnvironmentVariablesFile(IEnumerable<EnvironmentVariableRecord> records);

    byte[] BuildRevisionsFile(IEnumerable<RevisionRecord> records);
}
