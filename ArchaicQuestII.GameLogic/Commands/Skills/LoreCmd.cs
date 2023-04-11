using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Skills
{
    public class LoreCmd : SkillCore, ICommand
    {
        public LoreCmd()
            : base()
        {
            Aliases = new[] { "lore", "lor" };
            Description =
                "Lore is a useful skill for those that do not have the identify spell. Lore will give you the information and stats on the specified item";
            Usages = new[] { "Type: lore sword - to see the stats of that item" };
            DeniedStatus = null;
            Title = DefineSkill.Lore().Name;
            UserRole = UserRole.Player;
        }

        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var obj = input.ElementAtOrDefault(1)?.ToLower();

            var canDoSkill = CanPerformSkill(DefineSkill.Lore(), player);
            if (!canDoSkill)
            {
                return;
            }

            if (!player.RollSkill(SkillName.Lore, true))
            {
                return;
            }

            if (string.IsNullOrEmpty(obj))
            {
                Services.Instance.Writer.WriteLine("Lore What!?.", player.ConnectionId);
                return;
            }

            var item = FindItem(obj, room, player);
            if (item == null)
            {
                Services.Instance.Writer.WriteLine("You don't see that here.", player.ConnectionId);
                return;
            }

            // only lore items that can be picked up
            if (item.Stuck)
            {
                Services.Instance.Writer.WriteLine(
                    "There is nothing more to note about that object.",
                    player.ConnectionId
                );
                return;
            }

            var sb = new StringBuilder();

            sb.Append(
                $"It is a level {item.Level} {item.ItemType}, weight {item.Weight}.<br/>Locations it can be worn: {(item.ItemType == Item.Item.ItemTypes.Light || item.ItemType == Item.Item.ItemTypes.Weapon || item.ItemType == Item.Item.ItemTypes.Armour ? item.Slot : EquipmentSlot.Held)}.<br /> This {item.ItemType} has a gold value of {item.Value}.<br />"
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

            foreach (var pc in room.Players.Where(x => x.Id != player.Id))
            {
                Services.Instance.Writer.WriteLine(
                    $"{player.Name} twists and turns {item.Name.ToLower()} trying to figure out it's properties.",
                    pc.ConnectionId
                );
            }

            Services.Instance.Writer.WriteLine(sb.ToString(), player.ConnectionId);
        }
    }
}
