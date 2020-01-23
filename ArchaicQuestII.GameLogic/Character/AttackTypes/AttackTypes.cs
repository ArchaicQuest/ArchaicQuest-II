using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Character.AttackTypes
{
    public class AttackTypes
    {
        /// <summary>
        /// Only called on application start up
        /// This is to populate the system with sensible defaults
        /// </summary>
        /// <returns></returns>
        public List<Option> SeedData()
        {
            var seedData = new List<Option>()
            {
                new Option()
                {
                    Name = "Punch",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Pound",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Bite",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Charge",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Peck",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Headbutt",
                    CreatedBy = "Malleus"
                },
            };

            return seedData;
        }
    }
}
