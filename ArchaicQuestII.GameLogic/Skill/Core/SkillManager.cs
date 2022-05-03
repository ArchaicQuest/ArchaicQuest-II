using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Spell.Interface;
using ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill.Core
{
    public class SkillManager : ISkillManager
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _updateClientUi;
        private readonly IDice _dice;
        private readonly IDamage _damage;
        private readonly ICombat _fight;



        public SkillManager(IWriteToClient writer, IUpdateClientUI updateClientUi, IDice dice, IDamage damage, ICombat fight)
        {
            _writer = writer;
            _updateClientUi = updateClientUi;
            _dice = dice;
            _damage = damage;
            _fight = fight;

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
            return _fight.FindTarget(player, target, room, murder);

        }

        public void updateCombat(Player player, Player target, Room room)
        {
            if (target != null)
            {

                if (_fight.IsTargetAlive(target))
                {

                    _fight.InitFightStatus(player, target);
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

        public void DamagePlayer(string spellName, int damage, Player player, Player target, Room room)
        {

            if (_fight.IsTargetAlive(target))
            {

                var totalDam = _fight.CalculateSkillDamage(player, target, damage);

                _writer.WriteLine(
                    $"<p>Your {spellName} {_damage.DamageText(totalDam).Value} {target.Name}  <span class='damage'>[{damage}]</span></p>",
                    player.ConnectionId);
                _writer.WriteLine(
                    $"<p>{player.Name}'s {spellName} {_damage.DamageText(totalDam).Value} you!  <span class='damage'>[{damage}]</span></p>",
                    target.ConnectionId);

                foreach (var pc in room.Players)
                {
                    if (pc.ConnectionId.Equals(player.ConnectionId) ||
                        pc.ConnectionId.Equals(target.ConnectionId))
                    {
                        continue;
                    }

                    _writer.WriteLine(
                        $"<p>{player.Name}'s {spellName} {_damage.DamageText(totalDam).Value} {target.Name}  <span class='damage'>[{damage}]</span></p>",
                        pc.ConnectionId);

                }

                target.Attributes.Attribute[EffectLocation.Hitpoints] -= totalDam;

                if (!_fight.IsTargetAlive(target))
                {
                    _fight.TargetKilled(player, target, room);

                    _updateClientUi.UpdateHP(target);
                    return;
                    //TODO: create corpse, refactor fight method from combat.cs
                }

                //update UI
                _updateClientUi.UpdateHP(target);

                _fight.AddCharToCombat(target);
                _fight.AddCharToCombat(player);
            }

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
                _writer.WriteLine(ReplacePlaceholders(noAffect, target, false), player.ConnectionId);
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

            _updateClientUi.UpdateAffects(target);
            _updateClientUi.UpdateScore(target);
        }

        public void UpdateClientUI(Player player)
        {
            //update UI
            _updateClientUi.UpdateHP(player);
            _updateClientUi.UpdateMana(player);
            _updateClientUi.UpdateMoves(player);
            _updateClientUi.UpdateScore(player);
        }

        public void EmoteAction(Player player, Player target, Room room, SkillMessage emote)
        {
            if (target.ConnectionId == player.ConnectionId)
            {
                _writer.WriteLine(
                    $"<p>{ReplacePlaceholders(emote.Hit.ToPlayer, target, true)}</p>",
                    target.ConnectionId);
            }
            else
            {
                _writer.WriteLine(
                    $"<p>{ReplacePlaceholders(emote.Hit.ToPlayer, target, false)}</p>",
                    player.ConnectionId);
            }


            if (!string.IsNullOrEmpty(emote.Hit.ToTarget))
            {
                _writer.WriteLine(
                    $"<p>{emote.Hit.ToTarget}</p>",
                    target.ConnectionId);
            }

            foreach (var pc in room.Players)
            {
                if (pc.ConnectionId.Equals(player.ConnectionId) ||
                    pc.ConnectionId.Equals(target.ConnectionId))
                {
                    continue;
                }

                _writer.WriteLine($"<p>{ReplacePlaceholders(emote.Hit.ToRoom, target, false)}</p>",
                    pc.ConnectionId);

            }
        }

        public void EmoteEffectWearOffAction(Player player, Room room, SkillMessage emote)
        {

            foreach (var pc in room.Players)
            {
                if (pc.ConnectionId.Equals(player.ConnectionId))
                {
                    _writer.WriteLine($"<p>{emote.EffectWearOff.ToPlayer}</p>",
                        pc.ConnectionId);
                    continue;
                }

                _writer.WriteLine($"<p>{ReplacePlaceholders(emote.EffectWearOff.ToRoom, player, false)}</p>",
                    pc.ConnectionId);

            }
        }

    }
}
