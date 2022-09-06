using Hippo.Application.Common.Interfaces;

namespace Hippo.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.UtcNow;

    public DateTime UtcNow => DateTime.UtcNow;
}
