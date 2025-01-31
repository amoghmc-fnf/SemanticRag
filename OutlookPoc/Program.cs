using Microsoft.Office.Interop.Outlook;
using System;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OutlookPoc
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Outlook.Application outlookApp = new Outlook.Application();
            Outlook.NameSpace outlookNamespace = outlookApp.GetNamespace("MAPI");
            // Select the Inbox folder
            Outlook.MAPIFolder inboxFolder = outlookNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);

            // Select a subfolder (replace "SubfolderName" with the actual subfolder name)
            Outlook.MAPIFolder subFolder = inboxFolder.Folders["Test"];
            //subFolder = inboxFolder;

            Reply(subFolder);

            //MoveMailToMAPI(outlookApp, subFolder);
            //PrintAllMails(subFolder);
        }

        private static void Reply(MAPIFolder subFolder)
        {
            // Get the first mail item in the Inbox (for demonstration purposes)
            Outlook.MailItem mail = (Outlook.MailItem)subFolder.Items[1];

            // Reply to the mail item
            Outlook.MailItem reply = mail.Reply();
            reply.Body = "This is a reply to your email.\n\n" + reply.Body;
            reply.Subject = "Re: " + mail.Subject;
            reply.To = mail.SenderEmailAddress;
            reply.Move(subFolder);
        }

        private static void MoveMailToMAPI(Outlook.Application outlookApp, Outlook.MAPIFolder targetFolder)
        {
            // Create a new mail item
            Outlook.MailItem mail = (Outlook.MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);
            mail.Subject = "Sample Subject";
            mail.Body = "This is a sample body of the email.";
            mail.To = "recipient@example.com";
            mail.Move(targetFolder);
            Console.WriteLine("Mail moved to folder: " + targetFolder.Name);
        }

        private static void PrintAllMails(MAPIFolder subFolder)
        {
            // Print emails from the selected folder
            Outlook.Items mailItems = subFolder.Items;

            mailItems.Sort("[ReceivedTime]", true); // Sort by ReceivedTime from new to old 
            for (int i = 1; i <= 10 && i <= mailItems.Count; i++)
            {
                Outlook.MailItem item = mailItems[i] as Outlook.MailItem;
                if (item != null)
                {
                    Console.WriteLine("ID: " + item.EntryID);
                    Console.WriteLine("Subject: " + item.Subject);
                    //Console.WriteLine("Body: " + item.Body);
                    const string PR_SMTP_ADDRESS = "http://schemas.microsoft.com/mapi/proptag/0x39FE001E";
                    Outlook.PropertyAccessor pas = item.Sender.PropertyAccessor;
                    string ssmtpAddress = pas.GetProperty(PR_SMTP_ADDRESS).ToString();
                    Console.WriteLine("Sender: " + ssmtpAddress);
                    Outlook.Recipients recips = item.Recipients;
                    foreach (Outlook.Recipient recip in recips)
                    {
                        Outlook.PropertyAccessor pa = recip.PropertyAccessor;
                        string smtpAddress = pa.GetProperty(PR_SMTP_ADDRESS).ToString();
                        Console.WriteLine("Receiver: " + smtpAddress);
                    }
                    Console.WriteLine("Received: " + item.ReceivedTime.ToString());
                    Console.WriteLine("-----------------------------------");
                }
            }
        }
    }
}
