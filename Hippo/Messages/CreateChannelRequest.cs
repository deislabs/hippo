using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hippo.Logging;
using Hippo.Models;

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
        => $"{GetType().Name}[Appid={AppId}, Name={Name}, RevisionSelectionStrategy={RevisionSelectionStrategy}, RevisionNumber={RevisionNumber}, RevisionRange={RevisionRange}]";

        /// <summary>
        /// IValidatableObject.Validate implementation.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            // TODO : Should we validate that the revision exists or if its a range there is at least one revision in the range available?

            if (RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseSpecifiedRevision)
            { 
                if (String.IsNullOrEmpty(RevisionNumber))
                {
                    yield return new ValidationResult(
                        $"Revision Number must be specified when fixing a channel to a revision number",
                        new[] { nameof(RevisionNumber) ,
                                nameof(RevisionSelectionStrategy)});
                }

                if  (!SemVer.Version.TryParse(RevisionNumber, out _))
                {
                    yield return new ValidationResult(
                        $"Revision Number does not comply with Semantic Versioning version number rules",
                        new[] { nameof(RevisionNumber) });
                }
            
            }

            if (RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseRangeRule)
            {
                if (String.IsNullOrEmpty(RevisionRange))
                {
                    yield return new ValidationResult(
                        $"Revision Range must be specified when not fixing a channel to a revision number",
                        new[] { nameof(RevisionRange),
                                nameof(RevisionSelectionStrategy)});
                }


                if (!SemVer.Range.TryParse(RevisionRange, out _))
                {
                    yield return new ValidationResult(
                        $"Revision Range does not comply with Semantic Versioning version number range rules",
                        new[] { nameof(RevisionRange) });
                }

            }

        }
    }
}
