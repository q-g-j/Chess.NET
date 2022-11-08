using ChessDotNET.Models;
using CommunityToolkit.Mvvm.Messaging;
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
    internal static class WebApiClientPlayersCommands
    {
        internal static async Task<ObservableCollection<Player>> GetAllPlayersAsync()
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            ObservableCollection<Player> playerList = new ObservableCollection<Player>();

            HttpResponseMessage response = await globals.httpClient.GetAsync(
                "api/players");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                playerList = JsonConvert.DeserializeObject<ObservableCollection<Player>>(jsonString);
            }

            return playerList;
        }

        internal static async Task<Player> CreatePlayerAsync(Player player)
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            Player playerJson;

            var response = await globals.httpClient.PostAsJsonAsync(
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

        internal static async Task ResetInactiveCounterAsync(int localPlayerId)
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            await globals.httpClient.PutAsJsonAsync(
                $"api/players/{localPlayerId}", localPlayerId);
        }

        internal static async Task DeletePlayerAsync(int localPlayerId)
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            await globals.httpClient.DeleteAsync(
                $"api/players/{localPlayerId}");
        }
    }
}
