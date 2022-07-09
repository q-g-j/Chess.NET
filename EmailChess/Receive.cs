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
        internal static string CheckForNextMove(Dictionary<string, string> emailServer, ChessPieceColor color)
        {
            var client = new Pop3Client();
            client.Connect(emailServer["pop3_server"], int.Parse(emailServer["pop3_port"]), true);
            client.Authenticate(emailServer["username"], @emailServer["password"].Replace("\\\\", "\\"));
            int messageCount = client.GetMessageCount();
            Message message = null;
            for (int i = messageCount; i > 0; i--)
            {
                var subject = client.GetMessage(i).Headers.Subject;

                if (color == ChessPieceColor.White && subject.Contains("ChessDotNetMoveWhite"))
                {
                    message = client.GetMessage(i);
                    client.DeleteMessage(i);
                    break;
                }

                else if (color == ChessPieceColor.Black && subject.Contains("ChessDotNetMoveBlack"))
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
