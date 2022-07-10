using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenPop.Mime;
using OpenPop.Pop3;
using ChessDotNET.CustomTypes;
using ChessDotNET.Settings;
using ChessDotNET.GUI.ViewModels.MainWindow;


namespace ChessDotNET.EmailChess
{
    internal class Receive
    {
        internal static string CheckForNextWhiteMove(Dictionary<string, string> emailServer)
        {
            var client = new Pop3Client();
            client.Connect(emailServer["pop3_server"], int.Parse(emailServer["pop3_port"]), true);
            client.Authenticate(emailServer["email_address"], emailServer["password"].Replace("\\\\", "\\"));
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
        internal static string CheckForNextBlackMove(Dictionary<string, string> emailServer)
        {
            var client = new Pop3Client();
            client.Connect(emailServer["pop3_server"], int.Parse(emailServer["pop3_port"]), true);
            client.Authenticate(emailServer["email_address"], emailServer["password"].Replace("\\\\", "\\"));
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
        internal static async Task WaitForEmailNextWhiteMoveTask(MainWindowViewModel vm, TileDictionary tileDict, AppSettings appSettings)
        {
            await Task.Run(() =>
            {
                AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
                string message;
                string oldCoordsString = "";
                string newCoordsString = "";
                bool hasReceived = false;
                while (!hasReceived)
                {
                    message = CheckForNextWhiteMove(appSettingsStruct.EmailServer);
                    if (message == "")
                    {
                        System.Threading.Thread.Sleep(5000);
                    }
                    else
                    {
                        hasReceived = true;
                        Console.WriteLine(message);
                        oldCoordsString = message.Substring(0, 2);
                        newCoordsString = message.Substring(4, 2);
                    }
                }
                Console.WriteLine(oldCoordsString, newCoordsString);

                (oldCoordsString, newCoordsString) = Coords.InvertCoords(oldCoordsString, newCoordsString);

                tileDict[newCoordsString].ChessPiece = tileDict[oldCoordsString].ChessPiece;
                tileDict[oldCoordsString].ChessPiece = new ChessPiece(ChessPieceImages.Empty, ChessPieceColor.Empty, ChessPieceType.Empty);
                tileDict[oldCoordsString].IsOccupied = false;
                tileDict[newCoordsString].IsOccupied = true;
                tileDict[newCoordsString].ChessPiece.MoveCount++;
                vm.TileDict = tileDict;
            });
        }
        internal static async Task WaitForEmailNextBlackMoveTask(MainWindowViewModel vm, TileDictionary tileDict, AppSettings appSettings)
        {
            await Task.Run(() =>
            {
                AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
                string message;
                string oldCoordsString = "";
                string newCoordsString = "";
                bool hasReceived = false;
                while (!hasReceived)
                {
                    message = CheckForNextBlackMove(appSettingsStruct.EmailServer);
                    if (message == "")
                    {
                        System.Threading.Thread.Sleep(5000);
                    }
                    else
                    {
                        hasReceived = true;
                        Console.WriteLine(message);
                        oldCoordsString = message.Substring(0, 2);
                        newCoordsString = message.Substring(4, 2);
                    }
                }

                Console.WriteLine(oldCoordsString, newCoordsString);

                (oldCoordsString, newCoordsString) = Coords.InvertCoords(oldCoordsString, newCoordsString);

                tileDict[newCoordsString].ChessPiece = tileDict[oldCoordsString].ChessPiece;
                tileDict[oldCoordsString].ChessPiece = new ChessPiece(ChessPieceImages.Empty, ChessPieceColor.Empty, ChessPieceType.Empty);
                tileDict[oldCoordsString].IsOccupied = false;
                tileDict[newCoordsString].IsOccupied = true;
                tileDict[newCoordsString].ChessPiece.MoveCount++;
                vm.TileDict = tileDict;
            });
        }
    }
}
