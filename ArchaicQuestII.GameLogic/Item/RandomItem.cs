using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item.RandomItemTypes;

namespace ArchaicQuestII.GameLogic.Item
{
    public class ItemMods
    {
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public int MinArmour { get; set; }
        public int MaxArmour { get; set; }
        public int Hitpoints { get; set; }
        public int Mana { get; set; }
        public int Moves { get; set; }

    }

    public class PrefixItemMods : ItemMods
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Weapon
    {
        public string Name { get; set; }
        public Item.WeaponTypes WeaponType { get; set; }
        public Item.AttackTypes AttackTypes { get; set; }
        public string Description { get; set; }
    }


    public class RandomItem : IRandomItem
    {
        private IDice _dice;
        private IRandomWeapon _randomWeapon;
        public RandomItem(IDice dice, IRandomWeapon weapon)
        {
            _dice = dice;
            _randomWeapon = weapon;
        }


        public Item WeaponDrop(Player player)
        {
            var dropChance = 95;
            var roll = _dice.Roll(1, 1, 100);
            var legendary = false;
            if (roll <= dropChance)
            {
                if (roll == 1)
                {
                    legendary = true;
                }
                return _randomWeapon.CreateRandomWeapon(player, legendary);
            }

            return null;

        }
    }
}
