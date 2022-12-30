﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Skill.Skills;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Character.MobFunctions.Shop
{
    public class Shop : IMobFunctions
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _clientUi;
        private readonly IPassiveSkills _passiveSkills;

        public Shop(IWriteToClient writer, IUpdateClientUI clientUi, IPassiveSkills passiveSkills)
        {
            _writer = writer;
            _clientUi = clientUi;
            _passiveSkills = passiveSkills;
        }

        //       The cleric of Siccara says 'I offer the following spells:'
        // light: cure light wounds      10 gold
        // serious: cure serious wounds  15 gold
        // critic: cure critical wounds  25 gold
        // heal: healing spell           50 gold
        // blind: cure blindness         20 gold
        // disease: cure disease         15 gold
        // poison:  cure poison          25 gold
        // uncurse: remove curse         50 gold
        // refresh: restore movement      5 gold
        // mana:  restore mana           10 gold
        // cancel : cancellation spell   40 gold
        //Type heal<type> to be healed.


        public void DisplayInventory(Player mob, Player player)
        {

            var hagglePriceReduction = Haggle(player, mob);

            _writer.WriteLine(mob.Name + " says 'Here's what I have for sale.'", player.ConnectionId);
            var sb = new StringBuilder();
            sb.Append("<table class='data'><tr><td style='width: 30px; text-align: center;'>#</td><td style='width: 30px; text-align: center;'>Level</td><td  style='width: 65px;'>Price</td><td>Item</td></tr>");

            int i = 0;
            foreach (var item in mob.Inventory.Distinct().OrderBy(x => x.Level).ThenBy(x => x.Value))
            {
                i++;
                sb.Append($"<tr><td style='width: 30px; text-align: center;'>{i}</td><td style='width: 30px; text-align: center;'>{item.Level}</td><td  style='width: 65px;'>{DisplayUnit(item.Value, hagglePriceReduction)}</td><td>{item.Name}</td></tr>");
            }

            sb.Append("</table>");
            _writer.WriteLine(sb.ToString(), player.ConnectionId);

        }

        public Player FindShopKeeper(Room room)
        {
            foreach (var mob in room.Mobs)
            {
                if (mob.Shopkeeper)
                {
                    return mob;
                }
            }

            return null;
        }

        public void List(Room room, Player player)
        {
            var shopKeeper = FindShopKeeper(room);
            if (shopKeeper == null)
            {
                _writer.WriteLine("<p>There is no one selling here.</p>", player.ConnectionId);
                return;
            }

            if (!shopKeeper.Inventory.Any())
            {
                if (shopKeeper.SpellList.Any())
                {
                    _writer.WriteLine("<p>They have nothing for sale but do offer spells. Try 'heal'.</p>", player.ConnectionId);
                    return;
                }

                _writer.WriteLine("<p>They have nothing for sale.</p>", player.ConnectionId);
                return;
            }

            DisplayInventory(shopKeeper, player);
        }

        public int Haggle(Player player, Player target)
        {
            var priceReduction = _passiveSkills.Haggle(player, target);

            return priceReduction;

        }

        public double AddMarkUp(int price)
        {
            return price * 1.5;
        }



        /// <summary>
        /// Each gold piece is worth 100 silver pieces.
        /// 100 silver = 1 gold
        /// 1000 silver = 10 gold
        /// 10,000 silver = 100 gold
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public string DisplayUnit(int price, int hagglePriceReduction)
        {
            var goldPrice = AddMarkUp(price); // /100

            // show haggle price
            return $"{Math.Floor((decimal)goldPrice - Helpers.GetPercentage(hagglePriceReduction, (int)goldPrice))}";
            // return goldPrice < 1 ? $"{Math.Floor(goldPrice * 100)} SP" : $"{Math.Floor(goldPrice)} GP";
        }

        public void InspectItem(int itemNumber, Room room, Player player)
        {

            itemNumber -= 1;
            if (itemNumber < 0)
            {
                itemNumber = 0;
            }

            var vendor = room.Mobs.FirstOrDefault(x => x.Shopkeeper.Equals(true));

            if (vendor == null)
            {
                _writer.WriteLine("<p>You can't do that here.</p>", player.ConnectionId);
                return;
            }

            var hasItem = vendor.Inventory.Distinct().OrderBy(x => x.Level).ThenBy(x => x.Value).ToArray()[itemNumber];
            if (hasItem == null)
            {
                _writer.WriteLine($"<p>{vendor.Name} says 'I don't have anything like that to show you.'</p>", player.ConnectionId);
                return;
            }

            var sb = new StringBuilder();
            sb.Append($"<p>{vendor.Name} explains to you the properties of {hasItem.Name.ToLower()}:</p>");
            sb.Append(
                $"<p>'{hasItem.Name}' is type {hasItem.ItemType}<br />weight is {hasItem.Weight}, value is {hasItem.Value}, level is {hasItem.Level}.<br/>");

            string flags = "Extra flags: ";
            foreach (Enum value in Enum.GetValues(hasItem.ItemFlag.GetType()))
            {
                if (hasItem.ItemFlag.HasFlag(value))
                {

                    flags += value + ", ";

                }
            }

            if (hasItem.ItemType == Item.Item.ItemTypes.Armour)
            {
                sb.Append($"Armour Type: {hasItem.ArmourType}, Defense {hasItem.ArmourRating.Armour} and {hasItem.ArmourRating.Magic} vs magic.<br />");
            }

            if (hasItem.ItemType == Item.Item.ItemTypes.Weapon)
            {
                sb.Append($"Weapon Type: {hasItem.WeaponType}, Damage is {hasItem.Damage.Minimum}-{hasItem.Damage.Maximum} (average {hasItem.Damage.Minimum + hasItem.Damage.Maximum / 2}).<br />");
                sb.Append($"Damage type: {hasItem.DamageType}</br>");
            }

            sb.Append($"{flags}<br />");
            sb.Append("</p>");
            _writer.WriteLine(sb.ToString(), player.ConnectionId);
        }

        public void InspectItem(string itemName, Room room, Player player)
        {
            if (int.TryParse(itemName, out var n))
            {
                InspectItem(n, room, player);
                return;
            }

            var vendor = room.Mobs.FirstOrDefault(x => x.Shopkeeper.Equals(true));

            if (vendor == null)
            {
                _writer.WriteLine("<p>You can't do that here.</p>", player.ConnectionId);
                return;
            }

            var hasItem = vendor.Inventory.FirstOrDefault(x =>
                x.Name.Contains(itemName, StringComparison.InvariantCultureIgnoreCase));

            if (hasItem == null)
            {
                _writer.WriteLine($"<p>{vendor.Name} says 'I don't have anything like that to show you.'</p>", player.ConnectionId);
                return;
            }

            var sb = new StringBuilder();
            sb.Append($"<p>{vendor.Name} explains to you the properties of {hasItem.Name.ToLower()}:</p>");
            sb.Append(
                $"<p>'{hasItem.Name}' is type {hasItem.ItemType}<br />weight is {hasItem.Weight}, value is {hasItem.Value}, level is {hasItem.Level}.<br/>");

            string flags = "Extra flags: ";
            foreach (Enum value in Enum.GetValues(hasItem.ItemFlag.GetType()))
            {
                if (hasItem.ItemFlag.HasFlag(value))
                {

                    flags += value + ", ";

                }
            }

            if (hasItem.ItemType == Item.Item.ItemTypes.Armour)
            {
                sb.Append($"Armour Type: {hasItem.ArmourType}, Defense {hasItem.ArmourRating.Armour} and {hasItem.ArmourRating.Magic} vs magic.<br />");
            }

            if (hasItem.ItemType == Item.Item.ItemTypes.Weapon)
            {
                sb.Append($"Weapon Type: {hasItem.WeaponType}, Damage is {hasItem.Damage.Minimum}-{hasItem.Damage.Maximum} (average {hasItem.Damage.Minimum + hasItem.Damage.Maximum / 2}).<br />");
                sb.Append($"Damage type: {hasItem.DamageType}</br>");
            }

            sb.Append($"{flags}<br />");
            sb.Append("</p>");
            _writer.WriteLine(sb.ToString(), player.ConnectionId);
        }

        public void BuyItem(int itemNumber, Room room, Player player)
        {

            itemNumber -= 1;
            if (itemNumber < 0)
            {
                itemNumber = 0;
            }

            var vendor = room.Mobs.FirstOrDefault(x => x.Shopkeeper.Equals(true));

            if (vendor == null)
            {
                _writer.WriteLine("<p>You can't do that here.</p>", player.ConnectionId);
                return;
            }

            var hasItem = vendor.Inventory.Distinct().OrderBy(x => x.Level).ThenBy(x => x.Value).ToArray()[itemNumber];

            if (hasItem == null)
            {
                _writer.WriteLine($"<p>{vendor.Name} says 'I don't sell that, please view my \'list\' of items for sale.'</p>", player.ConnectionId);
                return;
            }

            var haggleReduction = _passiveSkills.Haggle(player, vendor);
            var goldValue = AddMarkUp(hasItem.Value);
            var trueGoldValue = goldValue - Helpers.GetPercentage(haggleReduction, (int)goldValue);
            if (player.Money.Gold < trueGoldValue)
            {
                _writer.WriteLine($"<p>{vendor.Name} says 'Sorry you can't afford that.'</p>", player.ConnectionId);
                return;
            }

            player.Money.Gold -= (int)Math.Floor(trueGoldValue);

            player.Inventory.Add(hasItem);

            // TODO: weight
            player.Weight += hasItem.Weight;

            _clientUi.UpdateScore(player);
            _clientUi.UpdateInventory(player);

            _writer.WriteLine($"<p>You buy {hasItem.Name.ToLower()} for {Math.Floor(trueGoldValue)} gold.</p>", player.ConnectionId);
        }

        public void BuyItem(string itemName, Room room, Player player)
        {
            if (int.TryParse(itemName, out var n))
            {
                BuyItem(n, room, player);
                return;
            }

            var vendor = room.Mobs.FirstOrDefault(x => x.Shopkeeper.Equals(true));

            if (vendor == null)
            {
                _writer.WriteLine("<p>You can't do that here.</p>", player.ConnectionId);
                return;
            }

            var hasItem = vendor.Inventory.FirstOrDefault(x =>
                x.Name.Contains(itemName, StringComparison.InvariantCultureIgnoreCase));

            if (hasItem == null)
            {
                _writer.WriteLine($"<p>{vendor.Name} says 'I don't sell that, please view my \'list\' of items for sale.'</p>", player.ConnectionId);
                return;
            }

            var haggleReduction = _passiveSkills.Haggle(player, vendor);
            var goldValue = AddMarkUp(hasItem.Value);
            var trueGoldValue = goldValue - Helpers.GetPercentage(haggleReduction, (int)goldValue);
            if (player.Money.Gold < trueGoldValue)
            {
                _writer.WriteLine($"<p>{vendor.Name} says 'Sorry you can't afford that.'</p>", player.ConnectionId);
                return;
            }

            player.Money.Gold -= (int)Math.Floor(trueGoldValue);

            player.Inventory.Add(hasItem);

            // TODO: weight
            player.Weight += hasItem.Weight;

            _clientUi.UpdateScore(player);
            _clientUi.UpdateInventory(player);

            _writer.WriteLine($"<p>You buy {hasItem.Name.ToLower()} for {Math.Floor(trueGoldValue)} gold.</p>", player.ConnectionId);
        }

        public void SellItem(string itemName, Room room, Player player)
        {
            var vendor = room.Mobs.FirstOrDefault(x => x.Shopkeeper.Equals(true));

            if (vendor == null)
            {
                _writer.WriteLine("<p>You can't do that here.</p>", player.ConnectionId);
                return;
            }

            var hasItem = player.Inventory.FirstOrDefault(x =>
                x.Name.Contains(itemName, StringComparison.InvariantCultureIgnoreCase));

            if (hasItem == null)
            {
                _writer.WriteLine($"<p>{vendor.Name} says 'You don't have that item.'</p>", player.ConnectionId);
                return;
            }

            var vendorInterested = vendor.Inventory.FirstOrDefault(x => x.ItemType.Equals(hasItem.ItemType));

            if (vendorInterested == null)
            {
                _writer.WriteLine($"<p>{vendor.Name} says 'I'm not interested in {hasItem.Name.ToLower()}.'</p>", player.ConnectionId);
                return;
            }

            var vendorBuyPrice = (int)Math.Floor((decimal)hasItem.Value / 2);

            player.Money.Gold += vendorBuyPrice <= 0 ? 1 : vendorBuyPrice;
            player.Inventory.Remove(hasItem);

            // if we wanted to show sold items in the vendors list we would add it here
            // currently we can't set limits so this would make all items sold infinite 

            //if (vendor.Inventory.FirstOrDefault(x => x.Name.Equals(hasItem.Name)) == null)
            //{
            //    vendor.Inventory.Add(hasItem);
            //}

            player.Weight -= hasItem.Weight;
            _clientUi.UpdateScore(player);
            _clientUi.UpdateInventory(player);

            _writer.WriteLine($"<p>You sell {hasItem.Name.ToLower()} for {(vendorBuyPrice <= 0 ? 1 : vendorBuyPrice)} gold.</p>", player.ConnectionId);
        }
    }
}
