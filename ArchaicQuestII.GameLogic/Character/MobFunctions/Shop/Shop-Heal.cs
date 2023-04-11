using System;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Skill.Skills;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Character.MobFunctions.Shop
{
    public class ShopHeal : IMobFunctions
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _clientUi;
        private readonly IPassiveSkills _passiveSkills;

        public ShopHeal(
            IWriteToClient writer,
            IUpdateClientUI clientUi,
            IPassiveSkills passiveSkills
        )
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

            _writer.WriteLine(mob.Name + " says 'I offer the following spells:'", player);
            var sb = new StringBuilder();
            sb.Append(
                "<table class='data'><tr><td style='width: 30px; text-align: center;'>#</td><td style='width: 30px; text-align: center;'>Spell</td><td  style='width: 65px;'>Price</td</tr>"
            );

            int i = 0;
            foreach (var item in mob.SpellList.OrderBy(x => x.Cost))
            {
                i++;
                sb.Append(
                    $"<tr><td style='width: 30px; text-align: center;'>{i}</td><td style='width: 30px; text-align: center;'>{item.Name}</td><td  style='width: 65px;'>{DisplayUnit(item.Cost, hagglePriceReduction)}</td</tr>"
                );
            }

            sb.Append("</table>");
            _writer.WriteLine(sb.ToString(), player);
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
                _writer.WriteLine("<p>There is no one offering spells here.</p>", player);
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
                _writer.WriteLine("<p>You can't do that here.</p>", player);
                return;
            }

            var hasItem = vendor.Inventory
                .Distinct()
                .OrderBy(x => x.Level)
                .ThenBy(x => x.Value)
                .ToArray()[itemNumber];

            if (hasItem == null)
            {
                _writer.WriteLine(
                    $"<p>{vendor.Name} says 'I don't offer that, please view my \'heal\' list of spells for sale.'</p>",
                    player
                );
                return;
            }

            var haggleReduction = _passiveSkills.Haggle(player, vendor);
            var goldValue = AddMarkUp(hasItem.Value);
            var trueGoldValue = goldValue - Helpers.GetPercentage(haggleReduction, (int)goldValue);
            if (player.Money.Gold < trueGoldValue)
            {
                _writer.WriteLine(
                    $"<p>{vendor.Name} says 'Sorry you can't afford that.'</p>",
                    player
                );
                return;
            }

            player.Money.Gold -= (int)Math.Floor(trueGoldValue);

            // MOB cast spells

            _clientUi.UpdateScore(player);
            _clientUi.UpdateInventory(player);

            _writer.WriteLine(
                $"<p>You buy {hasItem.Name.ToLower()} for {Math.Floor(trueGoldValue)} gold.</p>",
                player
            );
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
                _writer.WriteLine("<p>You can't do that here.</p>", player);
                return;
            }

            var hasItem = vendor.Inventory.FirstOrDefault(
                x => x.Name.Contains(itemName, StringComparison.InvariantCultureIgnoreCase)
            );

            if (hasItem == null)
            {
                _writer.WriteLine(
                    $"<p>{vendor.Name} says 'I don't offer that, please view my \'heal\' list of spells for sale.'</p>",
                    player
                );
                return;
            }

            var haggleReduction = _passiveSkills.Haggle(player, vendor);
            var goldValue = AddMarkUp(hasItem.Value);
            var trueGoldValue = goldValue - Helpers.GetPercentage(haggleReduction, (int)goldValue);
            if (player.Money.Gold < trueGoldValue)
            {
                _writer.WriteLine(
                    $"<p>{vendor.Name} says 'Sorry you can't afford that.'</p>",
                    player
                );
                return;
            }

            player.Money.Gold -= (int)Math.Floor(trueGoldValue);

            // MOB CAST SPELL

            _clientUi.UpdateScore(player);
            _clientUi.UpdateInventory(player);

            _writer.WriteLine(
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
    }
}
