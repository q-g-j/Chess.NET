using ChessDotNET.CustomTypes;
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
        internal static async Task SendCurrentWhiteMove(Dictionary<string, string> emailServer, Coords oldCoords, Coords newCoords)
        {
            await Task.Run(() =>
            {
                var smtpClient = new SmtpClient(emailServer["smtp_server"])
                {
                    Port = int.Parse(emailServer["smtp_port"]),
                    Credentials = new NetworkCredential(emailServer["username"], emailServer["password"].Replace("\\\\", "\\")),
                    EnableSsl = true,
                };

                smtpClient.Send(emailServer["email_address"], emailServer["email_address"], "ChessDotNetMoveWhite", oldCoords.ToString() + "->" + newCoords.ToString());
            });
        }
        internal static async Task SendCurrentBlackMove(Dictionary<string, string> emailServer, Coords oldCoords, Coords newCoords)
        {
            await Task.Run(() =>
            {
                var smtpClient = new SmtpClient(emailServer["smtp_server"])
                {
                    Port = int.Parse(emailServer["smtp_port"]),
                    Credentials = new NetworkCredential(emailServer["username"], emailServer["password"].Replace("\\\\", "\\")),
                    EnableSsl = true,
                };

                smtpClient.Send(emailServer["email_address"], emailServer["email_address"], "ChessDotNetMoveBlack", oldCoords.ToString() + "->" + newCoords.ToString());
            });
        }
    }
}
