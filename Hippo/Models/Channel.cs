using Nett;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Hippo.Models
{
    public class Channel: BaseEntity
    {
        public const int EphemeralPortRange = 32768;

        public string Name { get; set; }

        public ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; set; }
        
        public Revision SpecifiedRevision { get; set; }
        public string RangeRule { get; set; }

        public Revision ActiveRevision { get; set; }

        public Application Application { get; set; }
        public Domain Domain { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint PortID { get; set; }
        public Configuration Configuration { get; set; }
    }

    public enum ChannelRevisionSelectionStrategy
    {
        // IMPORTANT: The underlying values here are contractual with the database.
        // **DO NOT** change the underlying numeric value of any case.
        UseSpecifiedRevision = 0,
        UseRangeRule = 1,
    }
}
