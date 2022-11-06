using ChessDotNET.Models;
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
    internal class WebApiClientGamesCommands
    {
        internal HttpClient client;

        public WebApiClientGamesCommands(HttpClient client)
        {
            this.client = client;
        }

        internal async Task<Game> GetNewGame(int invitingId)
        {
            Game newGame = new Game();

            var response = await client.GetAsync(
                $"api/games/{invitingId}");
            
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                newGame = JsonConvert.DeserializeObject<Game>(jsonString);
            }

            return newGame;
        }

        internal async Task<Game> GetCurrentGame(int gameId)
        {
            Game currentMove = new Game();

            var response = await client.GetAsync(
                $"api/games/current/{gameId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                currentMove = JsonConvert.DeserializeObject<Game>(jsonString);
            }

            return currentMove;
        }

        internal async Task<Game> StartNewGameAsync(Game newGame)
        {
            Game responseNewGame = new Game();

            var response = await client.PostAsJsonAsync(
                $"api/games", newGame);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                responseNewGame = JsonConvert.DeserializeObject<Game>(jsonString);
            }

            return responseNewGame;
        }

        internal async Task<Game> PutCurrentGame(int gameId, Game currentGame)
        {
            Game currentMove = new Game();

            var response = await client.PutAsJsonAsync(
                $"api/games/current/{gameId}", currentGame);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                currentMove = JsonConvert.DeserializeObject<Game>(jsonString);
            }

            return currentMove;
        }

        internal async Task ResetWhiteInactiveCounterAsync(int gameId)
        {
            await client.PutAsJsonAsync(
                $"api/games/current/counter/white/{gameId}", gameId);
        }

        internal async Task ResetBlackInactiveCounterAsync(int gameId)
        {
            await client.PutAsJsonAsync(
                $"api/games/current/counter/black/{gameId}", gameId);
        }
    }
}
