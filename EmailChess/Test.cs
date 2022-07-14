using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace ChessDotNET.EmailChess
{
    internal class Test
    {
        internal static async Task<bool> TestSMTPConnection(Dictionary<string, string> emailServer)
        {
            if (! await Task.Run(() => TestSMTPConnectionTask(emailServer))) return false;
            return true;
        }

        private static bool TestSMTPConnectionTask(Dictionary<string, string> emailServer)
        {
            try
            {
                var smtpClient = new SmtpClient(emailServer["smtp_server"])
                {
                    Port = int.Parse(emailServer["smtp_port"]),
                    Credentials = new NetworkCredential(emailServer["email_address"], emailServer["password"].Replace("\\\\", "\\")),
                    EnableSsl = true,
                };
                smtpClient.Send(emailServer["email_address"], "___fake___address___@server.com", "", "");
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
