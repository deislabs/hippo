using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Models
{
    public class Domain : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public Guid ChannelId { get; set; }
        public Channel Channel { get; set; }
    }
}
