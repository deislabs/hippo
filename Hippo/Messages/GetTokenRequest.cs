using System.ComponentModel.DataAnnotations;
using Hippo.Logging;

namespace Hippo.Messages
{
    /// <summary>
    /// Request body for a Get Token API.
    /// </summary>
    public class GetTokenRequest : ITraceable
    {
        /// <summary>
        /// The name of the user to get a token for,
        /// </summary>
        /// <example>myuser</example>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// The password of the user to get a token for,
        /// </summary>
        /// <example>secret</example>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// ITraceable.FormatTrace implementation.
        /// </summary>
        /// <returns>Trace striing</returns>
        public string FormatTrace() =>
            $"{nameof(GetTokenRequest)}[username={UserName}]";
    }
}
