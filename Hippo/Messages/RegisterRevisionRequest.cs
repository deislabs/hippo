using System;
using System.ComponentModel.DataAnnotations;
using Hippo.Logging;

namespace Hippo.Messages
{
    /// <summary>
    /// Request body a new Register Revision API Request.
    /// </summary>
    public class RegisterRevisionRequest: ITraceable
    {
        /// <summary>
        /// The name GUID of the Application
        /// </summary>
        /// <example>4208d635-7618-4150-b8a8-bc3205e70e32</example>
        public Guid? AppId { get; set; }

        /// <summary>
        /// The StorageId 
        /// </summary>
        /// <example>contoso/weather</example>
        public string AppStorageId { get; set; }
        /// <summary>
        /// The  Revision number to register
        /// </summary>
        /// <example>v1.1.1</example>
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
