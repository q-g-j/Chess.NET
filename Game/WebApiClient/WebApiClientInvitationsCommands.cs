using ChessDotNET.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET.WebApiClient
{
    internal class WebApiClientInvitationsCommands
    {
        internal HttpClient client;

        public WebApiClientInvitationsCommands(HttpClient client)
        {
            this.client = client;
        }

        internal async Task<ObservableCollection<Player>> GetPlayerInvitationsAsync(int localPlayerId)
        {
            ObservableCollection<Player> invitations = new ObservableCollection<Player>();

            HttpResponseMessage response = await client.GetAsync(
                $"api/invitations/{localPlayerId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                invitations = JsonConvert.DeserializeObject<ObservableCollection<Player>>(jsonString);
            }

            return invitations;
        }

        internal async Task<Player> InvitePlayerAsync(int invitedId, Player localPlayer)
        {
            Player playerJson;

            var response = await client.PostAsJsonAsync(
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

        internal async Task CancelInvitationAsync(int invitedId, Player localPlayer)
        {
            await client.PutAsJsonAsync(
                $"api/invitations/cancel/{invitedId}", localPlayer);
        }
    }
}
