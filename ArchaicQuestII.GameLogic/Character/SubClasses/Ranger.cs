using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Effect;

namespace ArchaicQuestII.GameLogic.Character.Class;

public class Ranger : IClass
{
    public string Name => SubClassName.Ranger.ToString();
    public string Description => "Warriors are lethal combatants who can use any weapon and armor with ease, " +
                    "relying on their strength and endurance instead of mana. With a wide range " +
                    "of offensive and defensive skills, they are a versatile class suitable for " +
                    "any race. For beginners, we highly recommend choosing a Human Warrior, as " +
                    "their high hit points and straightforward playstyle make them an easy class to learn.";
                    
    public string PreferredWeapon => SkillName.Bow.ToString();
    public string HitDice => "1D10";
    public int ExperiencePointsCost => 1000;
    public string CreatedBy => "Malleus";
    public DateTime DateCreated => DateTime.Now;
    public DateTime DateUpdated => DateTime.Now;

    public Attributes AttributeBonus => new Attributes()
    {
        Attribute = new Dictionary<EffectLocation, int>()
        {
            { EffectLocation.Strength, 2 },
        }
    };

    public List<SkillList> Skills => new List<SkillList>
    {
        
    };

    public List<SubClassName> Reclasses => throw new NotImplementedException();
}