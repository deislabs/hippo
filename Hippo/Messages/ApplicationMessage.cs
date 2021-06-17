using System.ComponentModel.DataAnnotations;

namespace Hippo.Messages
{
    public abstract class ApplicationMessage
    {
        /// <summary>
        /// The name of the Application 
        /// </summary>
        /// <example>My Excellent Application</example>
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
    }
}
