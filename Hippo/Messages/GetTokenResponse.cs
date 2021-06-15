using System;
using System.ComponentModel.DataAnnotations;
namespace Hippo.Messages
{
    /// <summary>
    /// Response body for a new Hippo Application API.
    /// </summary>
    public class GetTokenResponse
    {
        /// <summary>
        /// The JWT Bearer Token
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzaW1vbkBtZS5jb20iLCJqdGkiOiIyNGZmZDcyYS01YTgyLTRhYTctYTQzYi1mYTYzZDgxZGE3YmIiLCJ1bmlxdWVfbmFtZSI6InNpbW9uIiwiZXhwIjoxNjIzNzg4MzQ5LCJpc3MiOiJsb2NhbGhvc3QiLCJhdWQiOiJoaXBwb3Mucm9ja3MifQ.7AzNZwyuf_ee2QlwoHrlMoe_NQR3BmCuPFMoBUNxSvY</example>
        [Required]
        [DataType(DataType.Password)]
        public string Token { get; set; }
        /// <summary>
        /// The expiraton time for the token.
        /// </summary>
        /// <example>2021-06-15T19:42:05.113Z</example>
        [Required]
        public DateTime Expiration { get; set; }
    }
}
