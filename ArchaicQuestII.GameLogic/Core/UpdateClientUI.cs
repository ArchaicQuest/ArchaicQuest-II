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
                await _hubContext.Clients.Client(player.ConnectionId).SendAsync("UpdatePlayerHP", player.Attributes.Attribute[EffectLocation.Hitpoints], player.MaxAttributes.Attribute[EffectLocation.Hitpoints]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void UpdateMana(Player player)
        {
            try
            {
                await _hubContext.Clients.Client(player.ConnectionId).SendAsync("UpdatePlayerMana", player.Attributes.Attribute[EffectLocation.Mana], player.MaxAttributes.Attribute[EffectLocation.Mana]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void UpdateMoves(Player player)
        {
            try
            {
                await _hubContext.Clients.Client(player.ConnectionId).SendAsync("UpdatePlayerMoves", player.Attributes.Attribute[EffectLocation.Moves], player.MaxAttributes.Attribute[EffectLocation.Moves]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void UpdateExp(Player player)
        {
            try
            {
                await _hubContext.Clients.Client(player.ConnectionId).SendAsync("UpdatePlayerExp", player.ExperienceToNextLevel, player.ExperienceToNextLevel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
