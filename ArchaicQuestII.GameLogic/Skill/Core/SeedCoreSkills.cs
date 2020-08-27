using System;
using System.Collections.Generic;
using System.Text;

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
            var seedData = new List<Model.Skill>()
            {
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
                new EvasiveTypeSkills().ShieldBLock()
            };

            return seedData;
        }
    }
}
