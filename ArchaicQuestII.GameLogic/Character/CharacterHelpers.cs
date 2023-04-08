using System.Linq;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Commands;

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
}