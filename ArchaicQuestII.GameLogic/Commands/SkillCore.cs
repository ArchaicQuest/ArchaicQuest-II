using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands;

public abstract class SkillCore
{
    private ICore Core { get; }
    protected SkillCore(ICore core)
    {
        Core = core;
    }
    
        public Player GetValidTarget(Player player, Player target, ValidTargets validTargets)
        {

            var setTarget = target;
            if (validTargets.HasFlag(ValidTargets.TargetFightSelf) && player.Status == CharacterStatus.Status.Fighting)
            {
                setTarget = player;
            }

            if (validTargets.HasFlag(ValidTargets.TargetFightVictim) && player.Status == CharacterStatus.Status.Fighting)
            {
                setTarget = target;
            }

            if (validTargets == ValidTargets.TargetIgnore)
            {
                setTarget = player;
            }

            return setTarget;
        }

        public bool HasSkill(Player player, string skill)
        {
            return player.Skills.FirstOrDefault(x => x.SkillName.Equals(skill, StringComparison.CurrentCultureIgnoreCase) && x.Level <= player.Level) != null;

        }

        public Player findTarget(Player player, string target, Room room, bool murder)
        {
            return Core.Combat.FindTarget(player, target, room, murder);

        }

        public void updateCombat(Player player, Player target, Room room)
        {
            if (target != null)
            {

                if (Core.Combat.IsTargetAlive(target))
                {

                    Core.Combat.InitFightStatus(player, target);
                }

            }

        }

        public string ReplacePlaceholders(string str, Player player, bool isTarget)
        {
            var newString = String.Empty;
            if (isTarget)
            {
                newString = str.Replace("#target#", "You");

                return newString;
            }

            newString = str.Replace("#target#", player.Name);

            return newString;

        }
        
        /*
         * Message for when attribute is full
         * message for player
         * message for target
         * message for room
         *
         */

        public bool AffectPlayerAttributes(string spellName, EffectLocation attribute, int value, Player player, Player target, Room room, string noAffect)
        {

            if ((attribute == EffectLocation.Hitpoints || attribute == EffectLocation.Mana || attribute == EffectLocation.Moves) && target.Attributes.Attribute[attribute] == target.MaxAttributes.Attribute[attribute])
            {
                Core.Writer.WriteLine(ReplacePlaceholders(noAffect, target, false), player.ConnectionId);
                return false;
            }

            target.Attributes.Attribute[attribute] += value;

            if ((attribute == EffectLocation.Hitpoints || attribute == EffectLocation.Mana || attribute == EffectLocation.Moves) && target.Attributes.Attribute[attribute] > target.MaxAttributes.Attribute[attribute])
            {
                target.Attributes.Attribute[attribute] = target.MaxAttributes.Attribute[attribute];
            }

            return true;

        }

        /// <summary>
        /// Adds affects to player
        /// Bless
        /// HitRoll +10
        /// DamRoll + 5
        /// </summary>
        /// <param name="spellAffects"></param>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <param name="room"></param>
        public void AddAffectToPlayer(List<Affect> spellAffects, Player player, Player target, Room room)
        {
            foreach (var affects in spellAffects)
            {
                var hasEffect = target.Affects.Custom.FirstOrDefault(x => x.Name.Equals(affects.Name));
                if (hasEffect != null)
                {
                    hasEffect.Duration = affects.Duration;
                }
                else
                {
                    target.Affects.Custom.Add(new Affect()
                    {
                        Modifier = affects.Modifier,
                        Benefits = affects.Benefits,
                        Affects = affects.Affects,
                        Duration = player.Level + player.Attributes.Attribute[EffectLocation.Intelligence] / 2,
                        Name = affects.Name
                    });

                    if (affects.Affects == DefineSpell.SpellAffect.Blind)
                    {
                        target.Affects.Blind = true;
                    }

                    //apply affects to target
                    if (affects.Modifier.Strength != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Strength] += affects.Modifier.Strength;
                    }


                    if (affects.Modifier.Dexterity != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Dexterity] += affects.Modifier.Dexterity;
                    }

                    if (affects.Modifier.Charisma != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Charisma] += affects.Modifier.Charisma;
                    }

