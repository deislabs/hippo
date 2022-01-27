namespace Hippo.Core.Entities;

public class Channel : AuditableEntity, IHasDomainEvent
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Domain { get; set; }

    public ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; set; }

    public string? RangeRule { get; set; }

    private Guid? _activeRevisionId;
    public Guid? ActiveRevisionId
    {
        get => _activeRevisionId;
        set
        {
            if (value != _activeRevisionId)
            {
                DomainEvents.Add(new ActiveRevisionChangedEvent(this));
            }

            _activeRevisionId = value;
        }
    }

    public Revision? _activeRevision;
    public Revision? ActiveRevision
    {
        get => _activeRevision;
        set
        {
            if (value != _activeRevision)
            {
                DomainEvents.Add(new ActiveRevisionChangedEvent(this));
            }

            _activeRevision = value;
        }
    }
    private Guid? _certificateId;
    public Guid? CertificateId
    {
        get => _certificateId;
        set
        {
            if (_certificateId != null && value != _certificateId)
            {
                DomainEvents.Add(new CertificateUnbindEvent(this));
            }

            if (value != null)
            {
                DomainEvents.Add(new CertificateBindEvent(this));
            }

            _certificateId = value;
        }
    }

    public Certificate? Certificate { get; set; }

    public int PortId { get; set; }

    public Guid AppId { get; set; }

    public App App { get; set; } = null!;

    public IList<EnvironmentVariable> EnvironmentVariables { get; private set; } = new List<EnvironmentVariable>();

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
