using System;
using System.ComponentModel.DataAnnotations;

using Hippo.Core.Interfaces;

namespace Hippo.Core.Messages
{
    /// <summary>
    /// Response body for a new Hippo Application API.
    /// </summary>
    public class CreateApplicationResponse : ApplicationMessage, ITraceable
    {
        /// <summary>
        /// The GUID of the new Application
        /// </summary>
        /// <example>4208d635-7618-4150-b8a8-bc3205e70e32</example>
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// ITraceable.FormatTrace implementation.
        /// </summary>
        /// <returns>Trace string</returns>
        public string FormatTrace()
            => $"{GetType().Name}[ApplicationName={ApplicationName}, ApplicationGUID={Id}], StorageId={StorageId}]";
    }
}
