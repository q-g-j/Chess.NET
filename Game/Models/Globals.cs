using ChessDotNET.Models;
using ChessDotNET.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET
{
    internal class Globals
    {
        public Globals(string address)
        {
            Client = new HttpClient
            {
                BaseAddress = new Uri(address)
            };

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        internal HttpClient Client;
        internal bool IsOnlineGame = false;
        internal bool IsWaitingForMove = false;
        internal Game CurrentOnlineGame;
        internal Lobby LobbyWindow;
        internal List<Move> MoveList;
        internal bool IsRotated;
        internal bool IsCheckMate = false;
        internal int CurrentlyDraggedChessPieceOriginalCanvasLeft;
        internal int CurrentlyDraggedChessPieceOriginalCanvasTop;
    }
}
