using System.ComponentModel.DataAnnotations;

namespace ArchaicQuestII.GameLogic.Character;

public enum SkillName
{
    None,
    [Display(Name = "Lock Pick")]
    Lockpick,
    Blunt,
    Haggle,
    Grip,
    Berserk,
    Fishing,
    Headbutt,
    Throw,
    Riposte,
    Dodge,
    [Display(Name = "Long Blades")]
    LongBlades,
    [Display(Name = "Short Blades")]
    ShortBlades,
    Axe,
    Flail,
    Polearm,
    Hammer,
    Spear,
    Whip,
    Staff,
    Unarmed,
    Crafting,
    Cooking,
    Foraging,
    Lore,
    Elbow,
    [Display(Name = "Dirt Kick")]
    DirtKick,
    Kick,
    Crossbow,
    Bow,
    Parry,
    [Display(Name = "Fast Healing")]
    FastHealing,
    [Display(Name = "Shield Block")]
    ShieldBlock,
    Charge,
    Rescue,
    [Display(Name = "Uppercut")]
    UpperCut,
    Trip,
    Stab,
    Mount,
    [Display(Name = "Second Attack")]
    SecondAttack,
    Disarm,
    [Display(Name = "Enhanced Damage")]
    EnhancedDamage,
    WarCry,
    [Display(Name = "Shield Bash")]
    ShieldBash,
    Lunge,
    [Display(Name = "Blind Fighting")]
    BlindFighting,
    [Display(Name = "Dual Wield")]
    DualWield,
    [Display(Name = "Third Attack")]
    ThirdAttack,
    [Display(Name = "Hamstring")]
    HamString,
    Slash,
    Impale,
    Cleave,
    [Display(Name = "Fourth Attack")]
    FourthAttack,
    [Display(Name = "Overhead Crush")]
    OverheadCrush,
    [Display(Name = "Fifth Attack")]
    FifthAttack,
    Arrows,
}

