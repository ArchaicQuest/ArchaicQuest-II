using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Item.RandomItemTypes;
using ArchaicQuestII.GameLogic.Utilities;

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
        private IRandomWeapon _randomWeapon;
        private IRandomClothItems _randomClothItems;
        private IRandomLeatherItems _randomLeatherItems;
        private IRandomStuddedLeatherArmour _randomStuddedItems;
        private IRandomChainMailArmour _randomChainMailItems;
        private IRandomPlateMailArmour _randomPlateMailItems;
        public RandomItem(
            IRandomWeapon weapon, 
            IRandomClothItems clothArmor, 
            IRandomLeatherItems leatherItems,
            IRandomStuddedLeatherArmour studdedLeather,
            IRandomChainMailArmour chainMail,
            IRandomPlateMailArmour plateMail)
        {
            _randomWeapon = weapon;
            _randomClothItems = clothArmor;
            _randomLeatherItems = leatherItems;
            _randomStuddedItems = studdedLeather;
            _randomChainMailItems = chainMail;
            _randomPlateMailItems = plateMail;
        }

        public Item CreateRandomItem(Player player, bool legendary)
        {
            var roll = DiceBag.Roll(1, 0, 5);

            return roll switch
            {
                0 => _randomWeapon.CreateRandomWeapon(player, legendary),
                1 => _randomClothItems.CreateRandomItem(player, legendary),
                2 => _randomLeatherItems.CreateRandomItem(player, legendary),
                3 => _randomStuddedItems.CreateRandomItem(player, legendary),
                4 => _randomChainMailItems.CreateRandomItem(player, legendary),
                5 => _randomPlateMailItems.CreateRandomItem(player, legendary),
                _ => _randomWeapon.CreateRandomWeapon(player, legendary),
            };
        }


        public Item WeaponDrop(Player player)
        {
            var dropChance = 5;
            var roll = DiceBag.Roll(1, 1, 250);
            var legendary = false;
            if (roll <= dropChance)
            {
                if (roll == 1)
                {
                    legendary = true;
                }

                return CreateRandomItem(player, legendary);
            }

            return null;

        }
    }
}
