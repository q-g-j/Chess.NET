using ChessDotNET.Models;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace ChessDotNET.WebApiClient
{
    internal static class WebApiClientGamesCommands
    {
        internal static async Task<Game> GetNewGame(int invitingId)
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            Game newGame = new Game();

            var response = await globals.httpClient.GetAsync(
                $"api/games/{invitingId}");
            
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                newGame = JsonConvert.DeserializeObject<Game>(jsonString);
            }

            return newGame;
        }

        internal static async Task<Game> GetCurrentGame(int gameId)
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            Game currentMove = new Game();

            var response = await globals.httpClient.GetAsync(
                $"api/games/current/{gameId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                currentMove = JsonConvert.DeserializeObject<Game>(jsonString);
            }

            return currentMove;
        }

        internal static async Task<Game> StartNewGameAsync(Game newGame)
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            Game responseNewGame = new Game();

            var response = await globals.httpClient.PostAsJsonAsync(
                $"api/games", newGame);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                responseNewGame = JsonConvert.DeserializeObject<Game>(jsonString);
            }

            return responseNewGame;
        }

        internal static async Task<Game> PutCurrentGame(int gameId, Game currentGame)
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            Game currentMove = new Game();

            var response = await globals.httpClient.PutAsJsonAsync(
                $"api/games/current/{gameId}", currentGame);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                currentMove = JsonConvert.DeserializeObject<Game>(jsonString);
            }

            return currentMove;
        }

        internal static async Task ResetWhiteInactiveCounterAsync(int gameId)
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            await globals.httpClient.PutAsJsonAsync(
                $"api/games/current/counter/white/{gameId}", gameId);
        }

        internal static async Task ResetBlackInactiveCounterAsync(int gameId)
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            await globals.httpClient.PutAsJsonAsync(
                $"api/games/current/counter/black/{gameId}", gameId);
        }
    }
}
