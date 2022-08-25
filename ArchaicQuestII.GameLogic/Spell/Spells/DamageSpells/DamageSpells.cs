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
        // Offensive
        int MagicMissile(Player player, Player target, Room room);

        // Defensive
        void Armor(Player player, Player target, Room room, bool wearOff);

        // Utility
        int CauseLightWounds(Player player, Player target, Room room);
        void Bless(Player player, Player target, Room room, bool wearOff);
        void CureLightWounds(Player player, Player target, Room room);
        void Identify(Player player, string fullCommand, Room room);
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

            _skillManager.DamagePlayer("magic missile", damage, player, target, room);

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
            _skillManager.EmoteAction(player, target, room, skillMessage);

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
                    Modifier = new Modifier()
                    {
                        Armour = 10,
                        DamRoll = 10,
                    },
                    Benefits = "Affects armour by 20\r\n Affects Dam by 10",
                    Affects = DefineSpell.SpellAffect.ArmorClass,
                    Duration = player.Level + player.Attributes.Attribute[EffectLocation.Intelligence] / 2,
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

            var skillMessageNoEffect = new SkillMessage()
            {
                NoEffect = new Messages(),
                Hit = new Messages()
                {
                    ToPlayer = "#target# is at full health.",
                    ToTarget = "You are at full health.",
                    ToRoom = "#target# is at full health."
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
            else
            {
                _skillManager.EmoteAction(player, target, room, skillMessageNoEffect);
                _skillManager.UpdateClientUI(target);
            }

            _skillManager.UpdateClientUI(player);

        }

        public void Identify(Player player, string obj, Room room)
        {

            if (string.IsNullOrEmpty(obj))
            {
                _writer.WriteLine("Identify what!?", player.ConnectionId);
                return;
            }
            var item = player.Inventory.FirstOrDefault(x => x.Name.Contains(obj, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                _writer.WriteLine($"You don't have an item starting with '{item}'", player.ConnectionId);
                return;
            }

            var sb = new StringBuilder();


            List<string> itemFlags = new List<string>();
            foreach (Item.Item.ItemFlags itemFlag in Enum.GetValues(typeof(Item.Item.ItemFlags)))
            {
                if ((item.ItemFlag & itemFlag) != 0) itemFlags.Add(itemFlag.ToString());
            }

            sb.Append($"<p>Object '{item.Name}' is type {item.ItemType}, extra flags: {(itemFlags.Any() ? String.Join(", ", itemFlags) : "none")}.<br /><br />");
            sb.Append($"Weight is {item.Weight}, value is {item.Value}, level is {item.Level}.<br />");

            if (item.ItemType == Item.Item.ItemTypes.Armour)
            {
                sb.Append($"Armour Type: {item.ArmourType}, Defense {item.ArmourRating.Armour} and {item.ArmourRating.Magic} vs magic.<br />");
            }

            if (item.ItemType == Item.Item.ItemTypes.Weapon)
            {
                sb.Append($"Weapon Type: {item.WeaponType}, Damage is {item.Damage.Minimum}-{item.Damage.Maximum} (average {item.Damage.Minimum + item.Damage.Maximum / 2}).<br />");
                sb.Append($"Attack type: {item.AttackType}</br>");
                sb.Append($"Damage type: {item.DamageType}</br>");
            }

            sb.Append($"Affects:</br>");
      
            
            if (item.Modifier.Strength != 0)
            {
                sb.Append($"{(item.Modifier.Strength > 0 ? "+" : "-")}{item.Modifier.Strength} Strength</br>");
            }
            if (item.Modifier.Dexterity != 0)
            {
                sb.Append($"{(item.Modifier.Dexterity > 0 ? "+" : "-")}{item.Modifier.Dexterity} Dexterity</br>");
            }
            if (item.Modifier.Constitution != 0)
            {
                sb.Append($"{(item.Modifier.Constitution > 0 ? "+" : "-")}{item.Modifier.Constitution} Constitution</br>");
            }
            if (item.Modifier.Wisdom != 0)
            {
                sb.Append($"{(item.Modifier.Wisdom > 0 ? "+" : "-")}{item.Modifier.Wisdom} Wisdom</br>");
            }
            if (item.Modifier.Intelligence != 0)
            {
                sb.Append($"{(item.Modifier.Intelligence > 0 ? "+" : "-")}{item.Modifier.Intelligence} Intelligence</br>");
            }
            if (item.Modifier.Charisma != 0)
            {
                sb.Append($"{(item.Modifier.Charisma > 0 ? "+" : "-")}{item.Modifier.Charisma} Charisma</br>");
            }
            
            if (item.Modifier.HP != 0)
            {
                sb.Append($"{(item.Modifier.HP > 0 ? "+" : "-")}{item.Modifier.HP} HP</br>");
            }
            if (item.Modifier.Mana != 0)
            {
                sb.Append($"{(item.Modifier.Mana > 0 ? "+" : "-")}{item.Modifier.Mana} Mana</br>");
            }
            if (item.Modifier.Moves != 0)
            {
                sb.Append($"{(item.Modifier.Moves > 0 ? "+" : "-")}{item.Modifier.Moves} Moves</br>");
            }
            if (item.Modifier.DamRoll != 0)
            {
                sb.Append($"{(item.Modifier.DamRoll > 0 ? "+" : "-")}{item.Modifier.DamRoll} Damage Roll</br>");
            }
            if (item.Modifier.HitRoll != 0)
            {
                sb.Append($"{(item.Modifier.HitRoll > 0 ? "+" : "-")}{item.Modifier.HitRoll} Hit Roll</br>");
            }
            if (item.Modifier.SpellDam != 0)
            {
                sb.Append($"{(item.Modifier.SpellDam > 0 ? "+" : "-")}{item.Modifier.SpellDam} Spell Damage Roll</br>");
            }
            if (item.Modifier.AcMod != 0)
            {
                sb.Append($"{(item.Modifier.AcMod > 0 ? "+" : "-")}{item.Modifier.AcMod} Armour</br>");
            }
            if (item.Modifier.AcMagicMod != 0)
            {
                sb.Append($"{(item.Modifier.AcMagicMod > 0 ? "+" : "-")}{item.Modifier.AcMagicMod} Magic Armour</br>");
            }


            // TODO: container? Affects? what else? 
            // show crafted by
            // show enchanted by

            sb.Append("</p>");

            _writer.WriteLine(sb.ToString(), player.ConnectionId);

        }
    }
}
