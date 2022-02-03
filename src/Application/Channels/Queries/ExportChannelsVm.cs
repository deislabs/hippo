using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Channels.Queries;

public class ExportChannelsVm
{
    public ExportChannelsVm(string fileName, string contentType, byte[] content)
    {
        FileName = fileName;
        ContentType = contentType;
        Content = content;
    }

    [Required]
    public string FileName { get; set; }

    [Required]
    public string ContentType { get; set; }

    [Required]
    public byte[] Content { get; set; }
}
