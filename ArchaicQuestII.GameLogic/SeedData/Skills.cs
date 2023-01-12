using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Skill;
using ArchaicQuestII.GameLogic.Skill.Core;
using ArchaicQuestII.GameLogic.Spell;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Skill.Model;
using DefineSkill = ArchaicQuestII.GameLogic.Skill.Model.DefineSkill;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class Skills
    {
        internal static void SeedAndCache(IDataBase db, ICache cache)
        {
 
            var seedData = new List<Skill.Model.Skill>()
            {
                DefineSkill.Axe(),
                DefineSkill.Blunt(),
                DefineSkill.Bows(),
                DefineSkill.Charge(),
                DefineSkill.Cleave(),
                DefineSkill.Crossbow(),
                DefineSkill.Elbow(),
                DefineSkill.Exotic(),
                DefineSkill.Flail(),
                DefineSkill.Headbutt(),
                DefineSkill.Impale(),
                DefineSkill.Kick(),
                DefineSkill.Lore(),
                DefineSkill.Lunge(),
                DefineSkill.Polearm(),
                DefineSkill.Slash(),
                DefineSkill.Spear(),
                DefineSkill.Stab(),
                DefineSkill.Staff(),
                DefineSkill.Trip(),
                DefineSkill.Whip(),
                DefineSkill.DirtKick(),
                DefineSkill.HamString(),
                DefineSkill.LongBlade(),
                DefineSkill.OverheadCrush(),
                DefineSkill.ShieldBash(),
                DefineSkill.ShortBlades(),
                DefineSkill.UpperCut(),
                DefineSkill.HandToHand()
                
               /* new CraftingSkills().Cooking(),
                new CraftingSkills().Crafting(),
                new CraftingSkills().Foraging(),
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
                 new AllSpells().Identify(),
                DefineOffensiveSkills.Kick(),
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
                skill.EnhancedDamage(),
                skill.WarCry(),
                skill.Hamstring(),
                skill.Impale(),
                skill.Slash(),
                skill.OverheadCrush(),
                skill.Cleave()*/
            };

            if (!db.DoesCollectionExist(DataBase.Collections.Skill))
            {
                foreach (var seed in seedData)
                {
                    db.Save(seed, DataBase.Collections.Skill);
                }
            }
            else
            {
                var currentSkills = db.GetList<Skill.Model.Skill>(DataBase.Collections.Skill);
                foreach (var skillSeed in seedData)
                {
                    if (!currentSkills.Any(x => x.Name == skillSeed.Name))
                    {
                        db.Save(skillSeed, DataBase.Collections.Skill);
                    }
                }
            }

            foreach (var skillSeed in seedData)
            {
                skillSeed.Id = seedData.Count > 0 ? seedData.Max(x => x.Id) + 1 : 1;
                cache.AddSkill(skillSeed.Id, skillSeed);
            }
        }
    }
}