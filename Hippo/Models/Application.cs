using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Tomlyn;

namespace Hippo.Models
{
    public class Application: BaseEntity
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public Account Owner { get; set; }

        [Required]
        public List<Account> Collaborators { get; set; }

        [Required]
        public List<Release> Releases { get; set; }

        public List<Channel> Channels { get; set; }

        public void Start(string revision, string channelName)
        {
            var channel = Channels.Where(c => c.Name == channelName).Single();
            var release = Releases.Where(r => r.Revision == revision).Single();
            channel.Stop();
            channel.Release = release;
            channel.Start();
        }
    }
}
