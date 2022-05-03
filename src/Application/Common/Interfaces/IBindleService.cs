namespace Hippo.Application.Common.Interfaces;

public interface IBindleService
{
    public Task<IEnumerable<string>> GetBindleRevisionNumbers(string bindleId);
}
