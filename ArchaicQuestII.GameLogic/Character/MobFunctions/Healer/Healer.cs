﻿using System;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Character.MobFunctions.Healer
{
    public class Healer : IHealer
    {
        public void DisplayInventory(Player mob, Player player)
        {
            var hagglePriceReduction = Haggle(player, mob);

            Services.Instance.Writer.WriteLine(
                mob.Name + " says 'I offer the following spells:'",
                player
            );
            var sb = new StringBuilder();
            sb.Append(
                "<table class='data'><tr><td style='width: 275px; text-align: left;'>Spell</td><td>Price</td</tr>"
            );

            int i = 0;
            foreach (var item in mob.SpellList.OrderBy(x => x.Cost))
            {
                i++;
                sb.Append(
                    $"<tr><td style='width: 275px; text-align: left;'>{item.Name}</td><td>{DisplayUnit(item.Cost, hagglePriceReduction)} GP</td></tr>"
                );
            }

            sb.Append("</table>");
            sb.Append("<p>Type heal &lt;type&gt; to be healed.</p>");
            Services.Instance.Writer.WriteLine(sb.ToString(), player);
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

        public void List(Room room, Player player, string spellName)
        {
            if (spellName.Equals("heal"))
            {
                var shopKeeper = FindShopKeeper(room);
                if (shopKeeper == null)
                {
                    Services.Instance.Writer.WriteLine(
                        "<p>There is no one offering spells here.</p>",
                        player
                    );
                    return;
                }

                DisplayInventory(shopKeeper, player);
                return;
            }

            BuyItem(spellName, room, player);
        }

        public int Haggle(Player player, Player target)
        {
            var priceReduction = Services.Instance.PassiveSkills.Haggle(player, target);

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

        public void BuyItem(string itemName, Room room, Player player)
        {
            var vendor = room.Mobs.FirstOrDefault(x => x.Shopkeeper.Equals(true));

            if (vendor == null)
            {
                Services.Instance.Writer.WriteLine("<p>You can't do that here.</p>", player);
                return;
            }

            var hasItem = vendor.SpellList.FirstOrDefault(
                x => x.Name.StartsWith(itemName, StringComparison.InvariantCultureIgnoreCase)
            );

            if (hasItem == null)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>{vendor.Name} says 'I don't offer that, please view my \'heal\' list of spells for sale.'</p>",
                    player
                );
                return;
            }

            var haggleReduction = Services.Instance.PassiveSkills.Haggle(player, vendor);
            var goldValue = AddMarkUp(hasItem.Cost);
            var trueGoldValue = goldValue - Helpers.GetPercentage(haggleReduction, (int)goldValue);
            if (player.Money.Gold < trueGoldValue)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>{vendor.Name} says 'Sorry you can't afford that.'</p>",
                    player
                );
                return;
            }

            player.Money.Gold -= (int)Math.Floor(trueGoldValue);

            //_spells.DoSpell("cure light wounds", vendor, player.Name, room);

            // MOB CAST SPELL

            Services.Instance.UpdateClient.UpdateScore(player);
            Services.Instance.UpdateClient.UpdateInventory(player);

            Services.Instance.Writer.WriteLine(
                $"<p>You buy {hasItem.Name.ToLower()} for {Math.Floor(trueGoldValue)} gold.</p>",
                player
            );
        }

        public void InspectItem(int itemNumber, Room room, Player player)
        {
            throw new NotImplementedException();
        }

        public void InspectItem(string itemName, Room room, Player player)
        {
            throw new NotImplementedException();
        }

        public void SellItem(string itemName, Room room, Player player)
        {
            throw new NotImplementedException();
        }

        public void BuyItem(int itemNumber, Room room, Player player)
        {
            throw new NotImplementedException();
        }
    }
}
