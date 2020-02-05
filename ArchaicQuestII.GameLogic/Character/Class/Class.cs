using ArchaicQuestII.GameLogic.Core;
using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.Character.Class
{
    public class Class : OptionDescriptive
    {
        public List<Class> SeedData()
        {
            var seedData = new List<Class>()
            {
                new Class()
                {
                    Name = "Fighter",
                    Description =
                        @"Warriors are able to use any weapon and armour effectively along side their wide range of lethal and defensive combat skills.
                    They have no need for mana, relying on their sheer strength and endurance alone to overcome opponents.
                    Important attributes for Warriors are Strength, Dexterity and Constitution Every race can train to be an effective warrior.
                    For beginners we recommend you pick a Human Warrior."
                },

                new Class()
                {
                    Name = "Thief",
                    Description =
                        @"Rogues are masters at the arts of remaining hidden and delivering devastating blows from the shadows before fleeing
                     into the darkness once more. They are strong in combat but can't handle the same amount of damage as a warrior.
                     They are also skilled lock and pocket pickers, can set or disarm traps and know how to apply poison to their blade.
                      Rogues are a versatile class. Important attributes for Mages are Dexterity, Constitution and Strength Every race can train
                      to be an rogue but Mau are one of the best due to their agile nature."
                },
                new Class()
                {   
                    Name = "Cleric",
                    Description =  @"Cleric power comes from the gods they worship, stronger the devotion, stronger the power,
            Clerical spells focus on healing and preserving life rather than destroy it but don't be fooled clerics
             know powerful offensive spells to rival any mage. They can also wear any armour just like a warrior.
             Important attributes for Clerics are Wisdom, Intelligence and Constitution
              Every race can train to be a cleric but Dwarfs are one of the best."
                },
                new Class()
                {
                    Name = "Mage",
                    Description =
                        @"Mages are the most feared across the realm due to their devastating spells and power.
             The road to such power is a hard, slow journey. Mages struggle more than other classes in melee combat
              because They spent years studying magic and how to hurl a ball of fire towards their opponent instead
               of training for physical combat. This makes mages relatively weak at the beginning of their training
                but this changes however when a they have mastered the arts of magic. Important attributes for Mages
                 are Intelligence, Wisdom and Dexterity Every race can train to be a mage but Elves are the best."
                }
            };

            return seedData;
        }



    }
}
