using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Models
{
    public class Certificate: BaseEntity
    {

        [Required]
        public Account Owner { get; set; }

        public string Cert { get; set; }

        public string PrivateKey { get; set; }

        public string CommonName { get; set; }

        public List<string> SubjectAlternateNames { get; set; }

        public string Fingerprint { get; set; }

        public DateTime ExpiryDate { get; set; }

        public DateTime StartDate { get; set; }

        public string Issuer { get; set; }

        public string Subject { get; set; }
    }
}
