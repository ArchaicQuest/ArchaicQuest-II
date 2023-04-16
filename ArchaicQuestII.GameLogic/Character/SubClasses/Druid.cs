using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Effect;

namespace ArchaicQuestII.GameLogic.Character.Class;

public class Druid : IClass
{
    public int Id { get; set; }
    public bool IsSubClass => true;
    public string Name => SubClassName.Druid.ToString();
    public string Description => "Druid";

    public string PreferredWeapon => SkillName.Staff.ToString();
    public string HitDice => "1D10";
    public int ExperiencePointsCost => 1000;
    public string CreatedBy => "Ithdrak";
    public DateTime DateCreated => DateTime.Now;
    public DateTime DateUpdated => DateTime.Now;

    public Attributes AttributeBonus =>
        new Attributes()
        {
            Attribute = new Dictionary<EffectLocation, int>() { { EffectLocation.Strength, 2 }, }
        };

    public List<Item.Item> StartingGear => new List<Item.Item> { };

    public List<SkillList> Skills => new List<SkillList> { };

    public List<SubClassName> Reclasses => null;
}
