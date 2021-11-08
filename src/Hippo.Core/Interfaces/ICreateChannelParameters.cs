using System;
using System.Collections.Generic;
using Hippo.Core.Models;

namespace Hippo.Core.Interfaces
{
    public interface ICreateChannelParameters
    {
        Guid ApplicationId { get; }
        string ChannelName { get; }
        string DomainName { get; }
        Dictionary<string, string> EnvironmentVariables { get; }
        ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; }
        string RevisionNumber { get; }
        string RangeRule { get; }
    }
}
