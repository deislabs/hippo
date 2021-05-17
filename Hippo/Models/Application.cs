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
    public class Application: BaseEntity
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public virtual Account Owner { get; set; }

        [Required]
        public virtual ICollection<Account> Collaborators { get; set; }

        [Required]
        public virtual ICollection<Release> Releases { get; set; }

        [Required]
        public virtual ICollection<Channel> Channels { get; set; }
    }
}
