using ChessDotNET.Models;
using ChessDotNET.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET
{
    internal class Globals
    {
        internal HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri(@"http://qgj.ddns.net:7002/")
        };
        internal bool IsOnlineGame = false;
        internal bool IsWaitingForMove = false;
        internal Game CurrentOnlineGame;
        internal Lobby LobbyWindow;
        internal List<Move> MoveList;
        internal bool IsRotated;
        internal bool IsCheckMate = false;
    }
}
