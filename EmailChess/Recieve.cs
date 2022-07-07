using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ChessDotNET.Settings;
using OpenPop.Mime;
using OpenPop.Pop3;

namespace ChessDotNET.EmailChess
{
    internal class Recieve
    {
        internal static void CheckForNextMove(AppSettings appsettings)
        {
            Console.Write("Password: ");
            string password = Console.ReadLine().Replace("\\", "\\\\");

            AppSettingsStruct appSettingsStruct = new AppSettingsStruct()
            {
                EmailServer = new Dictionary<string, string>()
                {
                    ["email_address"] = "name@server.com",
                    ["pop3_server"] = "pop.server.com",
                    ["smtp_server"] = "smtp.server.net",
                    ["pop3_port"] = "995",
                    ["smtp_port"] = "587",
                    ["username"] = "name@server.com",
                    ["password"] = password
                }
            };

            Dictionary<string, string> emailServer = appSettingsStruct.EmailServer;

            var client = new Pop3Client();
            client.Connect(emailServer["pop3_server"], int.Parse(emailServer["pop3_port"]), true);
            client.Authenticate(emailServer["username"], @emailServer["password"].Replace("\\\\", "\\"));
            int messageCount = client.GetMessageCount();
            Message message = null; ;
            for (int i = messageCount; i > 0; i--)
            {
                var subject = client.GetMessage(i).Headers.Subject;
                if (subject.Contains("ChessDotNet"))
                {
                    message = client.GetMessage(i);
                    break;
                }
            }

            var messagePlainText = message.FindFirstPlainTextVersion();
            var messageText = messagePlainText.GetBodyAsText();
            Console.WriteLine(messageText);
        }
    }
}
