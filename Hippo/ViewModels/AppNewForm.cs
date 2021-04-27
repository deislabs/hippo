using System.ComponentModel.DataAnnotations;

namespace Hippo.ViewModels
{
    public class AppNewForm
    {
        [Required]
        public string Name { get; set; }
    }
}
