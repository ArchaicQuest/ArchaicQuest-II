using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Spell.Interface;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill
{

    public interface ISKill
    {
        void PerfromSkill(string skillName, Player origin, string targetName, Room room = null);
    }
    public class DoSkill: ISKill
    {
        private readonly IWriteToClient _writer;
        private readonly ISpellTargetCharacter _spellTargetCharacter;
        private readonly ICache _cache;
        private readonly IDamage _damage;
        private readonly IUpdateClientUI _updateClientUi;
        private readonly IMobScripts _mobScripts;
        private readonly IDice _dice;
        private readonly ISkillList _skillList;


        public DoSkill(IWriteToClient writer, ISpellTargetCharacter spellTargetCharacter, ICache cache, IDamage damage, IUpdateClientUI updateClientUi, IMobScripts mobScripts, IDice dice, ISkillList skillList)
        {
            _writer = writer;
            _spellTargetCharacter = spellTargetCharacter;
            _cache = cache;
            _damage = damage;
            _updateClientUi = updateClientUi;
            _mobScripts = mobScripts;
            _dice = dice;
            _skillList = skillList;

        }

        public bool ValidStatus(Player player)
        {
            switch (player.Status)
            {
                case CharacterStatus.Status.Sleeping:
                    _writer.WriteLine("You can't do this while asleep.");
                    return false;
                case CharacterStatus.Status.Stunned:
                    _writer.WriteLine("You are stunned.");
                    return false;
                case CharacterStatus.Status.Dead:
                case CharacterStatus.Status.Ghost:
                case CharacterStatus.Status.Incapacitated:
                    _writer.WriteLine("You can't do this while dead.");
                    return false;
                case CharacterStatus.Status.Resting:
                case CharacterStatus.Status.Sitting:
                    _writer.WriteLine("You need to stand up before you do that.");
                    return false;
                case CharacterStatus.Status.Busy:
                    _writer.WriteLine("You can't do that right now.");
                    return false;
                default:
                    return true;
            }
        }


        public bool SkillSuccess(Player origin, Player target, Skill.Model.Skill spell)
        {
            var skill = origin.Skills.FirstOrDefault(x => x.SkillId.Equals(spell.Id));

            if (skill == null)
            {
                // TODO: log error, we should never get here.
                return false;
            }

            var proficiency = skill.Proficiency;
            var success = spell.Damage.Roll(1, 1,
                101);

            if (success == 1 || success == 101)
            {
                _writer.WriteLine($"<p>You got distracted.</p>", origin.ConnectionId);
                return false;
            }

            if (proficiency < success)
            {
                _writer.WriteLine($"<p>You lost concentration.</p>", origin.ConnectionId);
                return false;
            }

            return true;

        }

        public Skill.Model.Skill FindSkill(string skillName, Player player)
        {
            var foundSkill = player.Skills.FirstOrDefault(x => x.SkillName.StartsWith(skillName, StringComparison.CurrentCultureIgnoreCase));

            if (foundSkill == null)
            {
                _writer.WriteLine($"You don't know a skill that begins with {skillName}");
                return null;
            }

            var skill = _cache.GetSkill(foundSkill.SkillId);

            return skill;
        }

        public bool AffectsCharacter(Skill.Model.Skill spell)
        {

            return (spell.ValidTargets & ValidTargets.TargetPlayerWorld) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetFightVictim) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetSelfOnly) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetPlayerRoom) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetFightSelf) != 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="room"></param>
        public void PerfromSkill(string skillName, Player origin, string targetName = "", Room room = null)
        {


            if (!ValidStatus(origin))
            {
                return;
            }

            var FoundSkill = FindSkill(skillName, origin);

            if (FoundSkill == null)
            {
                return;
            }

            //if (!ManaCheck(spell, origin))
            //{
            //    return;
            //}

            // saves




            if (AffectsCharacter(FoundSkill))
            {
                Player target = null;
                target = _spellTargetCharacter.ReturnTarget(FoundSkill, targetName, room, origin);

                if (target == null)
                {
                    return;
                }
                var spellSkill = origin.Skills.FirstOrDefault(x => x.SkillId.Equals(FoundSkill.Id));
                //saving throw
                if (FoundSkill.Type == SkillType.Affect)
                {


                    var savingThrow = target.Attributes.Attribute[EffectLocation.Intelligence] / 2;

                    var rollForSave = _dice.Roll(1, 1, 100);

                    if (rollForSave <= savingThrow)
                    {
                        //save
                        // half spell affect duration


                        if (rollForSave == 1)
                        {
                            // fail
                            FoundSkill.Rounds = origin.Level / 2;
                        }

                        if (rollForSave != 1)
                        {
                            FoundSkill.Rounds = origin.Level / 4;
                        }
                    }


                }


            


                // spell lag
                // add lag property to player
                // lag == spell round
                // stops spell/skill spam
                // applies after spell is cast
                // is it needed?

                // hit / miss messages

                //  _writer.WriteLine(spell.SkillStart.ToPlayer);

                if (SkillSuccess(origin, target, FoundSkill))
                {


                    //if (spell.Type == SkillType.Damage)
                    //{

                    //    var savingThrow = origin.Attributes.Attribute[EffectLocation.Dexterity] / 2;

                    //    var rollForSave = _dice.Roll(1, 1, 100);

                    //    if (rollForSave <= savingThrow || rollForSave == 100)
                    //    {
                    //        if (rollForSave > 1)
                    //        {
                    //            damage /= 2;

                    //            _writer.WriteLine("You partly evade " + origin.Name + "'s " + spell.Name + " by jumping back.", target.ConnectionId);

                    //            _writer.WriteLine($"{target.Name} partly evades your " + spell.Name + " by jumping back.", origin.ConnectionId);

                    //            foreach (var pc in room.Players)
                    //            {
                    //                if (pc.ConnectionId.Equals(origin.ConnectionId) ||
                    //                    pc.ConnectionId.Equals(target.ConnectionId))
                    //                {
                    //                    continue;
                    //                }

                    //                _writer.WriteLine($"{target.Name} partly evades {origin.Name}'s " + spell.Name + " by jumping back.", pc.ConnectionId);
                    //            }
                    //        }
                    //    }
                    //}


                    var skillTarget = new SkillTarget
                    {
                        Origin = origin,
                        Target = target,
                        Room = room,
                        Skill = FoundSkill
                    };




                    if (string.IsNullOrEmpty(FoundSkill.Formula) && FoundSkill.Type == SkillType.Damage)
                    {
                        //do this for cast cure
                        _skillList.DoSkill(FoundSkill.Name, "", target, "", origin, room, false);

                    }




                    if (FoundSkill.Type != SkillType.Damage)
                    {

                        _skillList.DoSkill(FoundSkill.Name, "", target, "", origin, room, false);
                    }
                }
                else
                {
                    var skill = origin.Skills.FirstOrDefault(x => x.SkillId.Equals(FoundSkill.Id));

                    if (skill == null)
                    {
                        return;
                    }

                    if (skill.Proficiency == 95)
                    {
                        return;
                    }

                    var increase = new Dice().Roll(1, 1, 3);

                    skill.Proficiency += increase;

                    origin.Experience += 100;
                    origin.ExperienceToNextLevel -= 100;

                    _updateClientUi.UpdateExp(origin);

                    _writer.WriteLine(
                        $"<p class='improve'>You learn from your mistakes and gain 100 experience points.</p>",
                        origin.ConnectionId);
                    _writer.WriteLine(
                        $"<p class='improve'>Your {skill.SkillName} skill increases by {increase}%.</p>",
                        origin.ConnectionId);
                }

            }
            else
            {
                _writer.WriteLine(
                    $"<p>You cannot cast this spell upon another.</p>",
                    origin.ConnectionId);
            }

        }
 
    }
}
