using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells;
using AllSpells = ArchaicQuestII.GameLogic.Spell.AllSpells;

namespace ArchaicQuestII.GameLogic.Skill.Core
{
    public class SeedCoreSkills
    {
        /// <summary>
        /// Only called on application start up
        /// This is to populate the system with sensible defaults
        /// </summary>
        /// <returns></returns>
        public List<Model.Skill> SeedData()
        {
            var skill = new DefineSkill();
            var seedData = new List<Model.Skill>()
            {
                new CraftingSkills().Cooking(),
                new CraftingSkills().Crafting(),
                new WeaponTypeSkills().Crossbow(),
                new WeaponTypeSkills().Flail(),
                new WeaponTypeSkills().HandToHand(),
                new WeaponTypeSkills().LongBlade(),
                new WeaponTypeSkills().Axe(),
                new WeaponTypeSkills().Polearm(),
                new WeaponTypeSkills().ShortBlades(),
                new WeaponTypeSkills().Spear(),
                new WeaponTypeSkills().Staff(),
                new WeaponTypeSkills().Whip(),
                new WeaponTypeSkills().Blunt(),
                new WeaponTypeSkills().Bows(),
                new WeaponTypeSkills().Exotic(),
                new EvasiveTypeSkills().Dodge(),
                new EvasiveTypeSkills().Parry(),
                new EvasiveTypeSkills().ShieldBLock(),
                new AllSpells().MagicMissile(),
                new AllSpells().CauseWounds(),
                new AllSpells().CureWounds(),
                new AllSpells().Armour(),
                new AllSpells().Bless(),
                skill.Kick(),
                skill.Elbow(),
                skill.Lore(),
                skill.Trip(),
                skill.Haggle(),
                skill.HeadButt(),
                skill.Charge(),
                skill.FastHealing(),
                skill.Stab(),
                skill.Uppercut(),
                skill.DirtKick(),
                skill.Disarm(),
                skill.Lunge(),
                skill.Berserk(),
                skill.Rescue(),
                skill.SecondAttack(),
                skill.ThirdAttack(),
                skill.FouthAttack(),
                skill.FithAttack(),
                skill.Mount(),
                skill.BlindFighting(),
                skill.ShieldBash(),
                skill.DualWield(),
                skill.EnhancedDamage()
            };

            return seedData;
        }
    }
}
