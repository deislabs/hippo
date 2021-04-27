using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Hippo.Models
{
    public class Build: BaseEntity
    {
        [Required]
        public string UploadUrl { get; set; }

        internal string WagiConfigPath(string rootPath)
        {
            return Path.Combine(rootPath, "builds", Id.ToString(), "modules.toml");
        }
    }
}
