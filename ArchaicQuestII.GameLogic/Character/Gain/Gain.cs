using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;

namespace ArchaicQuestII.GameLogic.Character.Gain
{
  
    public class Gain : IGain
    {

        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _clientUi;

        public Gain(IWriteToClient writer, IUpdateClientUI clientUI)
        {
            _writer = writer;
            _clientUi = clientUI;
        }
        public void GainExperiencePoints(Player player, Player target)
        {

            var expWorth = GetExpWorth(target);
            if (Math.Floor((double)(player.Level / 2)) > target.Level)
            {
                expWorth /= 2;
            }
            player.Experience += expWorth;
            player.ExperienceToNextLevel -= expWorth;

            _clientUi.UpdateExp(player);

            if (expWorth == 1)
            {
                _writer.WriteLine(
                    $"<p class='improve'>You gain 1 measly experience point.</p>",
                    player.ConnectionId);
            }

            _writer.WriteLine(
                $"<p class='improve'>You receive {expWorth} experience points.</p>",
                player.ConnectionId);
         
        }

        public void GainExperiencePoints(Player player, int value)
        {

            player.Experience += value;
            player.ExperienceToNextLevel -= value;

            _clientUi.UpdateExp(player);

            _writer.WriteLine(
                $"<p class='improve'>You receive {value} experience points.</p>",
                player.ConnectionId);

        }

        public int GetExpWorth(Player character)
        {
            var maxEXP = 10000;
            var exp = character.Level;
            exp += character.Equipped.Wielded?.Damage.Maximum ?? 6; // 6 for hand to hand
            exp += character.Attributes.Attribute[EffectLocation.DamageRoll] * 10;
            exp += character.Attributes.Attribute[EffectLocation.HitRoll] +  character.Level * 10;
            exp += character.ArmorRating.Armour;
            // boost xp if mob is shielded

            return exp > maxEXP ? maxEXP : exp;
        }
    }
}
