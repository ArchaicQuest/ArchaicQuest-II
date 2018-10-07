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
            Drink,
            Food,
            Forage,
            Key,
            Light,
            LockPick,
            Potion,
            Repair,
            Weapon,
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
            Holy,
            Hum,  //affect n/a
            Invis, //invisible
            Nodrop, // cannot drop
            Nolocate,   //locate spell fails
            Noremove, //cannot remove w/o remove curse
            QuestItem,
            Vampric, // Drains hp on hoit
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
            Arms,
            Body,
            Face,
            Feet,
            Finger,
            Floating,
            Hands,
            Head,
            Held,
            Legs,
            Light,
            Neck,
            Shield,
            Torso,
            Waist,
            Wielded,
            Wrist,

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
