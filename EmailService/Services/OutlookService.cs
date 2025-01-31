using Azure.Core.Extensions;
using EmailPlugin.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Outlook;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace EmailService.Services
{
    public class OutlookService
    {
        private const string PR_SMTP_ADDRESS = "http://schemas.microsoft.com/mapi/proptag/0x39FE001E";
        private readonly IConfiguration _configuration;
        private readonly Outlook.MAPIFolder _folder;
        public OutlookService(IConfiguration configuration)
        {
            _configuration = configuration;
            MAPIFolder inboxFolder = InitializeOutlookFolder();
            _folder = inboxFolder.Folders[_configuration["Folder"]];
        }

        private static MAPIFolder InitializeOutlookFolder()
        {
            Outlook.Application outlookApp = new Outlook.Application();
            Outlook.NameSpace outlookNamespace = outlookApp.GetNamespace("MAPI");

            // Select the Inbox folder
            Outlook.MAPIFolder inboxFolder = outlookNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
            return inboxFolder;
        }

        public async Task<List<Email>> GetMailsFromOutlook(int count)
        {
            Outlook.Items outlookMails = _folder.Items;

            // Sort by ReceivedTime from new to old 
            outlookMails.Sort("[ReceivedTime]", true);

            List<Email> emails = new List<Email>();
            for (int i = 1; i <= count && i <= outlookMails.Count; i++)
            {
                Outlook.MailItem outlookMail = outlookMails[i] as Outlook.MailItem;
                if (outlookMail != null)
                {
                    Email email = new Email();
                    email.From = GetSenderEmail(outlookMail);
                    email.To = GetRecipientEmail(outlookMail);
                    email.Subject = outlookMail.Subject;
                    email.Body = outlookMail.Body;
                    email.Date = outlookMail.ReceivedTime;
                    emails.Add(email);
                }
            }
            return emails;
        }

        private static string GetSenderEmail(MailItem outlookMail)
        {
            Outlook.PropertyAccessor senderPropertyAccessor = outlookMail.Sender.PropertyAccessor;
            return senderPropertyAccessor.GetProperty(PR_SMTP_ADDRESS).ToString();
        }

        private static string GetRecipientEmail(MailItem outlookMail)
        {
            Outlook.Recipients recipients = outlookMail.Recipients;
            Outlook.PropertyAccessor recipientPropertyAccessor = recipients[1].PropertyAccessor;
            return recipientPropertyAccessor.GetProperty(PR_SMTP_ADDRESS).ToString();
        }
    }
}
