using System.Linq;
using Hippo.Models;

namespace Hippo.Repositories;

public class DbRevisionRepository : IRevisionRepository
{
    private readonly DataContext _context;

    public DbRevisionRepository(DataContext context)
    {
        _context = context;
    }

    public Revision GetRevisionByNumber(Application owner, string revisionNumber) =>
        _context.Revisions
        .Where(r => r.Application == owner && r.RevisionNumber == revisionNumber)
        .SingleOrDefault();
}
