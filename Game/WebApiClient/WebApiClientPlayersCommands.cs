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

namespace ChessDotNET.WebApiClient
{
    internal class WebApiClientPlayersCommands
    {
        internal HttpClient client;

        public WebApiClientPlayersCommands(HttpClient client)
        {
            this.client = client;
        }

        internal async Task<ObservableCollection<Player>> GetAllPlayersAsync()
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

        internal async Task<Player> CreatePlayerAsync(Player player)
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

        internal async Task ResetInactiveCounterAsync(int localPlayerId)
        {
            await client.PutAsJsonAsync(
                $"api/players/{localPlayerId}", localPlayerId);
        }

        internal async Task DeletePlayerAsync(int localPlayerId)
        {
            await client.DeleteAsync(
                $"api/players/{localPlayerId}");
        }
    }
}
