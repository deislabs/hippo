using System;
using System.ComponentModel.DataAnnotations;
using Hippo.Logging;

namespace Hippo.Messages
{
    /// <summary>
    /// Request body for Hippo Register Revision API.
    /// </summary>
    public class RegisterRevisionRequest : ITraceable
    {
        /// <summary>
        /// The GUID of the Application which the revision belongs to.
        /// </summary>
        /// <example>4208d635-7618-4150-b8a8-bc3205e70e32</example>
        public Guid? AppId { get; set; }

        /// <summary>
        /// This is the ID in Bindle or whatever storage backend is used. It gets composed
        /// with a revision ID to get a Bindle ID.
        ///
        /// For example, the Weather application might have the StorageId contoso/weather.
        /// Revision 1.4.0 of the Weather application would then have the bindle id
        /// contoso/weather/1.4.0
        /// </summary>
        /// <example>contoso/weather</example>
        public string AppStorageId { get; set; }

        /// <summary>
        /// The revision number for the new revision.
        /// </summary>
        /// <example>1.2.3</example>
        [Required]
        public string RevisionNumber { get; set; }

        /// <summary>
        /// ITraceable.FormatTrace implementation.
        /// </summary>
        /// <returns>Trace string</returns>
        public string FormatTrace()
            => $"{GetType().Name}[appid={AppId}, stgid={AppStorageId}, rev={RevisionNumber}]";
    }
}
