using Azure.Core.Extensions;
using EmailPlugin.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Outlook;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace EmailService.Services
{
    public class OutlookService
    {
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

        public async Task<Email> GetAllMailsFromOutlook()
        {
            Outlook.Items outlookMails = _folder.Items;

            // Sort by ReceivedTime from new to old 
            outlookMails.Sort("[ReceivedTime]", true); 

            List<Email> emails = new List<Email>();
            for (int i = 1; i <= 10 && i <= outlookMails.Count; i++)
            {
                Outlook.MailItem outlookMail = outlookMails[i] as Outlook.MailItem;
                if (outlookMail != null)
                {
                    Email email = new Email();
                    email.From = outlookMail.SenderEmailAddress;
                    email.To = outlookMail.Rec
                }
            }
        }
    }
}
