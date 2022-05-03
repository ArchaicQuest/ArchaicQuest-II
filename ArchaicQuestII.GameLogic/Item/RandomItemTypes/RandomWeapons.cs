using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Item.RandomItemTypes
{
    public class RandomWeapons : IRandomWeapon
    {

        private IDice _dice;

        public RandomWeapons(IDice dice)
        {
            _dice = dice;
        }

        // Fiery Iron Dagger as an example
        public List<PrefixItemMods> Prefix = new List<PrefixItemMods>()
        {
            new PrefixItemMods()
            {
                Name = "Rusty",
                MinDamage = 1,
                MaxDamage = 2,
                Description = "basic and rusty, don't expect much."
            },
            new PrefixItemMods()
            {
                Name = "Broken",
                MinDamage = 1,
                MaxDamage = 2,
                Description = "broken, don't expect much."
            },
            new PrefixItemMods()
            {
                Name = "Cursed",
                MinDamage = 1,
                MaxDamage = 4,
                Description = "appears cursed."
            },
            new PrefixItemMods()
            {
                Name = "Bronze",
                MinDamage = 1,
                MaxDamage = 5,
                Description = "crafted from bronze."
            },
            new PrefixItemMods()
            {
                Name = "Iron",
                MinDamage = 2,
                MaxDamage = 8,
                Description = "crafted from iron."
            },
            new PrefixItemMods()
            {
                Name = "Steel",
                MinDamage = 4,
                MaxDamage = 10,
                Description = "crafted from steel."
            },
            new PrefixItemMods()
            {
                Name = "Silver",
                MinDamage = 6,
                MaxDamage = 12,
                Description = "crafted expertly from silver."
            },
            new PrefixItemMods()
            {
                Name = "Gold",
                MinDamage = 8,
                MaxDamage = 14,
                Description = "crafted expertly from gold."
            },
            new PrefixItemMods()
            {
                Name = "Astral",
                MinDamage = 10,
                MaxDamage = 16,
                Description = "shimmering a turquoise colour, it appears solid with wisps of Astral energy rotating around the #weaponName# from the grip to the tip like a turquoise flame."
            },
            new PrefixItemMods()
            {
                Name = "Diamond",
                MinDamage = 12,
                MaxDamage = 18,
                Description = "pure quartz, translucent and very sharp to touch."

            },
            new PrefixItemMods()
            {
                Name = "Dwarven",
                MinDamage = 6,
                MaxDamage = 12,
                Description = "a dull golden colour, thick and heavy with a deadly sharp edge."
            },
            new PrefixItemMods()
            {
                Name = "Elvian",
                MinDamage = 6,
                MaxDamage = 12,
                Description = "expertly crafted with intricate elvian patterns decorating the weapon."
            },
            new PrefixItemMods()
            {
                Name = "Bone",
                MinDamage = 6,
                MaxDamage = 12,
                Description = "elaborately carved and stained brown with various sigils carved into the bone."
            },
            new PrefixItemMods()
            {
                Name = "Divine",
                MinDamage = 6,
                MaxDamage = 12,
                Description = "glowing a brilliant white light."
            },
            new PrefixItemMods()
            {
                Name = "Unholy",
                MinDamage = 6,
                MaxDamage = 12,
                Description = "sickly dark as if sucking light itself away."
            },
            new PrefixItemMods()
            {
                Name = "rune-covered",
                MinDamage = 5,
                MaxDamage = 12,
                Description = "covered in runes and glows a slight red."
            },
        };

        //public List<PrefixItemMods> ElementalPrefix = new List<PrefixItemMods>()
        //{
        //    new PrefixItemMods()
        //    {
        //        Name = "Fiery",
        //        MinDamage = 1,
        //        MaxDamage = 2
        //    },
        //    new PrefixItemMods()
        //    {
        //        Name = "Shocking",
        //        MinDamage = -1,
        //        MaxDamage = -2
        //    },
        //    new PrefixItemMods()
        //    {
        //        Name = "Frozen",
        //        MinDamage = -1,
        //        MaxDamage = -4
        //    },
        //    new PrefixItemMods()
        //    {
        //        Name = "Vampric",
        //        MinDamage = 1,
        //        MaxDamage = 5
        //    },
        //    new PrefixItemMods()
        //    {
        //        Name = "Poisoned",
        //        MinDamage = 2,
        //        MaxDamage = 8
        //    },
        //    new PrefixItemMods()
        //    {
        //        Name = "Vorpal",
        //        MinDamage = 4,
        //        MaxDamage = 10
        //    },
        //};

        public List<Weapon> WeaponNames = new List<Weapon>()
        {

            new Weapon()
            {
                Name = "Dagger",
                WeaponType = Item.WeaponTypes.ShortBlades,
                AttackTypes = Item.AttackTypes.Slash,
                Description = "A good sized #prefix# dagger with an ornate hilt, the blade is #prefixDescription"

            },
            new Weapon()
            {
                Name = "Long Sword",
                WeaponType = Item.WeaponTypes.LongBlades,
                AttackTypes = Item.AttackTypes.Slash,
                Description = "A good sized #prefix# long sword with an ornate hilt, the blade is #prefixDescription"

            },
            new Weapon()
            {
                Name = "Short Sword",
                WeaponType = Item.WeaponTypes.ShortBlades,
                AttackTypes = Item.AttackTypes.Slash,
                Description = "A good sized #prefix# short sword with an ornate hilt, the blade is #prefixDescription"

            },
            new Weapon()
            {
                Name = "Scimitar",
                WeaponType = Item.WeaponTypes.LongBlades,
                AttackTypes = Item.AttackTypes.Slash,
                Description = "A good sized #prefix# scimitar with an ornate hilt, the blade is #prefixDescription"
            },
            new Weapon()
            {
                Name = "Polearm",
                WeaponType = Item.WeaponTypes.Polearm,
                AttackTypes = Item.AttackTypes.Smash,
                Description = "A good sized #prefix# polearm with an ornate shaft, the spiked topped axe head is #prefixDescription"
            },
            new Weapon()
            {
                Name = "Trident",
                WeaponType = Item.WeaponTypes.Polearm,
                AttackTypes = Item.AttackTypes.Pierce,
                Description = "A good sized #prefix# trident with an ornate shaft, the three sharp prongs are #prefixDescription"
            },
            new Weapon()
            {
                Name = "Mace",
                WeaponType = Item.WeaponTypes.Blunt,
                AttackTypes = Item.AttackTypes.Crush,
                Description = "A good sized #prefix# mace with an ornate shaft and head that protrudes in multiple directions and is #prefixDescription"
            },
            new Weapon()
            {
                Name = "Morning Star",
                WeaponType = Item.WeaponTypes.Blunt,
                AttackTypes = Item.AttackTypes.Smash,
                Description = "A good sized #prefix# morning star with an ornate shaft, the spiked ball is #prefixDescription"

            },
            new Weapon()
            {
                Name = "Quarterstaff",
                WeaponType = Item.WeaponTypes.Blunt,
                AttackTypes = Item.AttackTypes.Crush,
                Description = "A good sized #prefix# quarterstaff with an ornate shaft, the quarterstaff is #prefixDescription"
            },
            new Weapon()
            {
                Name = "Axe",
                WeaponType = Item.WeaponTypes.Axe,
                AttackTypes = Item.AttackTypes.Cleave,
                Description = "A good sized #prefix# axe with an ornate shaft, the axe head is #prefixDescription"
            },
            new Weapon()
            {
                Name = "Double-edge Axe",
                WeaponType = Item.WeaponTypes.Axe,
                AttackTypes = Item.AttackTypes.Cleave,
                Description = "A good sized #prefix# double-edge axe with an ornate shaft, the double sided blade is #prefixDescription"
            }
        };

        public Item CreateRandomWeapon(Player player, bool legendary)
        {
            var prefix = Prefix[_dice.Roll(1, 0, Prefix.Count)];
            var weaponChoice = WeaponNames[_dice.Roll(1, 0, WeaponNames.Count)];


            var item = new Item()
            {
                Name = "a " + prefix.Name + " " + weaponChoice.Name,
                ItemType = Item.ItemTypes.Weapon,
                Level = player.Level,
                Value = player.Level * 75,
                Condition = _dice.Roll(1, 75, 100),
                WeaponType = weaponChoice.WeaponType,
                Weight = 11,
                Damage = new Damage()
                {
                    Minimum = prefix.MinDamage,
                    Maximum = prefix.MaxDamage
                },
                Gold = player.Level * 75,
                Description = new Description()
                {
                    Look = $"a {prefix.Name} {weaponChoice.Name}",
                },
                Slot = Equipment.EqSlot.Wielded,
                AttackType = weaponChoice.AttackTypes,

            };

            if (legendary)
            {
                item.Damage.Minimum += _dice.Roll(1, prefix.MinDamage, prefix.MinDamage * 2);
                item.Damage.Maximum += _dice.Roll(1, prefix.MaxDamage, prefix.MaxDamage * 2);
                item.Condition = 100;

                item.Name += " (Legendary)";
            }

            return item;

        }
    }
}
