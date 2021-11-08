using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Hippo.Core;

namespace Hippo.Core.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1724: Type names should not match namespaces", Justification = "Allowed for Configuration.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Model Class")]
    public class Configuration : BaseEntity
    {
        [Required]
        public virtual ICollection<EnvironmentVariable> EnvironmentVariables { get; set; }
    }
}
