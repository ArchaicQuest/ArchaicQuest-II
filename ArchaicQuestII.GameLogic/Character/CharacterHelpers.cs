using System.Linq;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Utilities;

namespace ArchaicQuestII.GameLogic.Character;

public static class CharacterHelpers
{
    public static void AddSkills(this Player player, SubClassName className)
    {
        var c = CharacterHandler.Instance.GetClass(className); 

        if(c != null)
        {
            player.Skills.AddRange(c.Skills);
        }
    }

    public static void AddSkills(this Player player, ClassName className)
    {
        var c = CharacterHandler.Instance.GetClass(className); 

        if(c != null)
        {
            player.Skills.AddRange(c.Skills);
        }
    }

    public static SkillList GetSkill(this Player player, SkillName skillName)
    {
        return player.Skills.FirstOrDefault(x =>
            x.Name == skillName && player.Level >= x.Level);
    }

    public static SkillList GetSkill(this Player player, string skillName)
    {
        return player.Skills.FirstOrDefault(x =>
            x.Name.ToString().ToLower() == skillName && player.Level >= x.Level);
    }

    public static int GetWeaponSkill(this Player player, Item.Item weapon)
    {
        var weaponSkill = player.Skills.FirstOrDefault(x =>
            x.Name == weapon.WeaponType);

        return weaponSkill.Proficiency;
    }

    public static IClass GetClass(this Player player)
    {
        return CharacterHandler.Instance.GetClass(player.ClassName);
    }

    public static bool HasSkill(this Player player, SkillName skillName)
    {
        var skill = player.Skills.FirstOrDefault(x => x.Name == skillName);

        if(skill != null) return true;

        return false;
    }

    public static bool RollSkill(this Player player, SkillName skillName, int modifier = 0)
    {
        return player.Skills.FirstOrDefault(x => x.Name == skillName).Proficiency > DiceBag.Roll(1, 1, 100, modifier);
    }

    public static void FailedSkill(this Player player, SkillName name, out string message)
    {
        var skill = player.Skills.FirstOrDefault(x => x.Name == name);

        var increase = DiceBag.Roll(1, 1, 5);

        if (skill == null)
        {
            message = null;
            return;
        }

        if(skill.Proficiency == 100)
        {
            message = null;
            return;
        }

        skill.Proficiency += increase;

        if (skill.Proficiency > 100)
        {
            skill.Proficiency = 100;
        }

        player.GainExperiencePoints(100 * skill.Level / 4, out _);  
        
        message = $"<p class='improve'>You learn from your mistakes and gain {100 * skill.Level / 4} experience points.</p>" +
        $"<p class='improve'>Your knowledge of {skill.Name} increases by {increase}%.</p>";
    }
}