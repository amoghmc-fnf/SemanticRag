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
        [EmailAddress]
        public string From { get; set; } = string.Empty;

        [VectorStoreRecordData(IsFilterable = true)]
        [EmailAddress]
        public string To { get; set; } = string.Empty;

        [VectorStoreRecordData(IsFilterable = true)]
        public DateTime Date { get; set; }

        [VectorStoreRecordData(IsFullTextSearchable = true)]
        public string Subject { get; set; }

        [VectorStoreRecordVector(Dimensions: 1536)]
        public string Body { get; set; }
    }
}
