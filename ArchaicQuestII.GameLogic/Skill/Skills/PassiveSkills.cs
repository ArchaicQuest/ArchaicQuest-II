using System;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Skill.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill.Skills
{
    public interface IPassiveSkills
    {
        int Haggle(Player player, Player target);
        int DualWield(Player player, Player target, Room room, string obj);
        int Lore(Player player, Room room, string obj);
    }

    public class PassiveSkills : IPassiveSkills
    {
        private readonly ISkillManager _skillManager;
        private readonly IEquip _equip;

        public PassiveSkills(ISkillManager skillManager, IEquip equip)
        {
            _skillManager = skillManager;
            _equip = equip;
        }

        public int Haggle(Player player, Player target)
        {
            var foundSkill = player.Skills.FirstOrDefault(x => x.Name == SkillName.Haggle);

            if (foundSkill == null)
            {
                return 0;
            }

            var getSkill = Services.Instance.Cache.GetSkill(foundSkill.Id);

            if (getSkill == null)
            {
                var skill = Services.Instance.Cache
                    .GetAllSkills()
                    .FirstOrDefault(
                        x => x.Name.Equals("haggle", StringComparison.CurrentCultureIgnoreCase)
                    );
                foundSkill.Id = skill.Id;
                getSkill = skill;
            }

            var proficiency = foundSkill.Proficiency;
            var success = DiceBag.Roll(1, 1, 100);

            if (success == 1 || success == 101)
            {
                return 0;
            }

            //TODO Charisma Check
            if (proficiency >= success)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>You charm {target.Name} in offering you favourable prices.</p>",
                    player.ConnectionId
                );
                return 25;
            }

            Services.Instance.Writer.WriteLine(
                "<p>Your haggle attempts fail.</p>",
                player.ConnectionId
            );

            if (foundSkill.Proficiency == 100)
            {
                return 0;
            }

            var increase = DiceBag.Roll(1, 1, 5);

            foundSkill.Proficiency += increase;

            player.FailedSkill(SkillName.Haggle, out var message);

            Services.Instance.UpdateClient.UpdateExp(player);

            Services.Instance.Writer.WriteLine(message, player.ConnectionId);

            return 0;
        }

        public int DualWield(Player player, Player target, Room room, string obj)
        {
            if (string.IsNullOrEmpty(obj))
            {
                Services.Instance.Writer.WriteLine(
                    "Use what for a secondary weapon?",
                    player.ConnectionId
                );
                return 0;
            }

            var foundSkill = player.Skills.FirstOrDefault(x => x.Name == SkillName.DualWield);

            if (foundSkill == null)
            {
                Services.Instance.Writer.WriteLine(
                    "One weapon is more than enough for you to worry about.",
                    player.ConnectionId
                );
                return 0;
            }

            var findWeapon = player.Inventory.FirstOrDefault(
                x => x.Name.Contains(obj) && x.Equipped == false
            );

            if (findWeapon == null)
            {
                Services.Instance.Writer.WriteLine(
                    "You can't find that weapon.",
                    player.ConnectionId
                );
                return 0;
            }

            if (player.Equipped.Wielded == null)
            {
                Services.Instance.Writer.WriteLine(
                    "You need to wield a weapon first.",
                    player.ConnectionId
                );
                return 0;
            }

            //// exception for rangers
            //if (findWeapon.Weight <= player.Equipped.Wielded.Weight)
            //{
            //    _writer.WriteLine("Your offhand secondary weapon must be lighter than your primary weapon", player.ConnectionId);
            //    return 0;
            //}


            if (player.Equipped.Shield != null)
            {
                var shield = player.Equipped.Shield;

                _equip.Remove(shield.Name, room, player);
            }

            _equip.Wear(findWeapon.Name, room, player, "dual");

            // combat on success 2 hits, on success for strength damage if not half damage

            return 0;
        }

        public int Lore(Player player, Room room, string obj)
        {
            if (string.IsNullOrEmpty(obj))
            {
                Services.Instance.Writer.WriteLine("Lore What!?.", player.ConnectionId);

                return 0;
            }

            var nthTarget = Helpers.findNth(obj);
            var item =
                Helpers.findRoomObject(nthTarget, room)
                ?? Helpers.findObjectInInventory(nthTarget, player);
            // only lore items that can be picked up
            if (item.Stuck)
            {
                Services.Instance.Writer.WriteLine(
                    "There is nothing more to note about that object.",
                    player.ConnectionId
                );

                return 0;
            }

            var sb = new StringBuilder();

            sb.Append(
                $"It is a level {item.Level} {item.ItemType}, weight {item.Weight}.<br/>Locations it can be worn: {(item.ItemType == Item.Item.ItemTypes.Light || item.ItemType == Item.Item.ItemTypes.Weapon || item.ItemType == Item.Item.ItemTypes.Armour ? item.Slot : Character.Equipment.Equipment.EqSlot.Held)}.<br /> This {item.ItemType} has a gold value of {item.Value}.<br />"
            );

            if (item.ItemType == Item.Item.ItemTypes.Weapon)
            {
                sb.Append($"Damage {item.Damage.Minimum} - {item.Damage.Maximum}</br>");
                sb.Append($"Damage Type {item.DamageType}");
            }
            if (
                item.ItemType == Item.Item.ItemTypes.Armour
                && (item.ArmourRating.Armour != 0 || item.ArmourRating.Magic != 0)
            )
            {
                sb.Append(
                    $"Affects armour by {item.ArmourRating.Armour} / {item.ArmourRating.Magic}"
                );
            }
            if (item.ItemType == Item.Item.ItemTypes.Potion)
            {
                sb.Append($"Potion of {item.SpellName}.<br />");
                sb.Append($"Potion Strength: {item.SpellLevel}</br>");
            }

            if (item.Modifier.Strength != 0)
            {
                sb.Append($"<br />Affects strength by {item.Modifier.Strength}");
            }

            if (item.Modifier.Dexterity != 0)
            {
                sb.Append($"<br />Affects dexterity by {item.Modifier.Dexterity}");
            }

            if (item.Modifier.Constitution != 0)
            {
                sb.Append($"<br />Affects constitution by {item.Modifier.Constitution}");
            }

            if (item.Modifier.Wisdom != 0)
            {
                sb.Append($"<br />Affects wisdom by {item.Modifier.Wisdom}");
            }

            if (item.Modifier.Intelligence != 0)
            {
                sb.Append($"<br />Affects intelligence by {item.Modifier.Intelligence}");
            }

            if (item.Modifier.Charisma != 0)
            {
                sb.Append($"<br />Affects charisma by {item.Modifier.Charisma}");
            }

            if (item.Modifier.HP != 0)
            {
                sb.Append($"<br />Affects HP by {item.Modifier.HP}");
            }

            if (item.Modifier.Mana != 0)
            {
                sb.Append($"<br />Affects mana by {item.Modifier.Mana}");
            }

            if (item.Modifier.Moves != 0)
            {
                sb.Append($"<br />Affects moves by {item.Modifier.Moves}");
            }

            if (item.Modifier.DamRoll != 0)
            {
                sb.Append($"<br />Affects damroll by {item.Modifier.DamRoll}");
            }

            if (item.Modifier.HitRoll != 0)
            {
                sb.Append($"<br />Affects hitroll by {item.Modifier.HitRoll}");
            }

            if (item.Modifier.Saves != 0)
            {
                sb.Append($"<br />Affects saves by {item.Modifier.Saves}");
            }

            if (item.Modifier.SpellDam != 0)
            {
                sb.Append($"<br />Affects spell dam by {item.Modifier.SpellDam}");
            }

            if (
                item.ItemType == Item.Item.ItemTypes.Crafting
                || item.ItemType == Item.Item.ItemTypes.Forage
                || item.ItemType == Item.Item.ItemTypes.Food
            )
            {
                sb.Append($"<br />Condition: {item.Condition}");
            }

            Services.Instance.Writer.WriteLine(sb.ToString(), player.ConnectionId);

            return 0;
        }
    }
}
