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

namespace ChessDotNET.WebClient
{
    internal static class WebClientCommands
    {
        internal static readonly HttpClient client = new HttpClient();

        #region HttpGetCommands
        internal static async Task<List<Player>> GetAllPlayersAsync()
        {
            HttpResponseMessage response = await client.GetAsync(
                "lobby");
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            List<Player> playerList = JsonConvert.DeserializeObject<List<Player>>(jsonString);

            return playerList;
        }
        #endregion HttpGetCommands

        #region HttpPostCommands
        internal static async Task<Player> CreatePlayerAsync(Player player)
        {
            var response = await client.PostAsJsonAsync(
                "lobby", player);

            Player playerJson;

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
                "lobby", player);
        }
        #endregion HttpPutCommands
    }
}
