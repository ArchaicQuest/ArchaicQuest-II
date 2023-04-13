using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Utilities;

namespace ArchaicQuestII.GameLogic.Item.RandomItemTypes
{
    public class RandomWeapons : IRandomWeapon
    {
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
                Description =
                    "shimmering a turquoise colour, it appears solid with wisps of Astral energy rotating around the #weaponName# from the grip to the tip like a turquoise flame."
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
                Description =
                    "expertly crafted with intricate elvian patterns decorating the weapon."
            },
            new PrefixItemMods()
            {
                Name = "Bone",
                MinDamage = 6,
                MaxDamage = 12,
                Description =
                    "elaborately carved and stained brown with various sigils carved into the bone."
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
                WeaponType = SkillName.ShortBlades,
                AttackTypes = Item.AttackTypes.Slash,
                Description =
                    "A good sized #prefix# dagger with an ornate hilt, the blade is #prefixDescription"
            },
            new Weapon()
            {
                Name = "Long Sword",
                WeaponType = SkillName.LongBlades,
                AttackTypes = Item.AttackTypes.Slash,
                Description =
                    "A good sized #prefix# long sword with an ornate hilt, the blade is #prefixDescription"
            },
            new Weapon()
            {
                Name = "Short Sword",
                WeaponType = SkillName.ShortBlades,
                AttackTypes = Item.AttackTypes.Slash,
                Description =
                    "A good sized #prefix# short sword with an ornate hilt, the blade is #prefixDescription"
            },
            new Weapon()
            {
                Name = "Scimitar",
                WeaponType = SkillName.LongBlades,
                AttackTypes = Item.AttackTypes.Slash,
                Description =
                    "A good sized #prefix# scimitar with an ornate hilt, the blade is #prefixDescription"
            },
            new Weapon()
            {
                Name = "Polearm",
                WeaponType = SkillName.Polearm,
                AttackTypes = Item.AttackTypes.Smash,
                Description =
                    "A good sized #prefix# polearm with an ornate shaft, the spiked topped axe head is #prefixDescription"
            },
            new Weapon()
            {
                Name = "Trident",
                WeaponType = SkillName.Polearm,
                AttackTypes = Item.AttackTypes.Pierce,
                Description =
                    "A good sized #prefix# trident with an ornate shaft, the three sharp prongs are #prefixDescription"
            },
            new Weapon()
            {
                Name = "Mace",
                WeaponType = SkillName.Hammer,
                AttackTypes = Item.AttackTypes.Crush,
                Description =
                    "A good sized #prefix# mace with an ornate shaft and head that protrudes in multiple directions and is #prefixDescription"
            },
            new Weapon()
            {
                Name = "Morning Star",
                WeaponType = SkillName.Hammer,
                AttackTypes = Item.AttackTypes.Smash,
                Description =
                    "A good sized #prefix# morning star with an ornate shaft, the spiked ball is #prefixDescription"
            },
            new Weapon()
            {
                Name = "Quarterstaff",
                WeaponType = SkillName.Staff,
                AttackTypes = Item.AttackTypes.Crush,
                Description =
                    "A good sized #prefix# quarterstaff with an ornate shaft, the quarterstaff is #prefixDescription"
            },
            new Weapon()
            {
                Name = "Axe",
                WeaponType = SkillName.Axe,
                AttackTypes = Item.AttackTypes.Cleave,
                Description =
                    "A good sized #prefix# axe with an ornate shaft, the axe head is #prefixDescription"
            },
            new Weapon()
            {
                Name = "Double-edge Axe",
                WeaponType = SkillName.Axe,
                AttackTypes = Item.AttackTypes.Cleave,
                Description =
                    "A good sized #prefix# double-edge axe with an ornate shaft, the double sided blade is #prefixDescription"
            }
        };

        public Item CreateRandomWeapon(Player player, bool legendary)
        {
            var prefix = Prefix[DiceBag.Roll(1, 0, Prefix.Count)];
            var weaponChoice = WeaponNames[DiceBag.Roll(1, 0, WeaponNames.Count)];

            var item = new Item()
            {
                Name = "a " + prefix.Name + " " + weaponChoice.Name,
                ItemType = Item.ItemTypes.Weapon,
                Level = player.Level,
                Value = player.Level * 75,
                Condition = DiceBag.Roll(1, 75, 100),
                WeaponType = weaponChoice.WeaponType,
                Weight = 11,
                Modifier = new Modifier(),
                Damage = new Damage() { Minimum = prefix.MinDamage, Maximum = prefix.MaxDamage },
                Gold = player.Level * 75,
                Description = new Description() { Look = $"a {prefix.Name} {weaponChoice.Name}", },
                Slot = EquipmentSlot.Wielded,
                AttackType = weaponChoice.AttackTypes,
            };

            // stats to buff

            for (int i = 0; i < (legendary ? 5 : 3); i++)
            {
                switch (DiceBag.Roll(1, 1, 16))
                {
                    case 1:
                        item.Modifier.Armour = DiceBag.Roll(1, 1, 10);
                        break;

                    case 2:
                        item.Modifier.Charisma = DiceBag.Roll(1, 1, 10);
                        break;

                    case 3:
                        item.Modifier.Constitution = DiceBag.Roll(1, 1, 10);
                        break;

                    case 4:
                        item.Modifier.Dexterity = DiceBag.Roll(1, 1, 10);
                        break;

                    case 5:
                        item.Modifier.Intelligence = DiceBag.Roll(1, 1, 10);
                        break;

                    case 6:
                        item.Modifier.Mana = DiceBag.Roll(1, 1, 10);
                        break;

                    case 7:
                        item.Modifier.Moves = DiceBag.Roll(1, 1, 10);
                        break;

                    case 8:
                        item.Modifier.Saves = DiceBag.Roll(1, 1, 10);
                        break;
                    case 9:
                        item.Modifier.Strength = DiceBag.Roll(1, 1, 10);
                        break;
                    case 10:
                        item.Modifier.Wisdom = DiceBag.Roll(1, 1, 10);
                        break;
                    case 11:
                        item.Modifier.AcMod = DiceBag.Roll(1, 1, 10);
                        break;
                    case 12:
                        item.Modifier.DamRoll = DiceBag.Roll(1, 1, 10);
                        break;
                    case 13:
                        item.Modifier.HitRoll = DiceBag.Roll(1, 1, 10);
                        break;
                    case 14:
                        item.Modifier.HP = DiceBag.Roll(1, 1, 10);
                        break;
                    case 15:
                        item.Modifier.SpellDam = DiceBag.Roll(1, 1, 10);
                        break;
                    case 16:
                        item.Modifier.AcMagicMod = DiceBag.Roll(1, 1, 10);

                        break;
                }
            }

            if (legendary)
            {
                item.Damage.Minimum += DiceBag.Roll(1, prefix.MinDamage, prefix.MinDamage * 2);
                item.Damage.Maximum += DiceBag.Roll(1, prefix.MaxDamage, prefix.MaxDamage * 2);
                item.Condition = 100;

                item.Name += " (Legendary)";
            }

            return item;
        }
    }
}
