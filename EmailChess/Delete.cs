using OpenPop.Pop3;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ChessDotNET.EmailChess
{
    internal class Delete
    {
        internal static async Task DeleteOldEmails(Dictionary<string, string> emailServer, bool keepNewest)
        {
            await Task.Run(() =>
            {
                var client = new Pop3Client();
                client.Connect(emailServer["pop3_server"], int.Parse(emailServer["pop3_port"]), true);
                client.Authenticate(emailServer["email_address"], emailServer["password"].Replace("\\\\", "\\"));
                int messageCount = client.GetMessageCount();
                for (int i = messageCount; i > 0; i--)
                {
                    var subject = client.GetMessage(i).Headers.Subject;

                    if (subject.Contains("ChessDotNetMoveWhite") || subject.Contains("ChessDotNetMoveBlack"))
                    {
                        client.DeleteMessage(i);
                    }
                }
                client.Disconnect();
            });
        }
    }
}
