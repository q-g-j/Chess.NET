using ChessDotNET.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChessDotNET.WebClient
{
    internal static class Commands
    {
        internal static readonly HttpClient client = new HttpClient();

        #region HttpGetCommands
        internal static async Task<List<Player>> GetAllPlayersAsync()
        {
            List<Player> playerList = new List<Player>();

            HttpResponseMessage response = await client.GetAsync(
                "api/players");

            if (response.IsSuccessStatusCode)
            {

                var jsonString = await response.Content.ReadAsStringAsync();
                playerList = JsonConvert.DeserializeObject<List<Player>>(jsonString);
            }

            return playerList;
        }
        #endregion HttpGetCommands

        #region HttpPostCommands
        internal static async Task<Player> CreatePlayerAsync(Player player)
        {
            Player playerJson;

            var response = await client.PostAsJsonAsync(
                "api/players", player);


            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                playerJson = JsonConvert.DeserializeObject<Player>(jsonString);
            }
            else
            {
                playerJson = new Player();
            }

            return playerJson;
        }
        #endregion HttpPostCommands

        #region HttpPutCommands
        internal static async Task ResetInactiveCounterAsync(Player player)
        {
            await client.PutAsJsonAsync(
                $"api/players/{player.Id}", player.Id);
        }
        #endregion HttpPutCommands
    }
}
