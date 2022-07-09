using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ChessDotNET.CustomTypes;
using ChessDotNET.Settings;
using OpenPop.Mime;
using OpenPop.Pop3;

namespace ChessDotNET.EmailChess
{
    internal class Receive
    {
        internal static string CheckForNextWhiteMove(Dictionary<string, string> EmailServer, ChessPieceColor color)
        {
            var client = new Pop3Client();
            client.Connect(EmailServer["pop3_server"], int.Parse(EmailServer["pop3_port"]), true);
            client.Authenticate(EmailServer["email_address"], @EmailServer["password"].Replace("\\\\", "\\"));
            int messageCount = client.GetMessageCount();
            Message message = null;
            for (int i = messageCount; i > 0; i--)
            {
                var subject = client.GetMessage(i).Headers.Subject;

                if (subject.Contains("ChessDotNetMoveWhite"))
                {
                    message = client.GetMessage(i);
                    client.DeleteMessage(i);
                    break;
                }
            }
            client.Disconnect();

            string messageText = "";

            if (message != null)
            {
                var messagePlainText = message.FindFirstPlainTextVersion();
                messageText = messagePlainText.GetBodyAsText();
            }

            return messageText;
        }
        internal static string CheckForNextBlackMove(Dictionary<string, string> EmailServer, ChessPieceColor color)
        {
            var client = new Pop3Client();
            client.Connect(EmailServer["pop3_server"], int.Parse(EmailServer["pop3_port"]), true);
            client.Authenticate(EmailServer["email_address"], @EmailServer["password"].Replace("\\\\", "\\"));
            int messageCount = client.GetMessageCount();
            Message message = null;
            for (int i = messageCount; i > 0; i--)
            {
                var subject = client.GetMessage(i).Headers.Subject;

                if (subject.Contains("ChessDotNetMoveBlack"))
                {
                    message = client.GetMessage(i);
                    client.DeleteMessage(i);
                    break;
                }
            }
            client.Disconnect();

            string messageText = "";

            if (message != null)
            {
                var messagePlainText = message.FindFirstPlainTextVersion();
                messageText = messagePlainText.GetBodyAsText();
            }

            return messageText;
        }
    }
}
