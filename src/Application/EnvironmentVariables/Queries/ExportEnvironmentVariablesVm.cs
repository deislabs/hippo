namespace Hippo.Application.EnvironmentVariables.Queries;

public class ExportEnvironmentVariablesVm
{
    public ExportEnvironmentVariablesVm(string fileName, string contentType, byte[] content)
    {
        FileName = fileName;
        ContentType = contentType;
        Content = content;
    }

    public string FileName { get; set; }

    public string ContentType { get; set; }

    public byte[] Content { get; set; }
}
