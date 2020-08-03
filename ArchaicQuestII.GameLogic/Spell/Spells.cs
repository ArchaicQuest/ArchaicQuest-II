
using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Spell.Interface;
using ArchaicQuestII.GameLogic.World.Room;
using LiteDB;

namespace ArchaicQuestII.GameLogic.Spell
{
    public class Spells : ISpells
    {
        private readonly IWriteToClient _writer;
        private readonly ISpellTargetCharacter _spellTargetCharacter;
        private readonly ICache _cache;
        private readonly IDamage _damage;
        private readonly IUpdateClientUI _updateClientUi;
        public Spells(IWriteToClient writer, ISpellTargetCharacter spellTargetCharacter, ICache cache, IDamage damage, IUpdateClientUI updateClientUi)
        {
            _writer = writer;
            _spellTargetCharacter = spellTargetCharacter;
            _cache = cache;
            _damage = damage;
            _updateClientUi = updateClientUi;
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

        public Skill.Model.Skill FindSpell(string skill, Player player)
        {
             var foundSpell = player.Skills.FirstOrDefault(x => x.SkillName.StartsWith(skill, StringComparison.CurrentCultureIgnoreCase));

             if (foundSpell == null)
             {
                 _writer.WriteLine($"You don't know a spell that begins with {skill}");
                 return null;
             }

             var spell = _cache.GetSkill(foundSpell.SkillId);

             return spell;
        }

        public bool ManaCheck(Skill.Model.Skill spell, Player player)
        {
            if (player.Attributes.Attribute[EffectLocation.Mana] < spell.Cost.Table[Cost.Mana])
            {
                _writer.WriteLine("You don't have enough mana.", player.ConnectionId);
                return false;
            }

            return true;
        }

        public bool SpellAffectsCharacter(Skill.Model.Skill spell)
        {
            
            return (spell.ValidTargets & ValidTargets.TargetPlayerWorld) != 0 || 
                   (spell.ValidTargets & ValidTargets.TargetFightVictim) != 0 || 
                   (spell.ValidTargets & ValidTargets.TargetSelfOnly)    != 0 || 
                   (spell.ValidTargets & ValidTargets.TargetPlayerRoom)  != 0 || 
                   (spell.ValidTargets & ValidTargets.TargetFightSelf)   != 0;
        }

        public void ReciteSpellCharacter(Player origin, Player target, Skill.Model.Skill spell)
        {
            // not correct need to send to room 
            if (origin.Id == target.Id)
            {
                _writer.WriteLine(
                    $"{origin.Name} closes {Helpers.GetPronoun(origin.Gender)} eyes and utters the words, '{spell.Name}'.");
            }
            else if (origin != target)
            {
                _writer.WriteLine($"{origin.Name} stares at {target.Name} and utters the words, '{spell.Name}'.");
            }

        }

        public bool SpellSuccess(Player origin, Player target, Skill.Model.Skill spell)
        {
            var spellSkill = origin.Skills.FirstOrDefault(x => x.SkillId.Equals(spell.Id)).Proficiency * 100;

            var success = spell.Damage.Roll(1, 1,
                101);

            if (success == 1 || success == 101)
            {
                _writer.WriteLine($"<p>You got distracted.</p>", origin.ConnectionId);
                return false;
            }

            if (spellSkill < success)
            {
                _writer.WriteLine($"<p>You lost concentration.</p>", origin.ConnectionId);
                return false;
            }

            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="room"></param>
        public void DoSpell(string spellName, Player origin, string targetName = "", Room room = null)
        {

            if (!ValidStatus(origin))
            {
                return;
            }

            var spell = FindSpell(spellName, origin);

            if (spell == null)
            {
                return;
            }

            if (!ManaCheck(spell, origin))
            {
                return;
            }

          
          if (SpellAffectsCharacter(spell)) {
              Player target = null;
              target =  _spellTargetCharacter.ReturnTarget(spell, targetName, room, origin);

              if (target == null)
              {
                  return;
              }

              ReciteSpellCharacter(origin, target, spell);

              var formula = spell.Damage.Roll(spell.Damage.DiceRoll, spell.Damage.DiceMinSize,
                                spell.Damage.DiceMaxSize) + (origin.Level + 1) / 2; //+ mod

              

              //deduct mana
              origin.Attributes.Attribute[EffectLocation.Mana] -= spell.Cost.Table[Cost.Mana] == 0 ? 5 : spell.Cost.Table[Cost.Mana];
              _updateClientUi.UpdateMana(origin);

               
                  // spell lag
                  // add lag property to player
                  // lag == spell round
                  // stops spell/skill spam
                  // applies after spell is cast
                  // is it needed?

                  // hit / miss messages

                  //  _writer.WriteLine(spell.SkillStart.ToPlayer);

                  if (SpellSuccess(origin, target, spell))
                  {

                      

                      var skillTarget = new SkillTarget
                      {
                          Origin = origin,
                          Target = target,
                          Room = room,
                          Skill = spell
                      };

                      _writer.WriteLine(
                          $"<p>Your {spell.Name} {_damage.DamageText(formula).Value} {target.Name} ({formula})</p>",
                          origin.ConnectionId);
                      _writer.WriteLine(
                          $"<p>{origin.Name}'s {spell.Name} {_damage.DamageText(formula).Value} you! ({formula})</p>",
                          target.ConnectionId);

                      // If no effect assume, negative spell and deduct HP
                      if (spell.Effect == null)
                      {
                          skillTarget.Target.Attributes.Attribute[EffectLocation.Hitpoints] -= formula;
                          //update UI
                          _updateClientUi.UpdateHP(skillTarget.Target);


                      }
                      else
                      {
                          new SpellEffect(_writer, skillTarget, formula).Type[skillTarget.Skill.Type].Invoke();
                      }

                  }
                  else
                  {
                      var skill = origin.Skills.FirstOrDefault(x => x.SkillId.Equals(spell.Id));

                      skill.Proficiency += (double)origin.Level - skill.Level;

                }

            }

        }

    }

}

