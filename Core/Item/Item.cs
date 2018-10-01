using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Core.Item
{
    public class Item : BaseItem
    {
        public enum ItemTypes
        {
            Armour,
            Book,
            Container,
            Food,
            Drink,
            Key,
            Light,
            Potion,
            Weapon,
            Forage,
            Repair,
            LockPick
        }


        public enum ItemFlags
        {
            Antievil, //zap if align -350 & lower
            Antigood, //zap if align +350 & lower
            Antineutral, //zap if align -350 to +350
            Bless, // +20% resist dam. 
            Container, // isContainer
            Cursed,
            Equipable, // can be equipped
            Evil, //glow on det.evil
            Glow, //lights area
            Hum,  //affect n/a
            Holy,
            Invis, //invisible
            Nodrop, // cannot drop
            Nolocate,   //locate spell fails
            Noremove, //cannot remove w/o remove curse
            Vampric, // Drains hp on hoit
            QuestItem
        }

        public enum ItemLocation
        {
            Room,
            Inventory,
            Worn,
            Wield
        }

        public enum EqSlot
        {
            Light,
            Finger,
            Neck,
            Face,
            Head,
            Torso,
            Legs,
            Feet,
            Hands,
            Arms,
            Body,
            Waist,
            Wrist,
            Wielded,
            Held,
            Shield,
            Floating

        }

        public enum AttackTypes
        {
            Slash,
            Slice,
            Stab,
            Thrust,
            Chop,
            Claw,
            Cleave,
            Charge,
            Crush,
            Smash,
            Pierce,
            Punch,
            Pound,
            Scratch,
            Slap,
            Whip,
        }


        public enum DamageTypes
        {
            Acidic,
            Blast,
            Chill,
            Flame,
            Flaming,
            Freezing,
            Shocking,
            Poisoned,
            Stun
        }

        public enum WeaponTypes
        {
            Axe,
            Exotic,
            Flail,
            Blunt,
            Polearm,
            Spear,
            Staff,
            LongBlades,
            ShortBlades,
            Whip,
            Bows,
            Arrows,
            Crossbow,
            Bolt,
            HandToHand
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


        public DamageTypes DamageType { get; set; }
        public ArmourTypes ArmourType { get; set; }
        public List<ItemFlags> ItemFlag { get; set; }
        public ItemTypes ItemType { get; set; }
        public AttackTypes AttackType { get; set; }
        public EqSlot Slot { get; set; }
        public WeaponTypes WeaponType { get; set; }
        public int WeaponSpeed { get; set; }
        public Dice Damage { get; set; }
        public Guid KeyId { get; set; }
        public Container Container { get; set; }
        public Book Book { get; set; }

        public ArmourRating ArmorRating { get; set; }
        public int Weight { get; set; } = 2;
 
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
    }
}