                    if (affects.Modifier.Constitution != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Constitution] += affects.Modifier.Constitution;
                    }

                    if (affects.Modifier.Intelligence != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Intelligence] += affects.Modifier.Intelligence;
                    }

                    if (affects.Modifier.Wisdom != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Wisdom] += affects.Modifier.Wisdom;
                    }

                    if (affects.Modifier.DamRoll != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.DamageRoll] += affects.Modifier.DamRoll;
                    }

                    if (affects.Modifier.HitRoll != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.HitRoll] += affects.Modifier.HitRoll;
                    }

                    if (affects.Modifier.HP != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Hitpoints] += affects.Modifier.HP;

                        if (target.Attributes.Attribute[EffectLocation.Hitpoints] >
                            target.MaxAttributes.Attribute[EffectLocation.Hitpoints])
                        {
                            target.Attributes.Attribute[EffectLocation.Hitpoints] =
                                target.MaxAttributes.Attribute[EffectLocation.Hitpoints];
                        }
                    }

                    if (affects.Modifier.Mana != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Mana] += affects.Modifier.Mana;

                        if (target.Attributes.Attribute[EffectLocation.Mana] >
                            target.MaxAttributes.Attribute[EffectLocation.Mana])
                        {
                            target.Attributes.Attribute[EffectLocation.Mana] =
                                target.MaxAttributes.Attribute[EffectLocation.Mana];
                        }
                    }

                    if (affects.Modifier.Moves != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Moves] += affects.Modifier.Moves;

                        if (target.Attributes.Attribute[EffectLocation.Moves] >
                            target.MaxAttributes.Attribute[EffectLocation.Moves])
                        {
                            target.Attributes.Attribute[EffectLocation.Moves] =
                                target.MaxAttributes.Attribute[EffectLocation.Moves];
                        }
                    }

                }
            }

            Core.UpdateClient.UpdateAffects(target);
            Core.UpdateClient.UpdateScore(target);
        }

        public void UpdateClientUI(Player player)
        {
            //update UI
            Core.UpdateClient.UpdateHP(player);
            Core.UpdateClient.UpdateMana(player);
            Core.UpdateClient.UpdateMoves(player);
            Core.UpdateClient.UpdateScore(player);
        }

        /// <summary>
        /// Emote action to the target and to the room
        /// </summary>
        /// <param name="textToTarget">Text the target should see</param>
        /// <param name="textToRoom">Text the room should see</param>
        /// <param name="target">Target of the action</param>
        /// <param name="room">Current Room</param>
        /// <param name="player">The Player</param>
        public void EmoteAction(string textToTarget, string textToRoom, string target, Room room, Player player)
        {
            foreach (var pc in room.Players.Where(x => x.Name != player.Name))
            {
                if (pc.Name.Equals(target))
                {
                    Core.Writer.WriteLine(textToTarget, pc.ConnectionId);
                    continue;
                }
                    
                Core.Writer.WriteLine(textToRoom, pc.ConnectionId);
            }
        }

        public void EmoteEffectWearOffAction(Player player, Room room, SkillMessage emote)
        {

            foreach (var pc in room.Players)
            {
                if (pc.ConnectionId.Equals(player.ConnectionId))
                {
                    Core.Writer.WriteLine($"<p>{emote.EffectWearOff.ToPlayer}</p>",
                        pc.ConnectionId);
                    continue;
                }

                Core.Writer.WriteLine($"<p>{ReplacePlaceholders(emote.EffectWearOff.ToRoom, player, false)}</p>",
                    pc.ConnectionId);

            }
        }

        /// <summary>
        /// Check if skill is a success comparing the attackers dexterity to the target
        /// and any level difference. for example:
        /// Player Level = 5, Target Level = 15
        /// Player Dexterity = 60, Target Dexterity = 75
        ///
        ///  base chance = 65
        ///  65 + 60 = 125
        ///  125 - 75 = 50
        ///  50 + 5 - 15 = 40
        ///
        /// Chance for success is 40
        ///
        /// roll 1d100 if it's <= 40 then it's a hit
        ///
        /// if everyone is equal it's 65% chance for success 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool DexterityAndLevelCheck(Player player, Player target)
        {
            /*dexterity check */
            var chance = 65;
            chance += player.Attributes.Attribute[EffectLocation.Dexterity];
            chance -= target.Attributes.Attribute[EffectLocation.Dexterity];

            if (player.Affects.Haste)
            {
                chance += 25;
            }

            if (target.Affects.Haste)
            {
                chance -= 25;
            }

            /* level check */
            chance += player.Level - target.Level;

            return DiceBag.Roll(1, 1, 100) <= chance;
        }
        
        public void DamagePlayer(string skillName, int damage, Player player, Player target, Room room)
        {

            if (Core.Combat.IsTargetAlive(target))
            {

                var totalDam = Core.Combat.CalculateSkillDamage(player, target, damage);

                Core.Writer.WriteLine(
                    $"<p>Your {skillName} {Core.Damage.DamageText(totalDam).Value} {target.Name}  <span class='damage'>[{damage}]</span></p>",
                    player.ConnectionId);
                Core.Writer.WriteLine(
                    $"<p>{player.Name}'s {skillName} {Core.Damage.DamageText(totalDam).Value} you!  <span class='damage'>[{damage}]</span></p>",
                    target.ConnectionId);

                foreach (var pc in room.Players)
                {
                    if (pc.ConnectionId.Equals(player.ConnectionId) ||
                        pc.ConnectionId.Equals(target.ConnectionId))
                    {
                        continue;
                    }

                    Core.Writer.WriteLine(
                        $"<p>{player.Name}'s {skillName} {Core.Damage.DamageText(totalDam).Value} {target.Name}  <span class='damage'>[{damage}]</span></p>",
                        pc.ConnectionId);

                }

                target.Attributes.Attribute[EffectLocation.Hitpoints] -= totalDam;

                if (!Core.Combat.IsTargetAlive(target))
                {
                    Core.Combat.TargetKilled(player, target, room);

                    Core.UpdateClient.UpdateHP(target);
                    return;
                    //TODO: create corpse, refactor fight method from combat.cs
                }

                //update UI
                Core.UpdateClient.UpdateHP(target);

                Core.Combat.AddCharToCombat(target);
                Core.Combat.AddCharToCombat(player);

         
            }

        }

        public Player FindTargetInRoom(string targetName, Room room, Player player)
        {
            var target =
                room.Players.FirstOrDefault(x =>
                    x.Name.StartsWith(targetName, StringComparison.CurrentCultureIgnoreCase)) ??
                room.Mobs.FirstOrDefault(x => x.Name.StartsWith(targetName, StringComparison.CurrentCultureIgnoreCase));

            if (target != null)
            {
                return target;
            }
            
            Core.Writer.WriteLine("They are not here.", player.ConnectionId);
            return null;

        }

        /// <summary>
        /// Finds the item in the room or inventory
        /// </summary>
        /// <param name="obj">name of object to find</param>
        /// <param name="room">The current room</param>
        /// <param name="player">The player</param>
        /// <returns></returns>
        public Item.Item FindItem(string obj, Room room, Player player)
        {
            var nthTarget = Helpers.findNth(obj);
           return Helpers.findRoomObject(nthTarget, room) ?? Helpers.findObjectInInventory(nthTarget, player);
        }

        protected bool CanPerformSkill(Skill.Model.Skill skill, Player player)
        {
            var playerHasSkill = player.Skills.FirstOrDefault(x => x.SkillName.Equals(skill.Name));

            if (playerHasSkill == null)
            {
                Core.Writer.WriteLine($"You do not know this skill.", player.ConnectionId);
                return false;
            }
            
            if (player.Level < playerHasSkill.Level)
            {
                Core.Writer.WriteLine($"You are not of the right level to use this skill.", player.ConnectionId);
                return false;
            }
            
            if (skill.ManaCost > player.Attributes.Attribute[EffectLocation.Mana])
            {
                Core.Writer.WriteLine($"You do not have enough mana to cast {skill.Name}.", player.ConnectionId);
                return false;
            }
               
            if (skill.MoveCost > player.Attributes.Attribute[EffectLocation.Moves])
            {
                Core.Writer.WriteLine($"You are too tired to {skill.Name}.", player.ConnectionId);
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Checks if skill is success or not and displays a generic error or a custom one
        /// </summary>
        /// <param name="player"></param>
        /// <param name="skill"></param>
        /// <param name="customErrorText"></param>
        /// <returns></returns>
        public bool SkillSuccessWithMessage(Player player, Skill.Model.Skill skill, string customErrorText = "")
        {
            var playerSkillProficiency = player.Skills.FirstOrDefault(x => x.SkillName.Equals(skill.Name))?.Proficiency;
            var success = DiceBag.Roll(1, 1, 100);
          
            if (success == 1)
            {
                var errorText = skill.ManaCost > 0
                    ? $"<p>You tried to cast {skill.Name} but failed miserably.</p>"
                    : $"<p>You tried to {skill.Name} but failed miserably.</p>";
                
                Core.Writer.WriteLine(errorText, player.ConnectionId);
                return false;
            }

            if (playerSkillProficiency <= success)
            {
                var failedSkillMessage = customErrorText ?? $"<p>You try to {skill.Name} but fail.</p>";
                
                var errorText = skill.ManaCost > 0
                    ? $"<p>You lost concentration.</p>"
                    : failedSkillMessage;
                
                Core.Writer.WriteLine(errorText, player.ConnectionId);
                return false;
            }

            return true;

        }
        
        public bool SkillSuccess(Player player, Skill.Model.Skill skill)
        {
            var playerSkillProficiency = player.Skills.FirstOrDefault(x => x.SkillName.Equals(skill.Name))?.Proficiency;
            var success = DiceBag.Roll(1, 1, 100);
          
            if (success == 1)
            {
                return false;
            }

            return !(playerSkillProficiency <= success);
        }

    
}