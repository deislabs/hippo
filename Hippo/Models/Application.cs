using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Tomlyn;

namespace Hippo.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Model Class")]
    public class Application : BaseEntity
    {

        [Required]
        public string Name { get; set; }

        // This is the ID in Bindle or whatever storage backend is used.  It gets composed
        // with a revision ID to get a Bindle ID.
        //
        // For example, the Weather application might have the StorageId contoso/weather.
        // Revision 1.4.0 of the Weather application would then have the bindle id
        // contoso/weather/1.4.0
        [Required]
        public string StorageId { get; set; }

        [Required]
        public virtual Account Owner { get; set; }

        [Required]
        public virtual ICollection<Account> Collaborators { get; set; }

        [Required]

        public virtual ICollection<Revision> Revisions { get; set; }

        [Required]
        public virtual ICollection<Channel> Channels { get; set; }

        public IReadOnlyList<ActiveRevisionChange> ReevaluateActiveRevisions()
        {
            return ReevaluateActiveRevisionsLazy().ToList().AsReadOnly();
        }

        public IEnumerable<ActiveRevisionChange> ReevaluateActiveRevisionsLazy()
        {
            if (Channels == null)
            {
                yield break;
            }
            foreach (var channel in Channels)
            {
                var change = channel.ReevaluateActiveRevision();
                if (change != null)
                {
                    yield return change;
                }
            }
            // TODO: should this trigger a redeploy?
        }

        public HealthStatus Status()
        {
            if (Channels != null)
            {
                var unhealthyChannels = Channels.Select(c => new { Name = c.Name, Status = c.Status() })
                                                .Where(c => c.Status.Health == HealthLevel.Unhealthy)
                                                .ToList();
                if (unhealthyChannels.Any())
                {
                    var message = $"{unhealthyChannels.Count} channel(s) unhealthy: {string.Join(", ", unhealthyChannels.Select(c => c.Name))}";
                    return HealthStatus.Unhealthy(message);
                }
            }
            return HealthStatus.Healthy;
        }

    }
}
