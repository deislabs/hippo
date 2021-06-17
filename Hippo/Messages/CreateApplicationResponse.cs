using System;
using System.ComponentModel.DataAnnotations;
using Hippo.Logging;

namespace Hippo.Messages
{
    /// <summary>
    /// Response body for a new Hippo Application API.
    /// </summary>
    public class CreateApplicationResponse: ApplicationMessage, ITraceable
    {
        /// <summary>
        /// The name GUID of the new Application
        /// </summary>
        /// <example>4208d635-7618-4150-b8a8-bc3205e70e32</example>
        [Required]
        public Guid ApplicationGUID { get; set; }
        /// <summary>
        /// ITraceable.FormatTrace implementation.
        /// </summary>
        /// <returns>Trace string</returns>
        public string FormatTrace() 
          => $"{GetType().Name}[ApplicationName={ApplicationName}, ApplicationGUID={ApplicationGUID}], StorageId={StorageId}]";
    }
}
