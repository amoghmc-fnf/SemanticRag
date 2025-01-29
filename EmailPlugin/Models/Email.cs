using Microsoft.Extensions.VectorData;
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
        [VectorStoreRecordKey]
        public int Id { get; set; }

        [VectorStoreRecordData(IsFilterable = true)]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string From { get; set; } = string.Empty;

        [VectorStoreRecordData(IsFilterable = true)]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string To { get; set; } = string.Empty;

        [VectorStoreRecordData(IsFilterable = true)]
        public required DateTime Date { get; set; }

        [VectorStoreRecordData(IsFullTextSearchable = true)]
        public required string Subject { get; set; }

        [VectorStoreRecordVector(Dimensions: 1536)]
        public required string Body { get; set; }
    }
}
