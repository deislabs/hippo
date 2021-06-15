using System.ComponentModel.DataAnnotations;
using Hippo.Logging;

namespace Hippo.Messages
{
    /// <summary>
    /// Request body for a new Hippo Application API.
    /// </summary>
    public class CreateApplicationRequest : ITraceable
    {
        /// <summary>
        /// The name of the ApplicationController
        /// </summary>
        /// <example>My Excellent New application</example>
        [Required]
        public string ApplicationName { get; set; }

        /// <summary>
        /// This is the ID in Bindle or whatever storage backend is used.It gets composed
        /// with a revision ID to get a Bindle ID.
        ///
        /// For example, the Weather application might have the StorageId contoso/weather.
        /// Revision 1.4.0 of the Weather application would then have the bindle id
        /// contoso/weather/1.4.0
        /// </summary>
        /// <example>contoso/weather</example>
        [Required]
        public string StorageId { get; set; }
        /// <summary>
        /// ITraceable.FormatTrace implementation.
        /// </summary>
        /// <returns>Trace striing</returns>
        public virtual string FormatTrace()
        => $"{GetType().Name}[ApplicationName={ApplicationName}, StorageId={StorageId}]";
    }
}
