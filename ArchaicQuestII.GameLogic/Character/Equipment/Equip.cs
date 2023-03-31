using System;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Character.Equipment
{
    public class Equip : IEquip
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _clientUi;

        public Equip(IWriteToClient writer, IUpdateClientUI clientUi)
        {
            _writer = writer;
            _clientUi = clientUi;
        }


        public string ShowEquipmentUI(Player player)
        {
            var displayEquipment = new StringBuilder();

            try
            {
                displayEquipment.Append("<p>You are using:</p>")
                    .Append("<table>")
                    .Append("<tr><td style='width:175px;' title='Worn as light'>").Append("&lt;used as light&gt;")
                    .Append("</td>").Append("<td>").Append(player.Equipped.Light?.Name ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td title='Worn on finger'>").Append(" &lt;worn on finger&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Finger?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Worn on finger'>").Append(" &lt;worn on finger&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Finger2?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Worn around neck'>").Append(" &lt;worn around neck&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Neck?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td title='Worn around neck'>").Append(" &lt;worn around neck&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Neck2?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Worn on face'>").Append(" &lt;worn on face&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Face?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Worn on head'>").Append(" &lt;worn on head&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Head?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Worn on torso'>").Append(" &lt;worn on torso&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Torso?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Worn on legs'>").Append(" &lt;worn on legs&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Legs?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Worn on feet'>").Append(" &lt;worn on feet&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Feet?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Worn on hands'>").Append(" &lt;worn on hands&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Hands?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Worn on arms'>").Append(" &lt;worn on arms&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Arms?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Worn about body'>").Append(" &lt;worn about body&gt;").Append("</td>")
                    .Append("<td>").Append(Helpers.DisplayEQNameWithFlags(player.Equipped.AboutBody) ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("<tr><td  title='Worn on waist'>").Append(" &lt;worn about waist&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Waist?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Worn on wrist'>").Append(" &lt;worn around wrist&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Wrist?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Worn on wrist'>").Append(" &lt;worn around wrist&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Wrist2?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='worn as weapon'>").Append(" &lt;wielded&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Wielded?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='worn as weapon'>").Append(" &lt;secondary&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Secondary?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Worn as shield'>").Append(" &lt;worn as shield&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Shield?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Held'>").Append(" &lt;Held&gt;").Append("</td>").Append("<td>")
                    .Append(player.Equipped.Held?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("<tr><td  title='Floating Nearby'>").Append(" &lt;Floating nearby&gt;").Append("</td>")
                    .Append("<td>").Append(player.Equipped.Floating?.Name ?? "(nothing)").Append("</td></tr>")
                    .Append("</table");
            }
            catch (Exception ex)
            {
                
            }

            return displayEquipment.ToString();
        }

        private string DisplayEQNameWithFlags(Item.Item item)
        {
            var flags = new StringBuilder();
            if (item.ItemFlag == Item.Item.ItemFlags.Glow)
            {
                flags.Append("(glowing)");
            }

            flags.Append(" " + item.Name);

            return flags.ToString();
        }
        private void EmitRemoveActionToRoom(Item.Item item, Room room, Player player)
        {
            foreach (var pc in room.Players)
            {
                if (pc.ConnectionId == player.ConnectionId)
                {
                    continue;
                }

                _writer.WriteLine($"<p>{pc.Name} stops using {item.Name.ToLower()}.</p>", pc.ConnectionId);
            }
        }

        private void EmitWearActionToRoom(string action, Room room, Player player)
        {
            foreach (var pc in room.Players)
            {
                if (pc.ConnectionId == player.ConnectionId)
                {
                    continue;
                }

                _writer.WriteLine($"<p>{player.Name} equips {action}</p>", pc.ConnectionId);
            }
        }

        public bool EqSlotSet(Equipment.EqSlot slot, Player player)
        {
            switch (slot)
            {
                case Equipment.EqSlot.Arms:
                    return player.Equipped.Arms != null;

                case Equipment.EqSlot.Body:
                    return player.Equipped.AboutBody != null;
                case Equipment.EqSlot.Face:
                    return player.Equipped.Face != null;
                case Equipment.EqSlot.Feet:
                    return player.Equipped.Feet != null;
                case Equipment.EqSlot.Finger:
                    return player.Equipped.Finger != null;
                case Equipment.EqSlot.Floating:
                    return player.Equipped.Floating != null;
                case Equipment.EqSlot.Hands:
                    return player.Equipped.Hands != null;
                case Equipment.EqSlot.Head:
                    return player.Equipped.Head != null;
                case Equipment.EqSlot.Held:
                    return player.Equipped.Held != null;
                case Equipment.EqSlot.Legs:
                    return player.Equipped.Legs != null;
                case Equipment.EqSlot.Light:
                    return player.Equipped.Light != null;
                case Equipment.EqSlot.Neck:
                    return player.Equipped.Neck != null;
                case Equipment.EqSlot.Shield:
                    return player.Equipped.Shield != null;
                case Equipment.EqSlot.Torso:
                    return player.Equipped.Torso != null;
                case Equipment.EqSlot.Waist:
                    return player.Equipped.Waist != null;
                case Equipment.EqSlot.Wielded:
                    return player.Equipped.Wielded != null;
                case Equipment.EqSlot.Wrist:
                    return player.Equipped.Wrist != null;
                case Equipment.EqSlot.Secondary:
                    return player.Equipped.Secondary != null;
                default:
                    return false;
            }

        }


        public void WearAll(Room room, Player player)
        {
            var itemsToWear = player.Inventory.Where(x => x.Equipped == false);

            foreach (var itemToWear in itemsToWear)
            {
                if (!EqSlotSet(itemToWear.Slot, player))
                {
                    Wear(itemToWear.Name, room, player);

                }
            }
        }

        public void Remove(string item, Room room, Player player)
        {
            if (item.Equals("all", StringComparison.CurrentCultureIgnoreCase))
            {
                RemoveAll(room, player);
                return;
            }

            var itemToRemove = player.Inventory.FirstOrDefault(x => x.Name.Contains(item, StringComparison.CurrentCultureIgnoreCase) && x.Equipped);

            if (itemToRemove == null)
            {
                _writer.WriteLine("<p>You are not wearing that item.</p>", player.ConnectionId);
                return;
            }
            
            if  ((itemToRemove.ItemFlag & Item.Item.ItemFlags.Noremove) != 0)
            {
                _writer.WriteLine($"<p>You can't remove {itemToRemove.Name}. It appears to be cursed.</p>", player.ConnectionId);
                return;
            }
            
            itemToRemove.Equipped = false;
            player.ArmorRating.Armour -= itemToRemove.ArmourRating.Armour + itemToRemove.Modifier.AcMod;
            player.ArmorRating.Magic -= itemToRemove.ArmourRating.Magic + itemToRemove.Modifier.AcMagicMod;
            player.Attributes.Attribute[EffectLocation.Strength] -= itemToRemove.Modifier.Strength;
            player.Attributes.Attribute[EffectLocation.Dexterity] -= itemToRemove.Modifier.Dexterity;
            player.Attributes.Attribute[EffectLocation.Constitution] -= itemToRemove.Modifier.Constitution;
            player.Attributes.Attribute[EffectLocation.Wisdom] -= itemToRemove.Modifier.Wisdom;
            player.Attributes.Attribute[EffectLocation.Intelligence] -= itemToRemove.Modifier.Intelligence;
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
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Body:
                    player.Equipped.AboutBody = null;
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Face:
                    player.Equipped.Face = null;
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Feet:
                    player.Equipped.Feet = null;
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Finger:
                    player.Equipped.Finger = null; // TODO: slot 2
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Floating:
                    player.Equipped.Floating = null;
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Hands:
                    player.Equipped.Hands = null;
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Head:
                    player.Equipped.Head = null;
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Held:
                    player.Equipped.Held = null; // TODO: handle when wield and shield or 2hand item are equipped
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Legs:
                    player.Equipped.Legs = null;
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Light:
                    player.Equipped.Light = null;
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Neck:
                    player.Equipped.Neck = null; // TODO: slot 2
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Shield:
                    player.Equipped.Shield = null;
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Torso:
                    player.Equipped.Torso = null;
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Waist:
                    player.Equipped.Waist = null;
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Wielded:

                    if (player.Equipped.Secondary != null && itemToRemove.Name.Equals(player.Equipped.Secondary.Name))
                    {
                        player.Equipped.Secondary = null;
                        _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                        EmitRemoveActionToRoom(itemToRemove, room, player);
                    }
                    else
                    {
                        player.Equipped.Wielded = null;
                        _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                        EmitRemoveActionToRoom(itemToRemove, room, player);
                    }

                    break;
                case Equipment.EqSlot.Wrist:
                    player.Equipped.Wrist = null; // TODO: slot 2
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                case Equipment.EqSlot.Secondary:
                    player.Equipped.Secondary = null;
                    _writer.WriteLine($"<p>You stop using {itemToRemove.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitRemoveActionToRoom(itemToRemove, room, player);
                    break;
                default:
                    itemToRemove.Equipped = false;
                    _writer.WriteLine("<p>You don't know how to remove this.</p>", player.ConnectionId);
                    break;
            }
            _clientUi.UpdateScore(player);
            _clientUi.UpdateEquipment(player);
            _clientUi.UpdateInventory(player);
            _clientUi.UpdateHP(player);
            _clientUi.UpdateMana(player);
            _clientUi.UpdateMoves(player);
        }

        public void RemoveAll(Room room, Player player)
        {
            var itemsToRemove = player.Inventory.Where(x => x.Equipped == true);

            foreach (var itemToRemove in itemsToRemove)
            {
                Remove(itemToRemove.Name, room, player);
            }
        }

        public void ShowEquipment(Player player)
        {

            var displayEquipment = ShowEquipmentUI(player);
            _writer.WriteLine(displayEquipment, player.ConnectionId);
        }

        // handle secondary equip
        public void Wear(string item, Room room, Player player, string type = "")
        {

            if (item.Equals("all", StringComparison.CurrentCultureIgnoreCase))
            {
                WearAll(room, player);
                return;
            }

            var itemToWear = player.Inventory.FirstOrDefault(x => x.Name.Contains(item, StringComparison.CurrentCultureIgnoreCase) && x.Equipped == false);

            if (itemToWear == null)
            {
                _writer.WriteLine("<p>You don't have that item.</p>", player.ConnectionId);
                return;
            }

            var itemSlot = itemToWear.Slot;

            if (itemToWear.ItemType != Item.Item.ItemTypes.Armour && itemToWear.ItemType != Item.Item.ItemTypes.Weapon && itemToWear.ItemType != Item.Item.ItemTypes.Light)
            {
                itemSlot = Equipment.EqSlot.Held;
            }

            if (type == "dual")
            {
                itemSlot = Equipment.EqSlot.Secondary;
            }

            itemToWear.Equipped = true;
            player.ArmorRating.Armour += itemToWear.ArmourRating.Armour + itemToWear.Modifier.AcMod;
            player.ArmorRating.Magic += itemToWear.ArmourRating.Magic + itemToWear.Modifier.AcMagicMod;
            player.Attributes.Attribute[EffectLocation.Strength] += itemToWear.Modifier.Strength;
            player.Attributes.Attribute[EffectLocation.Dexterity] += itemToWear.Modifier.Dexterity;
            player.Attributes.Attribute[EffectLocation.Constitution] += itemToWear.Modifier.Constitution;
            player.Attributes.Attribute[EffectLocation.Wisdom] += itemToWear.Modifier.Wisdom;
            player.Attributes.Attribute[EffectLocation.Intelligence] += itemToWear.Modifier.Intelligence;
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
                    _writer.WriteLine($"<p>You wear {itemToWear.Name.ToLower()} on your arms.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} on {Helpers.GetPronoun(player.Gender)} arms.", room, player);
                    break;
                case Equipment.EqSlot.Body:
                    if (player.Equipped.AboutBody != null)
                    {
                        Remove(player.Equipped.AboutBody.Name, room, player);
                    }
                    player.Equipped.AboutBody = itemToWear;
                    _writer.WriteLine($"<p>You wear {itemToWear.Name.ToLower()} about your body.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} about {Helpers.GetPronoun(player.Gender)} body.", room, player);
                    break;
                case Equipment.EqSlot.Face:

                    if (player.Equipped.Face != null)
                    {
                        Remove(player.Equipped.Face.Name, room, player);
                    }

                    player.Equipped.Face = itemToWear;
                    _writer.WriteLine($"<p>You wear {itemToWear.Name.ToLower()} on your face.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} on {Helpers.GetPronoun(player.Gender)} face.", room, player);
                    break;
                case Equipment.EqSlot.Feet:

                    if (player.Equipped.Feet != null)
                    {
                        Remove(player.Equipped.Feet.Name, room, player);
                    }

                    player.Equipped.Feet = itemToWear;
                    _writer.WriteLine($"<p>You wear {itemToWear.Name.ToLower()} on your feet.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} on {Helpers.GetPronoun(player.Gender)} feet.", room, player);
                    break;
                case Equipment.EqSlot.Finger:

                    if (player.Equipped.Finger != null)
                    {
                        Remove(player.Equipped.Finger.Name, room, player);
                    }

                    player.Equipped.Finger = itemToWear; // TODO: slot 2
                    _writer.WriteLine($"<p>You wear {itemToWear.Name.ToLower()} on your finger.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} on {Helpers.GetPronoun(player.Gender)} finger.", room, player);
                    break;
                case Equipment.EqSlot.Floating:

                    if (player.Equipped.Floating != null)
                    {
                        Remove(player.Equipped.Floating.Name, room, player);
                    }

                    player.Equipped.Floating = itemToWear;
                    _writer.WriteLine($"<p>You release {itemToWear.Name.ToLower()} to float around you.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} to float around {Helpers.GetPronoun(player.Gender)}.", room, player);
                    break;
                case Equipment.EqSlot.Hands:

                    if (player.Equipped.Hands != null)
                    {
                        Remove(player.Equipped.Hands.Name, room, player);
                    }

                    player.Equipped.Hands = itemToWear;
                    _writer.WriteLine($"<p>You wear {itemToWear.Name.ToLower()} on your hands.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} on {Helpers.GetPronoun(player.Gender)} hands.", room, player);
                    break;
                case Equipment.EqSlot.Head:

                    if (player.Equipped.Head != null)
                    {
                        Remove(player.Equipped.Head.Name, room, player);
                    }

                    player.Equipped.Head = itemToWear;
                    _writer.WriteLine($"<p>You wear {itemToWear.Name.ToLower()} on your head.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} on {Helpers.GetPronoun(player.Gender)} head.", room, player);
                    break;
                case Equipment.EqSlot.Held:
                    if (player.Equipped.Held != null)
                    {
                        Remove(player.Equipped.Held.Name, room, player);
                    }

                    player.Equipped.Held = itemToWear; // TODO: handle when wield and shield or 2hand item are equipped
                    _writer.WriteLine($"<p>You hold {itemToWear.Name.ToLower()} in your hands.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} in {Helpers.GetPronoun(player.Gender)} hands.", room, player);
                    break;
                case Equipment.EqSlot.Legs:

                    if (player.Equipped.Legs != null)
                    {
                        Remove(player.Equipped.Legs.Name, room, player);
                    }
                    player.Equipped.Legs = itemToWear;
                    _writer.WriteLine($"<p>You wear {itemToWear.Name.ToLower()} on your legs.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} on {Helpers.GetPronoun(player.Gender)} legs.", room, player);
                    break;
                case Equipment.EqSlot.Light:

                    if (player.Equipped.Light != null)
                    {
                        Remove(player.Equipped.Light.Name, room, player);
                    }

                    player.Equipped.Light = itemToWear;
                    _writer.WriteLine($"<p>You equip {itemToWear.Name.ToLower()} as your light.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} as {Helpers.GetPronoun(player.Gender)} light.", room, player);
                    break;
                case Equipment.EqSlot.Neck:

                    if (player.Equipped.Neck != null)
                    {
                        Remove(player.Equipped.Neck.Name, room, player);
                    }

                    player.Equipped.Neck = itemToWear; // TODO: slot 2
                    _writer.WriteLine($"<p>You wear {itemToWear.Name.ToLower()} around your neck.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} around {Helpers.GetPronoun(player.Gender)} neck.", room, player);
                    break;
                case Equipment.EqSlot.Shield:

                    if (player.Equipped.Wielded != null && player.Equipped.Wielded.TwoHanded)
                    {
                        _writer.WriteLine("Your hands are tied up with your two-handed weapon!", player.ConnectionId);
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
                    _writer.WriteLine($"<p>You equip {itemToWear.Name.ToLower()} as your shield.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} as {Helpers.GetPronoun(player.Gender)} shield.", room, player);
                    break;
                case Equipment.EqSlot.Torso:

                    if (player.Equipped.Torso != null)
                    {
                        Remove(player.Equipped.Torso.Name, room, player);
                    }

                    player.Equipped.Torso = itemToWear;
                    _writer.WriteLine($"<p>You wear {itemToWear.Name.ToLower()} around your torso.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} around {Helpers.GetPronoun(player.Gender)} torso.", room, player);
                    break;
                case Equipment.EqSlot.Waist:

                    if (player.Equipped.Waist != null)
                    {
                        Remove(player.Equipped.Waist.Name, room, player);
                    }

                    player.Equipped.Waist = itemToWear;
                    _writer.WriteLine($"<p>You wear {itemToWear.Name.ToLower()} around your waist.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} around {Helpers.GetPronoun(player.Gender)} waist.", room, player);
                    break;
                case Equipment.EqSlot.Wielded:

                    if (itemToWear.TwoHanded && player.Equipped.Shield != null)
                    {
                        _writer.WriteLine("You need two hands free for that weapon, remove your shield and try again.", player.ConnectionId);

                        return;
                    }

                    if (player.Equipped.Wielded != null)
                    {
                        Remove(player.Equipped.Wielded.Name, room, player);
                    }

                    player.Equipped.Wielded = itemToWear;
                    _writer.WriteLine($"<p>You wield {itemToWear.Name.ToLower()}.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()}.", room, player);
                    break;
                case Equipment.EqSlot.Wrist:

                    if (player.Equipped.Wrist != null)
                    {
                        Remove(player.Equipped.Wrist.Name, room, player);
                    }

                    player.Equipped.Wrist = itemToWear; // TODO: slot 2
                    _writer.WriteLine($"<p>You wear {itemToWear.Name.ToLower()} on your wrist.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} on {Helpers.GetPronoun(player.Gender)} wrist.", room, player);
                    break;
                case Equipment.EqSlot.Secondary:

                    if (player.Equipped.Secondary != null)
                    {
                        Remove(player.Equipped.Secondary.Name, room, player);
                    }

                    player.Equipped.Secondary = itemToWear; // TODO: slot 2
                    _writer.WriteLine($"<p>You wield {itemToWear.Name.ToLower()} as your second weapon.</p>", player.ConnectionId);
                    EmitWearActionToRoom($"{itemToWear.Name.ToLower()} as {Helpers.GetPronoun(player.Gender)} secondary weapon.", room, player);
                    break;
                default:
                    itemToWear.Equipped = false;
                    _writer.WriteLine("<p>You don't know how to wear this.</p>", player.ConnectionId);
                    break;
            }

            _clientUi.UpdateEquipment(player);
            _clientUi.UpdateScore(player);
            _clientUi.UpdateHP(player);
            _clientUi.UpdateMana(player);
            _clientUi.UpdateMoves(player);
            _clientUi.UpdateInventory(player);
        }

    }
}
