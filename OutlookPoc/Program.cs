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
                    Console.WriteLine("Sender: " + item.SenderName);
                    Console.WriteLine("Received: " + item.ReceivedTime.ToString());
                    Console.WriteLine("-----------------------------------");
                }
            }
        }
    }
}
