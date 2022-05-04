using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Revisions.Queries;

public class StorageList
{
    public StorageList(List<string> storages)
    {
        Storages = storages;
    }

    [Required]
    public List<string> Storages { get; set; }
}
