using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hippo.Logging;
using SemVer;

namespace Hippo.Messages
{
    /// <summary>
    /// Request body for a new ChannelMessage API Request.
    /// </summary>
    public class CreateChannelRequest : ChannelMessage, ITraceable, IValidatableObject
    {
        /// <summary>
        /// ITraceable.FormatTrace implementation.
        /// </summary>
        /// <returns>Trace striing</returns>
        public virtual string FormatTrace()
        => $"{GetType().Name}[Appid={AppId}, Name={Name}, FixedToRevision={FixedToRevision}, RevisionNumber={RevisionNumber}]";

        /// <summary>
        /// IValidatableObject.Validate implementation.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            // TODO : Should we validate that the revision exists or if its a range there is at least one revision in the range available?

            if (FixedToRevision)
            { 
                if (String.IsNullOrEmpty(RevisionNumber))
                {
                    yield return new ValidationResult(
                        $"Revision Number must be specified when fixing a channel to a revision number",
                        new[] { nameof(RevisionNumber) ,
                                nameof(FixedToRevision)});
                }

                if  (!SemVer.Version.TryParse(RevisionNumber, out _))
                {
                    yield return new ValidationResult(
                        $"Revision Number is not a valid SemVer version",
                        new[] { nameof(RevisionNumber) });
                }
            
            }

            if (!FixedToRevision)
            {
                if (String.IsNullOrEmpty(RevisionRange))
                {
                    yield return new ValidationResult(
                        $"Revision Range must be specified when not fixing a channel to a revision number",
                        new[] { nameof(RevisionRange),
                                nameof(FixedToRevision)});
                }


                if (!SemVer.Range.TryParse(RevisionRange, out _))
                {
                    yield return new ValidationResult(
                        $"Revision Range is not a valid SemVer range",
                        new[] { nameof(RevisionRange) });
                }

            }

        }
    }
}
