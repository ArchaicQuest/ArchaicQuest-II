using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Character.Status
{  
    public class CharacterStatus
    {

        [Flags]
        public enum Status
        {
            Standing = 0,
            Sitting = 1,
            Sleeping = 2,
            Fighting = 3,
            Resting = 4,
            Incapitated = 5,
            Dead = 6,
            Ghost = 7,
            Busy = 8,
            Floating = 9,
            Mounted = 10,
            Stunned = 11

        }


        public List<Option> SeedData()
        {
            var seedData = new List<Option>()
            {
                new Option()
                {
                    Name = "Sitting",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Standing",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Sleeping",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Fighting",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Resting",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Incapitated",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Dead",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Ghost",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Busy",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Floating",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Mounted",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Stunned",
                    CreatedBy = "Malleus"
                }

            };

            return seedData;
        }
    }
}
