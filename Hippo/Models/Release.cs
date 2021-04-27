using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hippo.Models
{
    public class Release: BaseEntity
    {
        [Required]
        public string Revision { get; set; }

        [Required]
        public Build Build { get; set; }

        [Required]
        public Config Config { get; set; }

        public string WagiConfig()
        {
            var wagiConfig = new StringBuilder();
            wagiConfig.AppendLine("[[module]]");
            wagiConfig.AppendFormat("module = \"{0}\"\n", Build.UploadUrl.ToString());
            foreach (EnvironmentVariable envvar in Config.EnvironmentVariables)
            {
                wagiConfig.AppendFormat("environment.{0} = \"{1}\"\n", envvar.Key, envvar.Value);
            }
            return wagiConfig.ToString();
        }
    }
}
