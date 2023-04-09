using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Commands;

namespace ArchaicQuestII.GameLogic.Character.Class;

public interface IClass
{
    public int Id { get; set; }
    public bool IsSubClass { get; }
    public string Name { get; }
    public string Description { get; }
    public string PreferredWeapon { get; }
    public string HitDice { get; }
    public int ExperiencePointsCost { get; }
    public string CreatedBy { get; }
    public DateTime DateCreated { get; }
    public DateTime DateUpdated { get; }
    public Attributes AttributeBonus { get; }
    public List<SubClassName> Reclasses { get; }
    public List<SkillList> Skills { get; }
}
