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

            // Print emails from the selected folder
            Outlook.Items mailItems = subFolder.Items;
            
            mailItems.Sort("[ReceivedTime]", true); // Sort by ReceivedTime from new to old 
            for (int i = 1; i <= 10 && i <= mailItems.Count; i++)
            {
                Outlook.MailItem item = mailItems[i] as Outlook.MailItem;
                if (item != null)
                {
                    Console.WriteLine("Subject: " + item.Subject);
                    Console.WriteLine("Body: " + item.Body);
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
