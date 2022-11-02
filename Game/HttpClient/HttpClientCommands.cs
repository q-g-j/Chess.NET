using ChessDotNET.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChessDotNET.WebClient
{
    internal static class HttpClientCommands
    {
        internal static readonly HttpClient client = new HttpClient();

        #region HttpGetCommands
        internal static async Task<ObservableCollection<Player>> GetAllPlayersAsync()
        {
            ObservableCollection<Player> playerList = new ObservableCollection<Player>();

            HttpResponseMessage response = await client.GetAsync(
                "api/players");

            if (response.IsSuccessStatusCode)
            {

                var jsonString = await response.Content.ReadAsStringAsync();
                playerList = JsonConvert.DeserializeObject<ObservableCollection<Player>>(jsonString);
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

        internal static async Task InvitePlayerAsync(int id, Player player)
        {
            await client.PutAsJsonAsync(
                $"api/players/invite/{id}", player);
        }
        #endregion HttpPutCommands
    }
}
