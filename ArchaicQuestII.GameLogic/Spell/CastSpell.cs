
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Core;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Spell.Interface; 
using ArchaicQuestII.GameLogic.World.Room;
using LiteDB;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.Spell
{
    public class CastSpell : ISpells
    {
        private readonly IWriteToClient _writer;
        private readonly ISpellTargetCharacter _spellTargetCharacter;
        private readonly ICache _cache;
        private readonly IDamage _damage;
        private readonly IUpdateClientUI _updateClientUi;
        private readonly IMobScripts _mobScripts;
        private readonly IDice _dice;
        private readonly ISpellList _spellList;
        public CastSpell(IWriteToClient writer, ISpellTargetCharacter spellTargetCharacter, ICache cache, IDamage damage, IUpdateClientUI updateClientUi, IMobScripts mobScripts, IDice dice, ISpellList spellList)
        {
            _writer = writer;
            _spellTargetCharacter = spellTargetCharacter;
            _cache = cache;
            _damage = damage;
            _updateClientUi = updateClientUi;
            _mobScripts = mobScripts;
            _dice = dice;
            _spellList = spellList;
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
                   (spell.ValidTargets & ValidTargets.TargetSelfOnly) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetPlayerRoom) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetFightSelf) != 0;
        }


        public string ObsfucateSpellName(string spellName)
        {

            var magicWords = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>(" ", " "),
                new Tuple<string, string>("ar", "abra"),
                new Tuple<string, string>("au", "kada"),
                new Tuple<string, string>("bless", "fido"),
                new Tuple<string, string>("blind", "nose"),
                new Tuple<string, string>("bur", "mosa"),
                new Tuple<string, string>("cu", "judi"),
                new Tuple<string, string>("de", "oculo"),
                new Tuple<string, string>("en", "unso"),
                new Tuple<string, string>("light", "dies"),
                new Tuple<string, string>("lo", "hi"),
                new Tuple<string, string>("mor", "zak"),
                new Tuple<string, string>("move", "sido"),
                new Tuple<string, string>("ness", "lacri"),
                new Tuple<string, string>("ning", "illa"),
                new Tuple<string, string>("per", "duda"),
                new Tuple<string, string>("ra", "gru"),
                new Tuple<string, string>("fresh", "ima"),
                new Tuple<string, string>("re", "candus"),
                new Tuple<string, string>("son", "sabru"),
                new Tuple<string, string>("tect", "infra"),
                new Tuple<string, string>("tri", "cula"),
                new Tuple<string, string>("ven", "nofo"),
                new Tuple<string, string>("a", "a"), new Tuple<string, string>("b", "b"),
                new Tuple<string, string>("c", "q"), new Tuple<string, string>("d", "e"),
                new Tuple<string, string>("e", "z"), new Tuple<string, string>("f", "y"),
                new Tuple<string, string>("g", "o"), new Tuple<string, string>("h", "p"),
                new Tuple<string, string>("i", "u"), new Tuple<string, string>("j", "y"),
                new Tuple<string, string>("k", "t"), new Tuple<string, string>("l", "r"),
                new Tuple<string, string>("m", "w"), new Tuple<string, string>("n", "i"),
                new Tuple<string, string>("o", "a"), new Tuple<string, string>("p", "s"),
                new Tuple<string, string>("q", "d"), new Tuple<string, string>("r", "f"),
                new Tuple<string, string>("s", "g"), new Tuple<string, string>("t", "h"),
                new Tuple<string, string>("u", "j"), new Tuple<string, string>("v", "z"),
                new Tuple<string, string>("w", "x"), new Tuple<string, string>("x", "n"),
                new Tuple<string, string>("y", "l"), new Tuple<string, string>("z", "k"),
                new Tuple<string, string>("", "")
            };

            var buff = new StringBuilder();
            var pos = 0;
            var length = 0;

            while (pos < spellName.Length)
            {

                foreach (var magicWord in magicWords)
                {
                    var prefix = magicWord.Item1;
                    var len = prefix.Length;

                    if (pos + len <= spellName.Length && spellName.IndexOf(prefix, pos, len, StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        buff.Append(magicWord.Item2);
                        length = len;
                        break;
                    }
                }

                if (length > 0)
                {
                    pos += length;
                }
            }

            return buff.ToString();
        }

        public void ReciteSpellCharacter(Player origin, Player target, Skill.Model.Skill spell, Room room)
        {
            // not correct need to send to room 
            if (origin.Id == target.Id)
            {

                _writer.WriteLine(
                    $"You close your eyes and utters the words, '{spell.Name}'.", origin.ConnectionId);

                foreach (var pc in room.Players)
                {
                    if (pc.ConnectionId.Equals(origin.ConnectionId))
                    {
                        continue;
                    }
                    _writer.WriteLine($"{origin.Name} closes {Helpers.GetPronoun(origin.Gender)} eyes and utters the words, '{spell.Name}'.", pc.ConnectionId);
                }


            }
            else if (origin != target)
            {
                
                var obsfucatedSpellName = ObsfucateSpellName(spell.Name);
               

                _writer.WriteLine($"You look at {target.Name} and utter the words, '{spell.Name}'.", origin.ConnectionId);
                _writer.WriteLine($"{origin.Name} looks at you and utters the words, '{(!Helpers.isCaster(target.ClassName) ? obsfucatedSpellName : spell.Name)}'.", target.ConnectionId);
                foreach (var pc in room.Players)
                {
                    if (pc.ConnectionId.Equals(origin.ConnectionId) ||
                        pc.ConnectionId.Equals(target.ConnectionId))
                    {
                        continue;
                    }
                    _writer.WriteLine($"{origin.Name} looks at {target.Name} and utters the words, '{(!Helpers.isCaster(target.ClassName) ? obsfucatedSpellName : spell.Name)}'.", pc.ConnectionId);
                }
               
            }

        }

        public bool SpellSuccess(Player origin, Player target, Skill.Model.Skill spell)
        {
            var spellSkill = origin.Skills.FirstOrDefault(x => x.SkillId.Equals(spell.Id));

            if (spellSkill == null)
            {
                // TODO: log error, we should never get here.
                return false;
            }

            var spellProficiency = spellSkill.Proficiency;
            var success = spell.Damage.Roll(1, 1,
                101);

            if (success == 1 || success == 101)
            {
                _writer.WriteLine($"<p>You got distracted.</p>", origin.ConnectionId);
                return false;
            }

            if (spellProficiency < success)
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

            //if (!ManaCheck(spell, origin))
            //{
            //    return;
            //}

            // saves

           


            if (SpellAffectsCharacter(spell))
            {
                Player target = null;
                target = _spellTargetCharacter.ReturnTarget(spell, targetName, room, origin);

                if (target == null)
                {
                    return;
                }
                var spellSkill = origin.Skills.FirstOrDefault(x => x.SkillId.Equals(spell.Id));
                //saving throw
                if (spell.Type == SkillType.Affect)
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
                            spell.Rounds = origin.Level / 2;
                        }

                        if (rollForSave != 1)
                        {
                            spell.Rounds = origin.Level / 4;
                        }
                    }


                }
 
               
                ReciteSpellCharacter(origin, target, spell, room);

             

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
                        Skill = spell
                    };


 

                    if (string.IsNullOrEmpty(spell.Formula) && spell.Type == SkillType.Damage)
                    {
                        //do this for cast cure
                         _spellList.CastSpell(spell.Name, "", target, "", origin, room, false);

                    }

            
                 

                    if (spell.Type != SkillType.Damage)
                    {

                     _spellList.CastSpell(spell.Name, "", target, "", origin, room, false);
                    }
                }
                else
                {
                    var skill = origin.Skills.FirstOrDefault(x => x.SkillId.Equals(spell.Id));

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

        public void DamagePlayer(string spellName, int damage, Player player, Player target, Room room)
        {
            _writer.WriteLine(
                $"<p>Your {spellName} {_damage.DamageText(damage).Value} {target.Name} ({damage})</p>",
                player.ConnectionId);
            _writer.WriteLine(
                $"<p>{player.Name}'s {spellName} {_damage.DamageText(damage).Value} you! ({damage})</p>",
                target.ConnectionId);

            foreach (var pc in room.Players)
            {
                if (pc.ConnectionId.Equals(player.ConnectionId) ||
                    pc.ConnectionId.Equals(target.ConnectionId))
                {
                    continue;
                }

                _writer.WriteLine($"<p>{ player.Name}'s {spellName} {_damage.DamageText(damage).Value} {target.Name} ({damage}))</p>",
                    pc.ConnectionId);

            }

            target.Attributes.Attribute[EffectLocation.Hitpoints] -= damage;
            //update UI
            _updateClientUi.UpdateHP(target);
        }
    }

}

