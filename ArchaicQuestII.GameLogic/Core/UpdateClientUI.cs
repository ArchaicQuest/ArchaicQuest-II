using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ArchaicQuestII.GameLogic.Core
{
   public class UpdateClientUI: IUpdateClientUI
    {
        private readonly IHubContext<GameHub> _hubContext;

        public UpdateClientUI(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async void UpdateHP(Player player)
        {
            try
            {
                await _hubContext.Clients.Client(player.ConnectionId).SendAsync("SendMessage", player.Attributes.Attribute[EffectLocation.Hitpoints], "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
