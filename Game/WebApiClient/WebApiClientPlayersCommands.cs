using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

using ChessDotNET.Models;


namespace ChessDotNET.WebApiClient
{
    internal static class WebApiClientPlayersCommands
    {
        internal static async Task<ObservableCollection<Player>> GetAllPlayersAsync()
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            ObservableCollection<Player> playerList = new ObservableCollection<Player>();

            HttpResponseMessage response = await globals.Client.GetAsync(
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

            var response = await globals.Client.PostAsJsonAsync(
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
            await globals.Client.PutAsJsonAsync(
                $"api/players/{localPlayerId}", localPlayerId);
        }

        internal static async Task DeletePlayerAsync(int localPlayerId)
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            await globals.Client.DeleteAsync(
                $"api/players/{localPlayerId}");
        }
    }
}
