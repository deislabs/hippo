using Hippo.Application.Apps.Queries;
using Hippo.Application.Certificates.Queries;
using Hippo.Application.Channels.Queries;
using Hippo.Application.EnvironmentVariables.Queries;
using Hippo.Application.Revisions.Queries;

namespace Hippo.Application.Common.Interfaces;

public interface IJsonFileBuilder
{
    byte[] BuildAppsFile(IEnumerable<AppRecord> records);

    byte[] BuildCertificatesFile(IEnumerable<CertificateRecord> records);

    byte[] BuildChannelsFile(IEnumerable<ChannelRecord> records);

    byte[] BuildEnvironmentVariablesFile(IEnumerable<EnvironmentVariableRecord> records);

    byte[] BuildRevisionsFile(IEnumerable<RevisionRecord> records);
}
