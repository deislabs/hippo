using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Revisions.Queries;

public class ExportRevisionsVm
{
    public ExportRevisionsVm(string fileName, string contentType, byte[] content)
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
