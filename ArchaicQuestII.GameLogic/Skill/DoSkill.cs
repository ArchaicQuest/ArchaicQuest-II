using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Spell.Interface;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill
{

    public interface ISKill
    {
        void PerfromSkill(Model.Skill skill, string command, Player origin, string targetName, Room room = null);
    }
    public class DoSkill : ISKill
    {
        private readonly IWriteToClient _writer;
        private readonly ISpellTargetCharacter _spellTargetCharacter;
        private readonly ICache _cache;
        private readonly IDamage _damage;
        private readonly IUpdateClientUI _updateClientUi;
        private readonly IMobScripts _mobScripts;
        private readonly ISkillList _skillList;


        public DoSkill(
            IWriteToClient writer, 
            ISpellTargetCharacter spellTargetCharacter, 
            ICache cache, 
            IDamage damage, 
            IUpdateClientUI updateClientUi, 
            IMobScripts mobScripts,
            ISkillList skillList)
        {
            _writer = writer;
            _spellTargetCharacter = spellTargetCharacter;
            _cache = cache;
            _damage = damage;
            _updateClientUi = updateClientUi;
            _mobScripts = mobScripts;
            _skillList = skillList;

        }

        public bool ValidStatus(Player player)
        {
            switch (player.Status)
            {
                case CharacterStatus.Status.Sleeping:
                    _writer.WriteLine("You can't do this while asleep.");
                    return false;
                //case CharacterStatus.Status.Stunned:
                //    _writer.WriteLine("You are stunned.");
                //    return false;
                //case CharacterStatus.Status.Dead:
                //case CharacterStatus.Status.Ghost:
                //case CharacterStatus.Status.Incapacitated:
                //    _writer.WriteLine("You can't do this while dead.");
                //    return false;
                //case CharacterStatus.Status.Resting:
                //case CharacterStatus.Status.Sitting:
                //    _writer.WriteLine("You need to stand up before you do that.");
                //    return false;
                //case CharacterStatus.Status.Busy:
                //    _writer.WriteLine("You can't do that right now.");
                //    return false;
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
            var success = DiceBag.Roll(1, 1,
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

        public Skill.Model.Skill FindSkill(Model.Skill skill, string skillName, Player player)
        {
            var foundSkill = player.Skills.FirstOrDefault(x => x.SkillName.StartsWith(skillName, StringComparison.CurrentCultureIgnoreCase) && x.Level <= player.Level);

            if (foundSkill == null)
            {
                _writer.WriteLine($"You don't know a skill that begins with {skillName}", player.ConnectionId);
                return null;
            }

            if (foundSkill.SkillId == 0)
            {
                foundSkill.SkillId = skill.Id;
            }

            var getSkill = _cache.ReturnSkills().FirstOrDefault(x => x.Name.Equals(foundSkill.SkillName));
            if (getSkill == null)
            {
                //player skill id mismatch as not using a db no more and these are generated, chance of player not having the correct id
                foundSkill.SkillId = skill.Id;
                getSkill = skill;
            }

            foundSkill.SkillId = getSkill.Id;


            return getSkill;
        }

        public bool AffectsCharacter(Skill.Model.Skill spell)
        {

            return (spell.ValidTargets & ValidTargets.TargetPlayerWorld) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetFightVictim) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetSelfOnly) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetPlayerRoom) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetFightSelf) != 0;
        }

        public bool AffectsSelf(Skill.Model.Skill spell)
        {

            return
                   (spell.ValidTargets & ValidTargets.TargetSelfOnly) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetFightSelf) != 0;
        }
        
        public bool AffectsObject(Skill.Model.Skill spell)
        {

            return
                (spell.ValidTargets & ValidTargets.TargetObjectEquipped) != 0 ||
                (spell.ValidTargets & ValidTargets.TargetObjectRoom) != 0 ||
                (spell.ValidTargets & ValidTargets.TargetObjectWorld) != 0 ||
                (spell.ValidTargets & ValidTargets.TargetObjectInventory) != 0;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="room"></param>
        public void PerfromSkill(Model.Skill performSkill, string command, Player origin, string targetName = "", Room room = null)
        {


            if (!ValidStatus(origin))
            {
                return;
            }

            var FoundSkill = FindSkill(performSkill, command, origin);

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
                // check if affects a target
                // if target blank
                // error with to  kick whome?

                if (!AffectsSelf(FoundSkill) && (origin.Status & CharacterStatus.Status.Fighting) == 0 && (!string.IsNullOrEmpty(targetName) && targetName == command))
                {
                    _writer.WriteLine(FoundSkill.Name + " whom?");
                    return;
                }

                Player target = null;

                if (targetName == command)
                {
                    targetName = string.Empty;
                }
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

                    var rollForSave = DiceBag.Roll(1, 1, 100);

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

                    var increase = DiceBag.Roll(1, 1, 3);

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
            else if (AffectsObject(FoundSkill))
            {
                if (SkillSuccess(origin, null, FoundSkill))
                {
                    
                    if (FoundSkill.Type != SkillType.Passive)
                    {
                        _skillList.DoSkill(FoundSkill.Name, targetName, null, "", origin, room, false);
                    }
                }
                else
                {
                    
                }
            }
            else
            {
                if (FoundSkill.Name.Equals("Axe"))
                {
                    _writer.WriteLine(
                    $"<p>What!? I don't understand.</p>",
                    origin.ConnectionId);
                    return;
                }
                _writer.WriteLine(
                    $"<p>You cannot use this upon another.</p>",
                    origin.ConnectionId);
            }

        }

    }
}
