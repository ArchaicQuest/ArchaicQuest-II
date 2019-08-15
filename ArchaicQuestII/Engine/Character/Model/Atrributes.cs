using System;
using System.Collections.Generic;
using ArchaicQuestII.Engine.Effect;

namespace ArchaicQuestII.Engine.Character.Model
{
    public class Attributes
    {

        public Dictionary<EffectLocation, int> Attribute { get; set; } = new Dictionary<EffectLocation, int>
        {
            {EffectLocation.Strength, 0},
            {EffectLocation.Dexterity, 0},
            {EffectLocation.Constitution, 0},
            {EffectLocation.Wisdom, 0},
            {EffectLocation.Intelligence, 0},
            {EffectLocation.Charisma, 0},
            {EffectLocation.Hitpoints, 0},
        };
      
    }
}
