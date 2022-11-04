﻿using ChessDotNET.Models;
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
        internal static async Task<ObservableCollection<Player>> GetPlayerInvitationsAsync(int id)
        {
            ObservableCollection<Player> invitations = new ObservableCollection<Player>();

            HttpResponseMessage response = await client.GetAsync(
                $"api/players/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                invitations = JsonConvert.DeserializeObject<ObservableCollection<Player>>(jsonString);
            }

            return invitations;
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

        internal static async Task InvitePlayerAsync(int invitedId, Player invitingPlayer)
        {
            await client.PostAsJsonAsync(
                $"api/players/invite/{invitedId}", invitingPlayer);
        }
        #endregion HttpPutCommands

        #region HttpDeleteCommands
        internal static async Task DeletePlayerAsync(int id)
        {
            await client.DeleteAsync(
                $"api/players/{id}");
        }
        #endregion HttpDeleteCommands
    }
}
