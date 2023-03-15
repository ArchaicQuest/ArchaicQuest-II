﻿using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;

namespace ArchaicQuestII.GameLogic.Character.Gain
{
    public class Gain : IGain
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _clientUi;
        private readonly ICache _cache;

        public Gain(IWriteToClient writer, IUpdateClientUI clientUI, ICache cache)
        {
            _writer = writer;
            _clientUi = clientUI;
            _cache = cache;
        }
        public void GainExperiencePoints(Player player, Player target)
        {
            var expWorth = GetExpWorth(target);
            var halfPlayerLevel = Math.Ceiling((double)(player.Level / 2m));
            /*
     
            The following only happens If (player level / 2) is Greater than or equal to mob level  
           If (player level / 2) + 2 is Greater than or equal to mob level then Exp Worth is divided by 4
           Else Exp Worth is divided by 2
            
            */
            if (halfPlayerLevel >= target.Level)
            {
                if (halfPlayerLevel + 2 >= target.Level)
                {
                    expWorth /= 4;  
                }
                else
                {
                    expWorth /= 2;
                }
              
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

            GainLevel(player);
            _clientUi.UpdateExp(player);
        }

        public void GroupGainExperiencePoints(Player player, Player target)
        {
            if (player.Grouped)
            {
                var isGroupLeader = string.IsNullOrEmpty(player.Following);

                var groupLeader = player;

                if (!isGroupLeader)
                {
                    groupLeader = _cache.GetPlayerCache().FirstOrDefault(x => x.Value.Name.Equals(player.Following)).Value;
                }

                if (groupLeader.RoomId == target.RoomId)
                {
                    GainExperiencePoints(groupLeader, target);
                }

                foreach (var follower in groupLeader.Followers.Where(follower => follower.Grouped && follower.Following == groupLeader.Name).Where(follower => follower.RoomId == target.RoomId))
                {
                    GainExperiencePoints(follower, target);
                }
            }
        }

        public void GainExperiencePoints(Player player, int value, bool showMessage = true)
        {
            // TODO: gain level
            player.Experience += value;
            player.ExperienceToNextLevel -= value;

            if (showMessage)
            {
                _writer.WriteLine(
                    $"<p class='improve'>You receive {value} experience points.</p>",
                    player.ConnectionId);
            }

            GainLevel(player);

            _clientUi.UpdateExp(player);

        }

        public void GainLevel(Player player)
        {
            if (player.ExperienceToNextLevel <= 0)
            {
                player.Level++;
                player.ExperienceToNextLevel = player.Level * 4000; //TODO: have class and race mod

                var hpGain = (player.MaxAttributes.Attribute[EffectLocation.Constitution] / 100m) * 20;
                var minHPGain = (hpGain / 100m) * 20;
                var totalHP = DiceBag.Roll(1, (int)minHPGain, (int)hpGain);

                var manaGain = player.MaxAttributes.Attribute[EffectLocation.Intelligence] / 100m * 20;
                var minManaGain = manaGain / 100m * 20;
                var totalMana = DiceBag.Roll(1, (int)minManaGain, (int)manaGain);

                var moveGain = player.MaxAttributes.Attribute[EffectLocation.Dexterity] / 100m * 20;
                var minMoveGain = manaGain / 100 * 20;
                var totalMove = DiceBag.Roll(1, (int)minMoveGain, (int)moveGain);

                //player.Attributes.Attribute[EffectLocation.Hitpoints] += totalHP;
                //player.Attributes.Attribute[EffectLocation.Mana] += totalMana;
                //player.Attributes.Attribute[EffectLocation.Moves] += totalMove;
                player.MaxAttributes.Attribute[EffectLocation.Hitpoints] += totalHP;
                player.MaxAttributes.Attribute[EffectLocation.Mana] += totalMana;
                player.MaxAttributes.Attribute[EffectLocation.Moves] += totalMove;

                _writer.WriteLine($"<p class='improve'>You have advanced to level {player.Level}, you gain: {totalHP} HP, {totalMana} Mana, {totalMove} Moves.</p>", player.ConnectionId);

                SeedData.Classes.SetGenericTitle(player);

                _clientUi.UpdateMana(player);
                _clientUi.UpdateMoves(player);
                _clientUi.UpdateHP(player);
                _clientUi.UpdateExp(player);
                _clientUi.UpdateScore(player);

            }
        }

        public void GainLevel(Player player, string target)
        {
            var foundTarget = _cache.GetPlayerCache()
                .FirstOrDefault(x => x.Value.Name.Equals(target, StringComparison.CurrentCultureIgnoreCase));

            if (foundTarget.Value == null)
            {
                _writer.WriteLine($"Cannot find {target}.");
                return;
            }

            foundTarget.Value.ExperienceToNextLevel = 0;
            _writer.WriteLine($"{player.Name} has rewarded you with a level.", foundTarget.Value.ConnectionId);
            GainLevel(foundTarget.Value);
        }

        public int GetExpWorth(Player character)
        {
            var maxEXP = 10000;
            var exp = character.Level;
            exp += DiceBag.Roll(1, 25, 275);
            exp += character.Equipped.Wielded?.Damage.Maximum ?? 6; // 6 for hand to hand
            exp += character.Attributes.Attribute[EffectLocation.DamageRoll] * 10;
            exp += character.Attributes.Attribute[EffectLocation.HitRoll] + character.Level * 10;
            exp += character.ArmorRating.Armour;

            exp += character.Attributes.Attribute[EffectLocation.Hitpoints] * 3;
            exp += character.Attributes.Attribute[EffectLocation.Mana];
            exp += character.Attributes.Attribute[EffectLocation.Strength];
            exp += character.Attributes.Attribute[EffectLocation.Dexterity];
            exp += character.Attributes.Attribute[EffectLocation.Constitution];
            exp += character.Attributes.Attribute[EffectLocation.Wisdom];
            exp += character.Attributes.Attribute[EffectLocation.Intelligence];
            exp += character.ArmorRating.Magic;
            exp += character.Level * 15;
            //exp += character.Attributes.Attribute[EffectLocation.Moves];
            // boost xp if mob is shielded

            return exp > maxEXP ? maxEXP : exp;
        }

        public void GainSkillExperience(Player character, int expGain, SkillList skill, int increase)
        {
            character.Experience += expGain;
            character.ExperienceToNextLevel -= expGain;
            skill.Proficiency += increase;

            GainLevel(character);
            _clientUi.UpdateExp(character);

            _writer.WriteLine(
                $"<p class='improve'>You learn from your mistakes and gain {expGain} experience points.</p>",
                character.ConnectionId);
            _writer.WriteLine(
                $"<p class='improve'>Your {skill.SkillName} skill increases by {increase}%.</p>",
                character.ConnectionId);
        }
    }
}
