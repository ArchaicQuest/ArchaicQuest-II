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
using ArchaicQuestII.GameLogic.Skill.Core;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Spell.Interface;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells
{
    public interface IDamageSpells
    {
        int MagicMissile(Player player, Player target, Room room);
        int CauseLightWounds(Player player, Player target, Room room);
        void Armor(Player player, Player target, Room room, bool wearOff);
        void Bless(Player player, Player target, Room room, bool wearOff);
        void CureLightWounds(Player player, Player target, Room room);
    }

    public class DamageSpells : IDamageSpells
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _updateClientUi;
        private readonly IDice _dice;
        private readonly IDamage _damage;
        private readonly ICombat _fight;
        private readonly ISkillManager _skillManager;



        public DamageSpells(IWriteToClient writer, IUpdateClientUI updateClientUi, IDice dice, IDamage damage, ICombat fight, ISkillManager skillManager)
        {
            _writer = writer;
            _updateClientUi = updateClientUi;
            _dice = dice;
            _damage = damage;
            _fight = fight;
            _skillManager = skillManager;

        }

        
      
        public int MagicMissile(Player player, Player target, Room room)
        {
            var damage = _dice.Roll(1, 1, 4) + 1;

            _skillManager.DamagePlayer("magic missile", damage, player, target, room );

            return damage;
        }

        public int CauseLightWounds(Player player, Player target, Room room)
        {
            var casterLevel = player.Level > 10 ? 5 : player.Level;
            var damage = _dice.Roll(1, 1, 8) + casterLevel;

            _skillManager.DamagePlayer("Cause light wounds", damage, player, target, room);

            return damage;
        }

        public void Armor(Player player, Player target, Room room, bool wearOff)
        {

            var skillMessage = new SkillMessage()
            {
                NoEffect = new Messages()
                {
                    ToPlayer = "A protective white light engulfs #target#."
                },
                Hit = new Messages()
                {
                    ToPlayer = "#target# is engulfed by a protective white light.",
                    ToTarget = "You feel protected.",
                    ToRoom = "A protective white light engulfs #target#."
                },
                Death = new Messages(),
                Miss = new Messages(),
                EffectWearOff = new Messages()
                {
                    ToPlayer = "Your protective white light fades away.",
                    ToRoom = "#target#'s protective white light fades away."
                }
            };

            var affect = target.Affects.Custom.FirstOrDefault(x =>
                x.Name.Equals("Armour", StringComparison.CurrentCultureIgnoreCase));

            if (wearOff)
            {
                player.ArmorRating.Armour -= 20;


              target.Affects.Custom.Remove(affect);

              _skillManager.EmoteEffectWearOffAction(player, room, skillMessage);

                _updateClientUi.UpdateAffects(player);
                _updateClientUi.UpdateScore(player);
                return;
            }

            var skill = new AllSpells().Armour();
            target = _skillManager.GetValidTarget(player, target, skill.ValidTargets);
          
            //create emote effectWear off message
            _skillManager.EmoteAction(player,target,room, skillMessage);

            if (affect == null)
            {

                var newAffect = new Affect()
                {
                    Modifier = new Modifier()
                    {
                        Armour = 20
                    },
                    Benefits = "Affects armour by 20",
                    Affects = DefineSpell.SpellAffect.ArmorClass,
                    Duration = player.Level + player.Attributes.Attribute[EffectLocation.Intelligence] / 2,
                    Name = "Armour",

                };

                target.Affects.Custom.Add(newAffect);

                Helpers.ApplyAffects(newAffect, target);
            }
            else
            {
                affect.Duration = player.Level + player.Attributes.Attribute[EffectLocation.Intelligence] / 2;
            }

          

            _updateClientUi.UpdateAffects(target);
            _updateClientUi.UpdateScore(target);
            _updateClientUi.UpdateScore(player);
     

        }

        public void Bless(Player player, Player target, Room room, bool wearOff)
        {

            var skillMessage = new SkillMessage()
            {
                NoEffect = new Messages(),
                Hit = new Messages()
                {
                    ToPlayer = "You lay the blessing of your god upon #target#",
                    ToTarget = "A powerful blessing is laid upon you.",
                    ToRoom = "#caster# beams as a powerful blessing is laid upon #target#."
                },
                Death = new Messages(),
                Miss = new Messages(),
                EffectWearOff = new Messages()
                {
                    ToPlayer = "The blessing fades away.",
                    ToRoom = "#target#'s blessing fades away."
                }
            };

            var affect = target.Affects.Custom.FirstOrDefault(x =>
                x.Name.Equals("Bless", StringComparison.CurrentCultureIgnoreCase));

            if (wearOff)
            {
                target.ArmorRating.Armour -= 20;
                target.Attributes.Attribute[EffectLocation.DamageRoll] -= 20;


                target.Affects.Custom.Remove(affect);

                _skillManager.EmoteEffectWearOffAction(player, room, skillMessage);

                _updateClientUi.UpdateAffects(player);
                _updateClientUi.UpdateScore(player);
                return;
            }

            var skill = new AllSpells().Armour();
            target = _skillManager.GetValidTarget(player, target, skill.ValidTargets);

            //create emote effectWear off message
            _skillManager.EmoteAction(player, target, room, skillMessage);

            if (affect == null)
            {
                var newAffect = new Affect()
                {
                    Modifier = new Modifier() {
                        Armour = 10,
                        DamRoll = 10,},
                    Benefits = "Affects armour by 20\r\n Affects Dam by 10",
                    Affects = DefineSpell.SpellAffect.ArmorClass,
                    Duration =  player.Level + player.Attributes.Attribute[EffectLocation.Intelligence] / 2,
                    Name = "Bless",
                    
                };

            
                target.Affects.Custom.Add(newAffect);

                Helpers.ApplyAffects(newAffect, target);
            }
            else
            {
                affect.Duration = player.Level + player.Attributes.Attribute[EffectLocation.Intelligence] / 2;
            }

            

            _updateClientUi.UpdateAffects(target);
            _updateClientUi.UpdateScore(target);
            _updateClientUi.UpdateScore(player);


        }

        public void CureLightWounds(Player player, Player target, Room room)
        {
            var casterLevel = player.Level > 10 ? 5 : player.Level;
            var value = _dice.Roll(1, 1, 4) + 1 + casterLevel / 4;

            var skillMessage = new SkillMessage()
            {
                NoEffect = new Messages()
                {
                    ToPlayer = "#target# is at full health."
                },
                Hit = new Messages()
                {
                    ToPlayer = "#target# looks better!",
                    ToTarget = "You feel better.",
                    ToRoom = "#target# looks better!"
                },
                Death = new Messages(),
                Miss = new Messages()
            };

         var hasAffcted = _skillManager.AffectPlayerAttributes("Cure light wounds", EffectLocation.Hitpoints, value, player, target, room, skillMessage.NoEffect.ToPlayer);

         if (hasAffcted)
         {
             _skillManager.EmoteAction(player, target, room, skillMessage);
             _skillManager.UpdateClientUI(target);
            }

         _skillManager.UpdateClientUI(player);
           
        }


    }
}
