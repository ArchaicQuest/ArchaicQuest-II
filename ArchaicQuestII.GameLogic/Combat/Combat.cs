using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Combat
{
   public class Combat
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _clientUi;
        public Combat(IWriteToClient writer, IUpdateClientUI clientUi)
        {
            _writer = writer;
            _clientUi = clientUi;
        }

        public Player FindTarget(string target, Room room, bool isMurder)
        {
            // If mob
            if (!isMurder)
            {
                return (Player)room.Mobs.FirstOrDefault(x => x.Name.Contains(target));
            }

            return (Player)room.Players.FirstOrDefault(x => x.Name.StartsWith(target));
        }

        public int OffensivePoints(Player player)
        {
            var offensePoints = player.Attributes.Attribute[EffectLocation.Strength] / 5
                                   + player.Attributes.Attribute[EffectLocation.Dexterity] / 10
                                   + player.Attributes.Attribute[EffectLocation.HitRoll];
            var weaponSkill = 100; //weapon types are hardcoded so make hardcoded skills for weapon types

             return offensePoints + offensePoints * weaponSkill / 100;
        }

        public int DefensivePoints(Player player)
        {
            return player.Attributes.Attribute[EffectLocation.Dexterity] / 5 + player.Attributes.Attribute[EffectLocation.Strength] / 10;
        }

        public int ToHitChance(Player player, Player target)
        {
            return OffensivePoints(player) * 100 / (OffensivePoints(player) + DefensivePoints(target));
        }

        public bool DoesHit(int chance)
        {
            var roll = Dice.Roll(1, 1, 100);

            return roll switch
            {
                1 => false,
                100 => true,
                _ => roll > chance
            };
        }

        public void Fight(Player player, string victim, Room room, bool isMurder)
        {
            var target = FindTarget(victim, room, isMurder);

            if (target == null)
            {
                _writer.WriteLine("They are not here.", player.ConnectionId);
                return;
            }

            var chanceToHit = ToHitChance(player, target);

            var doesHit = DoesHit(chanceToHit);

            if (doesHit)
            {
                // check for block / dodge
                    // if block, show block message
                    // if dodge, show dodge message
                // check if critical hit, roll 1d20?
                    // do damage
                // check if target is dead or alive
                    // award xp if dead
                        // create corpse container holding targets inventory and money
                            // end combat
            }
            else
            {
                // miss message
                // gain improvements on weapon skill
            }

        }
    }
}
 