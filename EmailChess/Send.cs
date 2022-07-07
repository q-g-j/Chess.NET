using ChessDotNET.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET.EmailChess
{
    internal class Send
    {
        internal static void SendCurrentMove()
        {
            AppSettingsStruct appSettingsStruct = new AppSettingsStruct();

            var emailServer = appSettingsStruct.EmailServer;

            var smtpClient = new SmtpClient(emailServer["smtp_server"])
            {
                Port = int.Parse(emailServer["smtp_port"]),
                Credentials = new NetworkCredential(emailServer["username"], emailServer["password"].Replace("\\\\", "\\")),
                EnableSsl = true,
            };

            smtpClient.Send("sender", "recipient", "ChessDotNetMove", "B1->C3");
        }
    }
}
