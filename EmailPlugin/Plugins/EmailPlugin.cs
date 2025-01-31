using EmailPlugin.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailPlugin.Plugins
{
    public class EmailPlugin 
    {
        private VectorStoreTextSearch<Email> _emailVectors;
        public EmailPlugin(VectorStoreTextSearch<Email> emailVectors)
        {
            _emailVectors = emailVectors;
        }

        [KernelFunction("get_all_emails")]
        [Description("Gets all emails in the collection")]
        public async Task<List<Email>> GetAllEmails()
        {
            List<Email> emails = [];
            var query = "get all emails";
            KernelSearchResults<object> emailResults = await _emailVectors.GetSearchResultsAsync(query);
            await foreach (Email result in emailResults.Results)
            {
                Email email = new Email();
                email.From = result.From;
                email.To = result.To;
                email.Subject = result.Subject;
                email.Date = result.Date;
                email.Body = result.Body;
                emails.Add(email);
            }
            return emails;
        }
    }
}
