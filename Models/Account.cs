using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Hippo.Models
{
    /// <summary>
    /// The generic IdentityUser has a lot of built in properties
    /// that can be used for identities.
    /// By using IdentityUser to inherit we are saying that we want to add on to it.
    /// </summary>
    public class Account: IdentityUser
    {

        /// <summary>
        /// Determines whether the user has administrative privileges.
        /// Used to determine whether the user can access protected
        /// endpoints like the administration portal.
        /// </summary>
        public bool IsSuperUser { get; set; }
    }
}
