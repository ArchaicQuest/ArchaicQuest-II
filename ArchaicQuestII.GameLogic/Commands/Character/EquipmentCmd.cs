using System;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class EquipmentCmd : ICommand
    {
        public EquipmentCmd()
        {
            Aliases = new[] { "hold", "wear", "remove", "wield", "eq", "equipment" };
            Description =
                @"'{yellow}Wear{/}' is used to wear a piece of armour from your inventory, if you are already wearing a piece of armour 
in the same slot it will automatically remove it.

Examples:
wear vest
wear all - wear everything in your inventory until all EQ slots are filled

'{yellow}remove{/}' is used to remove a piece of armour or stop using a weapon. Removed items return to your inventory.

Examples:
remove vest
remove all - remove everything you are wearing

{yellow}wield{/}' is used to equip a weapon

Examples:
wield staff

'{yellow}eq{/}' or {yellow}equipment{/}' is used to display all your equipment and the equipment slots available

Examples:
eq
equipment

{yellow}hold{/}' is used to hold an item, this can be useful such as if blind, and you're holding a potion you will be able to quaff it, other items that can only be held may give benefits 

Examples:
hold potion
";
            Usages = new[] { "Type: wear vest, remove vest, wield dagger, eq, hold doll" };
            Title = "Equipment";
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Busy,
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Ghost,
                CharacterStatus.Status.Fleeing,
                CharacterStatus.Status.Incapacitated,
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Stunned
            };
            UserRole = UserRole.Player;
        }

        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        private IEquip Equip { get; set; }

        public void Execute(Player player, Room room, string[] input)
        {
            var command = input.ElementAtOrDefault(0);
            var target = input.ElementAtOrDefault(1);

            switch (command)
            {
                case "wear":
                case "hold":

                    if (
                        command == "wear"
                            && !CommandTargetCheck(target, player, "<p>Wear what?</p>")
                        || command == "hold"
                            && !CommandTargetCheck(target, player, "<p>Hold what?</p>")
                    )
                    {
                        return;
                    }

                    if (target == "all")
                    {
                        WearAll(room, player);
                        return;
                    }

                    Wear(target, room, player, command == "hold" ? "hold" : String.Empty);

                    break;
                case "remove":

                    if (!CommandTargetCheck(target, player, "<p>Remove what?</p>"))
                    {
                        return;
                    }

                    if (target == "all")
                    {
                        RemoveAll(room, player);
                        return;
                    }
                    Remove(target, room, player);
                    break;
                case "eq":
                case "equipment":
                    ShowEquipment(player);
                    break;
                case "wield":
                    if (!CommandTargetCheck(target, player, "<p>Wield what?</p>"))
                    {
                        return;
                    }

                    Wear(target, room, player, "wield");
                    break;
            }
        }

        private bool CommandTargetCheck(string target, Player player, string errorMessage = "What?")
        {
            if (!string.IsNullOrEmpty(target))
                return true;
            Services.Instance.Writer.WriteLine(errorMessage, player.ConnectionId);
            return false;
        }

        private string ShowEquipmentUI(Player player)
        {
            var displayEquipment = new StringBuilder();

            try
            {
                displayEquipment
                    .Append("<p>You are using:</p>")
                    .Append("<table>")
                    .Append(
                        "<tr><td style='width:175px;' class='cell-title'  title='Worn as light'>"
                    )
                    .Append("&lt;used as light&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Light?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td class='cell-title'  title='Worn on finger'>")
                    .Append(" &lt;worn on finger&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Finger?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Worn on finger'>")
                    .Append(" &lt;worn on finger&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Finger2?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Worn around neck'>")
                    .Append(" &lt;worn around neck&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Neck?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td class='cell-title'  title='Worn around neck'>")
                    .Append(" &lt;worn around neck&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Neck2?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Worn on face'>")
                    .Append(" &lt;worn on face&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Face?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Worn on head'>")
                    .Append(" &lt;worn on head&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Head?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Worn on torso'>")
                    .Append(" &lt;worn on torso&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Torso?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Worn on legs'>")
                    .Append(" &lt;worn on legs&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Legs?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Worn on feet'>")
                    .Append(" &lt;worn on feet&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Feet?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Worn on hands'>")
                    .Append(" &lt;worn on hands&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Hands?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Worn on arms'>")
                    .Append(" &lt;worn on arms&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Arms?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Worn about body'>")
                    .Append(" &lt;worn about body&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(
                        Helpers.DisplayEQNameWithFlags(player.Equipped.AboutBody) ?? "(nothing)"
                    )
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Worn on waist'>")
                    .Append(" &lt;worn about waist&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Waist?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Worn on wrist'>")
                    .Append(" &lt;worn around wrist&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Wrist?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Worn on wrist'>")
                    .Append(" &lt;worn around wrist&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Wrist2?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='worn as weapon'>")
                    .Append(" &lt;wielded&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Wielded?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='worn as weapon'>")
                    .Append(" &lt;secondary&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Secondary?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Worn as shield'>")
                    .Append(" &lt;worn as shield&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Shield?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Held'>")
                    .Append(" &lt;Held&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Held?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  class='cell-title'  title='Floating Nearby'>")
                    .Append(" &lt;Floating nearby&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(player.Equipped.Floating?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("</table");
            }
            catch (Exception ex)
            {
                Console.WriteLine("EquipmentCmd.cs: " + ex);
            }

            return displayEquipment.ToString();
        }

        private bool EqSlotOpen(Equipment.EqSlot slot, Player player)
        {
            switch (slot)
            {
                case Equipment.EqSlot.Arms:
                    return player.Equipped.Arms == null;
                case Equipment.EqSlot.Body:
                    return player.Equipped.AboutBody == null;
                case Equipment.EqSlot.Face:
                    return player.Equipped.Face == null;
                case Equipment.EqSlot.Feet:
                    return player.Equipped.Feet == null;
                case Equipment.EqSlot.Finger:
                    return player.Equipped.Finger == null || player.Equipped.Finger2 == null;
                case Equipment.EqSlot.Floating:
                    return player.Equipped.Floating == null;
                case Equipment.EqSlot.Hands:
                    return player.Equipped.Hands == null;
                case Equipment.EqSlot.Head:
                    return player.Equipped.Head == null;
                case Equipment.EqSlot.Held:
                    return player.Equipped.Held == null;
                case Equipment.EqSlot.Legs:
                    return player.Equipped.Legs == null;
                case Equipment.EqSlot.Light:
                    return player.Equipped.Light == null;
                case Equipment.EqSlot.Neck:
                    return player.Equipped.Neck == null || player.Equipped.Neck2 == null;
                case Equipment.EqSlot.Shield:
                    return player.Equipped.Shield == null;
                case Equipment.EqSlot.Torso:
                    return player.Equipped.Torso == null;
                case Equipment.EqSlot.Waist:
                    return player.Equipped.Waist == null;
                case Equipment.EqSlot.Wielded:
                    return player.Equipped.Wielded == null;
                case Equipment.EqSlot.Wrist:
                    return player.Equipped.Wrist == null || player.Equipped.Wrist2 == null;
                case Equipment.EqSlot.Secondary:
                    return player.Equipped.Secondary == null;
                default:
                    return false;
            }
        }

        private void WearAll(Room room, Player player)
        {
            var itemsToWear = player.Inventory.Where(x => x.Equipped == false);

            foreach (var itemToWear in itemsToWear)
            {
                if (EqSlotOpen(itemToWear.Slot, player))
                {
                    Wear(itemToWear.Name, room, player);
                }
            }
        }

        private void Remove(string item, Room room, Player player)
        {
            if (item.Equals("all", StringComparison.CurrentCultureIgnoreCase))
            {
                RemoveAll(room, player);
                return;
            }

            var itemToRemove = player.Inventory.FirstOrDefault(
                x => x.Name.Contains(item, StringComparison.CurrentCultureIgnoreCase) && x.Equipped
            );

            if (itemToRemove == null)
            {
                Services.Instance.Writer.WriteLine(
                    "<p>You are not wearing that item.</p>",
                    player.ConnectionId
                );
                return;
            }

            if ((itemToRemove.ItemFlag & Item.Item.ItemFlags.Noremove) != 0)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>You can't remove {itemToRemove.Name}. It appears to be cursed.</p>",
                    player.ConnectionId
                );
                return;
            }

            itemToRemove.Equipped = false;
            player.ArmorRating.Armour -=
                itemToRemove.ArmourRating.Armour + itemToRemove.Modifier.AcMod;
            player.ArmorRating.Magic -=
                itemToRemove.ArmourRating.Magic + itemToRemove.Modifier.AcMagicMod;
            player.Attributes.Attribute[EffectLocation.Strength] -= itemToRemove.Modifier.Strength;
            player.Attributes.Attribute[EffectLocation.Dexterity] -= itemToRemove
                .Modifier
                .Dexterity;
            player.Attributes.Attribute[EffectLocation.Constitution] -= itemToRemove
                .Modifier
                .Constitution;
            player.Attributes.Attribute[EffectLocation.Wisdom] -= itemToRemove.Modifier.Wisdom;
            player.Attributes.Attribute[EffectLocation.Intelligence] -= itemToRemove
                .Modifier
                .Intelligence;
            player.Attributes.Attribute[EffectLocation.Charisma] -= itemToRemove.Modifier.Charisma;

            player.Attributes.Attribute[EffectLocation.Hitpoints] -= itemToRemove.Modifier.HP;
            player.Attributes.Attribute[EffectLocation.Mana] -= itemToRemove.Modifier.Mana;
            player.Attributes.Attribute[EffectLocation.Moves] -= itemToRemove.Modifier.Moves;
            player.MaxAttributes.Attribute[EffectLocation.Hitpoints] -= itemToRemove.Modifier.HP;
            player.MaxAttributes.Attribute[EffectLocation.Mana] -= itemToRemove.Modifier.Mana;
            player.MaxAttributes.Attribute[EffectLocation.Moves] -= itemToRemove.Modifier.Moves;

            player.Attributes.Attribute[EffectLocation.DamageRoll] -= itemToRemove.Modifier.DamRoll;
            player.Attributes.Attribute[EffectLocation.HitRoll] -= itemToRemove.Modifier.HitRoll;

            switch (itemToRemove.Slot)
            {
                case Equipment.EqSlot.Arms:
                    player.Equipped.Arms = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Body:
                    player.Equipped.AboutBody = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Face:
                    player.Equipped.Face = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Feet:
                    player.Equipped.Feet = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Finger:
                    if (player.Equipped.Finger.Id == itemToRemove.Id)
                        player.Equipped.Finger = null;
                    else if (player.Equipped.Finger2.Id == itemToRemove.Id)
                        player.Equipped.Finger2 = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Floating:
                    player.Equipped.Floating = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Hands:
                    player.Equipped.Hands = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Head:
                    player.Equipped.Head = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Held:
                    player.Equipped.Held = null; // TODO: handle when wield and shield or 2hand item are equipped
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Legs:
                    player.Equipped.Legs = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Light:
                    player.Equipped.Light = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Neck:
                    if (player.Equipped.Neck.Id == itemToRemove.Id)
                        player.Equipped.Neck = null;
                    else if (player.Equipped.Neck2.Id == itemToRemove.Id)
                        player.Equipped.Neck2 = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Shield:
                    player.Equipped.Shield = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Torso:
                    player.Equipped.Torso = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Waist:
                    player.Equipped.Waist = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Wielded:
                    if (
                        player.Equipped.Secondary != null
                        && itemToRemove.Name.Equals(player.Equipped.Secondary.Name)
                    )
                    {
                        player.Equipped.Secondary = null;
                        Services.Instance.Writer.WriteLine(
                            $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                            player.ConnectionId
                        );
                        Services.Instance.Writer.WriteToOthersInRoom(
                            $"<p>{player.Name} stops using {itemToRemove.Name}",
                            room,
                            player
                        );
                    }
                    else
                    {
                        player.Equipped.Wielded = null;
                        Services.Instance.Writer.WriteLine(
                            $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                            player.ConnectionId
                        );
                        Services.Instance.Writer.WriteToOthersInRoom(
                            $"<p>{player.Name} stops using {itemToRemove.Name}",
                            room,
                            player
                        );
                    }
                    break;
                case Equipment.EqSlot.Wrist:
                    if (player.Equipped.Wrist.Id == itemToRemove.Id)
                        player.Equipped.Wrist = null;
                    else if (player.Equipped.Wrist2.Id == itemToRemove.Id)
                        player.Equipped.Wrist2 = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Secondary:
                    player.Equipped.Secondary = null;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} stops using {itemToRemove.Name}",
                        room,
                        player
                    );
                    break;
                default:
                    itemToRemove.Equipped = false;
                    Services.Instance.Writer.WriteLine(
                        "<p>You don't know how to remove this.</p>",
                        player.ConnectionId
                    );
                    break;
            }

            Services.Instance.UpdateClient.UpdateScore(player);
            Services.Instance.UpdateClient.UpdateEquipment(player);
            Services.Instance.UpdateClient.UpdateInventory(player);
            Services.Instance.UpdateClient.UpdateHP(player);
            Services.Instance.UpdateClient.UpdateMana(player);
            Services.Instance.UpdateClient.UpdateMoves(player);
        }

        private void RemoveAll(Room room, Player player)
        {
            var itemsToRemove = player.Inventory.Where(x => x.Equipped == true);

            foreach (var itemToRemove in itemsToRemove)
            {
                Remove(itemToRemove.Name, room, player);
            }
        }

        private void ShowEquipment(Player player)
        {
            var displayEquipment = ShowEquipmentUI(player);
            Services.Instance.Writer.WriteLine(displayEquipment, player.ConnectionId);
        }

        // handle secondary equip
        private void Wear(string item, Room room, Player player, string type = "")
        {
            if (item.Equals("all", StringComparison.CurrentCultureIgnoreCase))
            {
                WearAll(room, player);
                return;
            }

            var itemToWear = player.Inventory.FirstOrDefault(
                x =>
                    x.Name.Contains(item, StringComparison.CurrentCultureIgnoreCase)
                    && x.Equipped == false
            );

            if (itemToWear == null)
            {
                Services.Instance.Writer.WriteLine(
                    "<p>You don't have that item.</p>",
                    player.ConnectionId
                );
                return;
            }

            if (type == "wield")
            {
                if (itemToWear.ItemType != Item.Item.ItemTypes.Weapon)
                {
                    Services.Instance.Writer.WriteLine(
                        "<p>You can't wield that.</p>",
                        player.ConnectionId
                    );
                    return;
                }
            }

            if (type == "hold")
            {
                if (itemToWear.Slot != Equipment.EqSlot.Held)
                {
                    Services.Instance.Writer.WriteLine(
                        "<p>You can't hold that.</p>",
                        player.ConnectionId
                    );
                    return;
                }
            }

            var itemSlot = itemToWear.Slot;

            if (
                itemToWear.ItemType != Item.Item.ItemTypes.Armour
                && itemToWear.ItemType != Item.Item.ItemTypes.Weapon
                && itemToWear.ItemType != Item.Item.ItemTypes.Light
            )
            {
                itemSlot = Equipment.EqSlot.Held;
            }

            if (type == "dual")
            {
                itemSlot = Equipment.EqSlot.Secondary;
            }

            itemToWear.Equipped = true;
            player.ArmorRating.Armour += itemToWear.ArmourRating.Armour + itemToWear.Modifier.AcMod;
            player.ArmorRating.Magic +=
                itemToWear.ArmourRating.Magic + itemToWear.Modifier.AcMagicMod;
            player.Attributes.Attribute[EffectLocation.Strength] += itemToWear.Modifier.Strength;
            player.Attributes.Attribute[EffectLocation.Dexterity] += itemToWear.Modifier.Dexterity;
            player.Attributes.Attribute[EffectLocation.Constitution] += itemToWear
                .Modifier
                .Constitution;
            player.Attributes.Attribute[EffectLocation.Wisdom] += itemToWear.Modifier.Wisdom;
            player.Attributes.Attribute[EffectLocation.Intelligence] += itemToWear
                .Modifier
                .Intelligence;
            player.Attributes.Attribute[EffectLocation.Charisma] += itemToWear.Modifier.Charisma;

            player.Attributes.Attribute[EffectLocation.Hitpoints] += itemToWear.Modifier.HP;
            player.Attributes.Attribute[EffectLocation.Mana] += itemToWear.Modifier.Mana;
            player.Attributes.Attribute[EffectLocation.Moves] += itemToWear.Modifier.Moves;
            player.MaxAttributes.Attribute[EffectLocation.Hitpoints] += itemToWear.Modifier.HP;
            player.MaxAttributes.Attribute[EffectLocation.Mana] += itemToWear.Modifier.Mana;
            player.MaxAttributes.Attribute[EffectLocation.Moves] += itemToWear.Modifier.Moves;

            player.Attributes.Attribute[EffectLocation.DamageRoll] += itemToWear.Modifier.DamRoll;
            player.Attributes.Attribute[EffectLocation.HitRoll] += itemToWear.Modifier.HitRoll;
            // player.Attributes.Attribute[EffectLocation.DamageRoll] += itemToWear.Modifier.SpellDam; // spell dam no exist
            // player.Attributes.Attribute[EffectLocation.SavingSpell] += itemToWear.Modifier.Saves; not implemented
            switch (itemSlot)
            {
                case Equipment.EqSlot.Arms:
                    if (player.Equipped.Arms != null)
                    {
                        Remove(player.Equipped.Arms.Name, room, player);
                    }

                    player.Equipped.Arms = itemToWear;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You wear {itemToWear.Name.ToLower()} on your arms.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} on {player.ReturnPronoun()} arms.</p>",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Body:
                    if (player.Equipped.AboutBody != null)
                    {
                        Remove(player.Equipped.AboutBody.Name, room, player);
                    }
                    player.Equipped.AboutBody = itemToWear;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You wear {itemToWear.Name.ToLower()} about your body.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} about {player.ReturnPronoun()} body.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Face:

                    if (player.Equipped.Face != null)
                    {
                        Remove(player.Equipped.Face.Name, room, player);
                    }

                    player.Equipped.Face = itemToWear;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You wear {itemToWear.Name.ToLower()} on your face.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} on {player.ReturnPronoun()} face.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Feet:

                    if (player.Equipped.Feet != null)
                    {
                        Remove(player.Equipped.Feet.Name, room, player);
                    }

                    player.Equipped.Feet = itemToWear;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You wear {itemToWear.Name.ToLower()} on your feet.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} on {player.ReturnPronoun()} feet.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Finger:

                    if (player.Equipped.Finger == null)
                    {
                        player.Equipped.Finger = itemToWear;
                    }
                    else if (player.Equipped.Finger2 == null)
                    {
                        player.Equipped.Finger2 = itemToWear;
                    }
                    else
                    {
                        Remove(player.Equipped.Finger.Name, room, player);
                        player.Equipped.Finger = itemToWear;
                    }

                    Services.Instance.Writer.WriteLine(
                        $"<p>You wear {itemToWear.Name.ToLower()} on your finger.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} on {player.ReturnPronoun()} finger.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Floating:

                    if (player.Equipped.Floating != null)
                    {
                        Remove(player.Equipped.Floating.Name, room, player);
                    }

                    player.Equipped.Floating = itemToWear;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You release {itemToWear.Name.ToLower()} to float around you.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} to float around {player.ReturnPronoun()}.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Hands:

                    if (player.Equipped.Hands != null)
                    {
                        Remove(player.Equipped.Hands.Name, room, player);
                    }

                    player.Equipped.Hands = itemToWear;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You wear {itemToWear.Name.ToLower()} on your hands.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} on {player.ReturnPronoun()} hands.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Head:

                    if (player.Equipped.Head != null)
                    {
                        Remove(player.Equipped.Head.Name, room, player);
                    }

                    player.Equipped.Head = itemToWear;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You wear {itemToWear.Name.ToLower()} on your head.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} on {player.ReturnPronoun()} head.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Held:
                    if (player.Equipped.Held != null)
                    {
                        Remove(player.Equipped.Held.Name, room, player);
                    }

                    player.Equipped.Held = itemToWear; // TODO: handle when wield and shield or 2hand item are equipped
                    Services.Instance.Writer.WriteLine(
                        $"<p>You hold {itemToWear.Name.ToLower()} in your hands.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} in {player.ReturnPronoun()} hands.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Legs:

                    if (player.Equipped.Legs != null)
                    {
                        Remove(player.Equipped.Legs.Name, room, player);
                    }
                    player.Equipped.Legs = itemToWear;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You wear {itemToWear.Name.ToLower()} on your legs.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} on {player.ReturnPronoun()} legs.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Light:

                    if (player.Equipped.Light != null)
                    {
                        Remove(player.Equipped.Light.Name, room, player);
                    }

                    player.Equipped.Light = itemToWear;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You equip {itemToWear.Name.ToLower()} as your light.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} as {player.ReturnPronoun()} light.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Neck:

                    if (player.Equipped.Neck == null)
                    {
                        player.Equipped.Neck = itemToWear;
                    }
                    else if (player.Equipped.Neck2 == null)
                    {
                        player.Equipped.Neck2 = itemToWear;
                    }
                    else
                    {
                        Remove(player.Equipped.Neck.Name, room, player);
                        player.Equipped.Neck = itemToWear;
                    }

                    Services.Instance.Writer.WriteLine(
                        $"<p>You wear {itemToWear.Name.ToLower()} around your neck.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} around {player.ReturnPronoun()} neck.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Shield:

                    if (player.Equipped.Wielded != null && player.Equipped.Wielded.TwoHanded)
                    {
                        Services.Instance.Writer.WriteLine(
                            "Your hands are tied up with your two-handed weapon!",
                            player.ConnectionId
                        );
                        return;
                    }

                    if (player.Equipped.Shield != null)
                    {
                        Remove(player.Equipped.Shield.Name, room, player);
                    }

                    if (player.Equipped.Secondary != null)
                    {
                        Remove(player.Equipped.Secondary.Name, room, player);
                    }

                    player.Equipped.Shield = itemToWear;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You equip {itemToWear.Name.ToLower()} as your shield.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} as {player.ReturnPronoun()} shield.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Torso:

                    if (player.Equipped.Torso != null)
                    {
                        Remove(player.Equipped.Torso.Name, room, player);
                    }

                    player.Equipped.Torso = itemToWear;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You wear {itemToWear.Name.ToLower()} around your torso.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} around {player.ReturnPronoun()} torso.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Waist:

                    if (player.Equipped.Waist != null)
                    {
                        Remove(player.Equipped.Waist.Name, room, player);
                    }

                    player.Equipped.Waist = itemToWear;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You wear {itemToWear.Name.ToLower()} around your waist.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} around {player.ReturnPronoun()} waist.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Wielded:

                    if (itemToWear.TwoHanded && player.Equipped.Shield != null)
                    {
                        Services.Instance.Writer.WriteLine(
                            "You need two hands free for that weapon, remove your shield and try again.",
                            player.ConnectionId
                        );

                        return;
                    }

                    if (player.Equipped.Wielded != null)
                    {
                        Remove(player.Equipped.Wielded.Name, room, player);
                    }

                    player.Equipped.Wielded = itemToWear;
                    Services.Instance.Writer.WriteLine(
                        $"<p>You wield {itemToWear.Name.ToLower()}.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()}.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Wrist:

                    if (player.Equipped.Wrist == null)
                    {
                        player.Equipped.Wrist = itemToWear;
                    }
                    else if (player.Equipped.Wrist2 == null)
                    {
                        player.Equipped.Wrist2 = itemToWear;
                    }
                    else
                    {
                        Remove(player.Equipped.Wrist.Name, room, player);
                        player.Equipped.Wrist = itemToWear;
                    }

                    Services.Instance.Writer.WriteLine(
                        $"<p>You wear {itemToWear.Name.ToLower()} on your wrist.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} on {player.ReturnPronoun()} wrist.",
                        room,
                        player
                    );
                    break;
                case Equipment.EqSlot.Secondary:

                    if (player.Equipped.Secondary != null)
                    {
                        Remove(player.Equipped.Secondary.Name, room, player);
                    }

                    player.Equipped.Secondary = itemToWear; // TODO: slot 2
                    Services.Instance.Writer.WriteLine(
                        $"<p>You wield {itemToWear.Name.ToLower()} as your second weapon.</p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} equips {itemToWear.Name.ToLower()} as {player.ReturnPronoun()} secondary weapon.",
                        room,
                        player
                    );
                    break;
                default:
                    itemToWear.Equipped = false;
                    Services.Instance.Writer.WriteLine(
                        "<p>You don't know how to wear this.</p>",
                        player.ConnectionId
                    );
                    break;
            }

            Services.Instance.UpdateClient.UpdateEquipment(player);
            Services.Instance.UpdateClient.UpdateScore(player);
            Services.Instance.UpdateClient.UpdateHP(player);
            Services.Instance.UpdateClient.UpdateMana(player);
            Services.Instance.UpdateClient.UpdateMoves(player);
            Services.Instance.UpdateClient.UpdateInventory(player);
        }
    }
}
