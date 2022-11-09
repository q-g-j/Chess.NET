using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

using ChessDotNET.Models;


namespace ChessDotNET.WebApiClient
{
    internal static class WebApiClientInvitationsCommands
    {
        internal static async Task<ObservableCollection<Player>> GetPlayerInvitationsAsync(int localPlayerId)
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            ObservableCollection<Player> invitations = new ObservableCollection<Player>();

            HttpResponseMessage response = await globals.Client.GetAsync(
                $"api/invitations/{localPlayerId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                invitations = JsonConvert.DeserializeObject<ObservableCollection<Player>>(jsonString);
            }

            return invitations;
        }

        internal static async Task<Player> InvitePlayerAsync(int invitedId, Player localPlayer)
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            Player playerJson;

            var response = await globals.Client.PostAsJsonAsync(
                $"api/invitations/{invitedId}", localPlayer);

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

        internal static async Task CancelInvitationAsync(int invitedId, Player localPlayer)
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            await globals.Client.PutAsJsonAsync(
                $"api/invitations/cancel/{invitedId}", localPlayer);
        }
    }
}
