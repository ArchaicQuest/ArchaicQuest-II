using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ArchaicQuestII.GameLogic.Character.Equipment;
using static ArchaicQuestII.GameLogic.Character.Equipment.Equipment;

namespace ArchaicQuestII.GameLogic.Item
{
    public class Item: BaseItem
    {
        public enum ItemTypes
        {
            Armour,
            Book,
            Container,
            Drink,
            Food,
            Forage,
            Key,
            Light,
            LockPick,
            Potion,
            Repair,
            Weapon,
            Seating,
            Inanimate,
            Money,
            Portal,
            Crafting,
            Material,
        }

        [Flags]
        public enum ItemFlags
        {
            None = 0,
            Antievil = 1 << 0, //zap if align -350 & lower
            Antigood = 1 << 1, //zap if align +350 & lower
            Antineutral = 1 << 2, //zap if align -350 to +350
            Bless = 1 << 3, // +20% resist dam.
            Container = 1 << 4, // isContainer
            Cursed = 1 << 5,
            Equipable = 1 << 6, // can be equipped
            Evil = 1 << 7, //glow on det.evil
            Glow = 1 << 8, //lights area
            Holy = 1 << 9,
            Hum = 1 << 10, //affect n/a
            Invis = 1 << 11, //invisible
            Nodrop = 1 << 12, // cannot drop
            Nolocate = 1 << 13,   //locate spell fails
            Noremove = 1 << 14, //cannot remove w/o remove curse
            QuestItem = 1 << 15,
            Vampric = 1 << 16, // Drains hp on hoit
        }

        public enum ItemLocation
        {
            Room,
            Inventory,
            Worn,
            Wield
        }



        public enum AttackTypes
        {
            Charge,
            Chop,
            Claw,
            Cleave,
            Crush,
            Pierce,
            Pound,
            Punch,
            Scratch,
            Slap,
            Slash,
            Slice,
            Smash,
            Stab,
            Thrust,
            Thwack,
            Whip
        }


        public enum DamageTypes
        {
            None,
            Acidic,
            Blast,
            Chill,
            Divine,
            Flame,
            Flaming,
            Freezing,
            Poisoned,
            Shocking,
            Stun,
            Wrath
        }

        public enum WeaponTypes
        {
            Arrows,
            Axe,
            Blunt,
            Bolt,
            Bows,
            Crossbow,
            Exotic,
            Flail,
            [Display(Name = "Hand to hand")]
            HandToHand,
            [Display(Name = "Long blades")]
            LongBlades,
            Polearm,
            [Display(Name = "Short blades")]
            ShortBlades,
            Spear,
            Staff,
            Whip,
        }

        public enum ArmourTypes
        {
            Cloth,
            Leather,
            [Display(Name = "Studded Leather")]
            StuddedLeather,
            [Display(Name = "Chain Mail")]
            ChainMail,
            [Display(Name = "Plate Mail")]
            PlateMail,

        }

        public enum LockStrength
        {
            Simple,
            Easy,
            Medium,
            Hard,
            Impossible,
        }


        public bool Equipped { get; set; }
        public DamageTypes DamageType { get; set; }
        public ArmourTypes ArmourType { get; set; }
        public ItemFlags ItemFlag { get; set; }
        public ItemTypes ItemType { get; set; }
        public AttackTypes AttackType { get; set; }
        public EqSlot Slot { get; set; }
        public WeaponTypes WeaponType { get; set; }
        public int WeaponSpeed { get; set; }
        public Damage Damage { get; set; }
        public Guid KeyId { get; set; }
        public Container Container { get; set; }
        public Book Book { get; set; }
        public Modifier Modifier { get; set; }
        public ArmourRating ArmourRating { get; set; }
        public float Weight { get; set; } = 2;

        public int Gold { get; set; } = 0;
        public int Silver { get; set; } = 0;

        /// <summary>
        /// used for foraging, is rank is equal or less than player level
        /// player can find item
        /// </summary>
        public int ForageRank { get; set; } = 1;
        /// <summary>
        /// Used for wands, starves and repai hammers to determine how many uses they have left
        /// </summary>
        public int Uses { get; set; }
        /// <summary>
        /// Infinite uses
        /// </summary>
        public bool Infinite { get; set; }

        public bool Deleted { get; set; }

        public Portal Portal { get; set; }
    }
}
