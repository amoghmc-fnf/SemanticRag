using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailPlugin.Models
{
    public class Email
    {
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string From { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string To { get; set; } = string.Empty;

        public required DateTime Date { get; set; }

        public required string Subject { get; set; }

        public required string Body { get; set; }
    }
}
