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

        public bool AutoDeploy { get; set; }

        public string VersionRange { get; set; }
        public Application Application { get; set; }
        public Domain Domain { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint PortID { get; set; }
        public Configuration Configuration { get; set; }
        public Release Release { get; set; }
    }
}
