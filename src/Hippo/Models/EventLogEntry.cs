using System;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Models;

public class EventLogEntry : BaseEntity
{
    [Required]
    public EventKind EventKind { get; set; }

    [Required]
    public EventOrigin EventSource { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    // Use IDs rather than object references so we don't end up with FK constraints
    // that result in logs for old apps being inadvertently deleted

    public Guid? ApplicationId { get; set; }

    public Guid? ChannelId { get; set; }

    public string UserName { get; set; }

    [Required]
    public string Description { get; set; }
}

public enum EventKind
{
    // IMPORTANT: The underlying values here are contractual with the database.
    // **DO NOT** change the underlying numeric value of any case.
    //
    // No! Not even if it throws off the increasing sequence, or the alphabetical
    // order, or whatever you feel is aesthetically important.  The values are
    // a CONTRACT.  If you change them YOU WILL BREAK EVENT LOGGING.

    AccountCreated = 0,
    AccountLogin = 1,
    AccountLoginFailed = 2,
    ApplicationCreated = 3,
    ApplicationEdited = 4,
    ChannelCreated = 5,
    ChannelEdited = 6,
    ChannelRevisionChanged = 7,
    RevisionRegistered = 8,
    ChannelDeleted = 9,
}

public enum EventOrigin
{
    // IMPORTANT: The underlying values here are contractual with the database.
    // **DO NOT** change the underlying numeric value of any case.
    //
    // Go read the warning on EventKind and just don't mess with the values.

    UI = 0,
    API = 1,
}
