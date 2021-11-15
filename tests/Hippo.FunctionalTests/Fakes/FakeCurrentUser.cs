using Hippo.Repositories;

namespace Hippo.FunctionalTests.Fakes;

class FakeCurrentUser : ICurrentUser
{
    private readonly string _name;

    public FakeCurrentUser(string name)
    {
        _name = name;
    }

    public string Name() => _name;
}
