using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Engine.Effect
{
    public enum EffectLocation
    {
            None = 0,
            Strength = 1 << 0,
            Dexterity = 1 << 1,  
            Constitution = 1 << 2,  
            Intelligence = 1 << 3, 
            Wisdom = 1 << 4,  
            Charisma = 1 << 5,
            Luck = 1 << 6,  
            Hitpoints = 1 << 7,  
            Mana = 1 << 8,  
            Moves = 1 << 9,
            Armour = 1 << 10,  
            HitRoll = 1 << 11,  
            SavingSpell = 1 << 12,  
            DamageRoll = 1 << 13,   
            Gender = 1 << 14,  
            Age = 1 << 15,
            Weight = 1 << 16,  
            Height = 1 << 16, 
            Level = 1 << 16,  
    }
}
