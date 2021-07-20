using Hippo.ControllerCore;
using Hippo.Logging;

namespace Hippo.Messages
{
    /// <summary>
    /// Request body for a new Hippo Application API.
    /// </summary>
    public class CreateApplicationRequest : ApplicationMessage, ITraceable, ICreateApplicationParameters
    {
        /// <summary>
        /// ITraceable.FormatTrace implementation.
        /// </summary>
        /// <returns>Trace string</returns>
        public virtual string FormatTrace()
            => $"{GetType().Name}[ApplicationName={ApplicationName}, StorageId={StorageId}]";
    }
}
