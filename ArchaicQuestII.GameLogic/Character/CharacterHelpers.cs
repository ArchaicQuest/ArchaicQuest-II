using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Character;

public static class CharacterHelpers
{
    public static void AddSkills(this Player player, SubClassName className)
    {
        var c = Services.Instance.CharacterHandler.GetClass(className);

        if (c != null)
        {
            player.Skills.AddRange(c.Skills);
        }
    }

    public static void AddSkills(this Player player, ClassName className)
    {
        var c = Services.Instance.CharacterHandler.GetClass(className);

        if (c != null)
        {
            player.Skills.AddRange(c.Skills);
        }
    }

    public static void RemoveSkills(this Player player, ClassName className)
    {
        var c = Services.Instance.CharacterHandler.GetClass(className);

        if (c == null) return;
        
        // remove skills that are null or none
        player.Skills.RemoveAll(x => x.Name.Equals(SkillName.None));
            
        // remove skills no longer part of class
        for (var i = player.Skills.Count - 1; i >= 0; i--)
        {
            if (c.Skills.FirstOrDefault(x => x.Name == player.Skills[i].Name) == null)
            {
                player.Skills.Remove(player.Skills[i]);
            }
        }
    }
    
    public static void UpdateSkillLevel(this Player player, ClassName className)
    {
        var c = Services.Instance.CharacterHandler.GetClass(className);
        if (c == null) return;
        
        // update skills incase levels get updated
        for (var i = player.Skills.Count - 1; i >= 0; i--)
        {
            var skill = c.Skills.FirstOrDefault(x => x.Name == player.Skills[i].Name);

            if (skill != null)
            {
                player.Skills[i].Level = skill.Level;
            }
        }
    }
    public static SkillList GetSkill(this Player player, SkillName skillName)
    {
        return player.Skills.FirstOrDefault(x => x.Name == skillName && player.Level >= x.Level);
    }

    public static SkillList GetSkill(this Player player, string skillName)
    {
        return player.Skills.FirstOrDefault(
            x => x.Name.ToString().ToLower() == skillName && player.Level >= x.Level
        );
    }

    public static int GetWeaponSkill(this Player player, Item.Item weapon)
    {
        var weaponSkill = player.Skills.FirstOrDefault(x => x.Name == weapon.WeaponType);

        return weaponSkill.Proficiency;
    }

    public static IClass GetClass(this Player player)
    {
        return Services.Instance.CharacterHandler.GetClass(player.ClassName);
    }

    public static bool HasSkill(this Player player, SkillName skillName)
    {
        var skill = player.GetSkill(skillName);

        return skill == null ? false : true;
    }

    public static bool RollSkill(
        this Player player,
        SkillName skillName,
        bool display,
        string customErrorText = "",
        int modifier = 0
    )
    {
        var skill = player.GetSkill(skillName);

        if (skill == null)
            return false;

        var roll = DiceBag.Roll(1, 1, 100);

        var success = skill.Proficiency > roll + modifier;

        if (roll == 1 && display)
        {
            var errorText = skill.IsSpell
                ? $"<p>You tried to cast {skill.Name} but failed miserably.</p>"
                : $"<p>You tried to {skill.Name} but failed miserably.</p>";

            Services.Instance.Writer.WriteLine(errorText, player);
            return false;
        }

        if (!success && display)
        {
            var failedSkillMessage = customErrorText ?? $"<p>You try to {skill.Name} but fail.</p>";

            var errorText = skill.IsSpell ? $"<p>You lost concentration.</p>" : failedSkillMessage;

            Services.Instance.Writer.WriteLine(errorText, player);
        }

        return success;
    }

    public static void FailedSkill(this Player player, SkillName name, bool display)
    {
        var skill = player.Skills.FirstOrDefault(x => x.Name == name);

        if (skill == null)
        {
            Services.Instance.Writer.WriteLine("Skill not found", player);
            return;
        }

        var increase = DiceBag.Roll(1, 1, 5);

        if (skill.Proficiency == 100)
        {
            return;
        }

        skill.Proficiency += increase;

        if (skill.Proficiency > 100)
        {
            skill.Proficiency = 100;
        }

        player.GainExperiencePoints(100 * skill.Level / 4, false);

        player.UpdateClientUI();

        Services.Instance.Writer.WriteLine(
            $"<p class='improve'>You learn from your mistakes and gain {100 * skill.Level / 4} experience points.</p>"
                + $"<p class='improve'>Your knowledge of {skill.Name} increases by {increase}%.</p>",
            player
        );
    }

    public static int GetExpWorth(this Player character)
    {
        var maxEXP = 10000;
        var exp = character.Level;
        exp += DiceBag.Roll(1, 25, 275);
        exp += character.Equipped.Wielded?.Damage.Maximum ?? 6; // 6 for hand to hand
        exp += character.Attributes.Attribute[EffectLocation.DamageRoll] * 10;
        exp += character.Attributes.Attribute[EffectLocation.HitRoll] + character.Level * 10;
        exp += character.ArmorRating.Armour;

        exp += character.Attributes.Attribute[EffectLocation.Hitpoints] * 3;
        exp += character.Attributes.Attribute[EffectLocation.Mana];
        exp += character.Attributes.Attribute[EffectLocation.Strength];
        exp += character.Attributes.Attribute[EffectLocation.Dexterity];
        exp += character.Attributes.Attribute[EffectLocation.Constitution];
        exp += character.Attributes.Attribute[EffectLocation.Wisdom];
        exp += character.Attributes.Attribute[EffectLocation.Intelligence];
        exp += character.ArmorRating.Magic;
        exp += character.Level * 15;
        //exp += character.Attributes.Attribute[EffectLocation.Moves];
        // boost xp if mob is shielded

        return exp > maxEXP ? maxEXP : exp;
    }

    public static void GainExperiencePoints(this Player player, Player target, bool display)
    {
        var expWorth = GetExpWorth(target);
        var halfPlayerLevel = Math.Ceiling((double)(player.Level / 2m));
        /*
    
        The following only happens If (player level / 2) is Greater than or equal to mob level
        If (player level / 2) + 2 is Greater than or equal to mob level then Exp Worth is divided by 4
        Else Exp Worth is divided by 2
        
        */
        if (halfPlayerLevel >= target.Level)
        {
            if (halfPlayerLevel + 2 >= target.Level)
            {
                expWorth /= 4;
            }
            else
            {
                expWorth /= 2;
            }
        }

        player.Experience += expWorth;
        player.ExperienceToNextLevel -= expWorth;

        Services.Instance.UpdateClient.UpdateExp(player);

        if (!display)
            return;

        Services.Instance.Writer.WriteLine(
            expWorth == 1
                ? "<p class='improve'>You gain 1 measly experience point.</p>"
                : $"<p class='improve'>You receive {expWorth} experience points.</p>",
            player
        );
    }

    public static void GainExperiencePoints(this Player player, int amount, bool display)
    {
        player.Experience += amount;
        player.ExperienceToNextLevel -= amount;

        Services.Instance.UpdateClient.UpdateExp(player);

        if (!display)
            return;

        Services.Instance.Writer.WriteLine(
            amount == 1
                ? "<p class='improve'>You gain 1 measly experience point.</p>"
                : $"<p class='improve'>You receive {amount} experience points.</p>",
            player
        );
    }

    public static void GainLevel(this Player player, bool display)
    {
        player.Level++;
        player.ExperienceToNextLevel = player.Level * 4000; //TODO: have class and race mod

        var hpGain = (player.MaxAttributes.Attribute[EffectLocation.Constitution] / 100m) * 20;
        var minHPGain = (hpGain / 100m) * 20;
        var totalHP = DiceBag.Roll(1, (int)minHPGain, (int)hpGain);

        var manaGain = player.MaxAttributes.Attribute[EffectLocation.Intelligence] / 100m * 20;
        var minManaGain = manaGain / 100m * 20;
        var totalMana = DiceBag.Roll(1, (int)minManaGain, (int)manaGain);

        var moveGain = player.MaxAttributes.Attribute[EffectLocation.Dexterity] / 100m * 20;
        var minMoveGain = manaGain / 100 * 20;
        var totalMove = DiceBag.Roll(1, (int)minMoveGain, (int)moveGain);

        player.MaxAttributes.Attribute[EffectLocation.Hitpoints] += totalHP;
        player.MaxAttributes.Attribute[EffectLocation.Mana] += totalMana;
        player.MaxAttributes.Attribute[EffectLocation.Moves] += totalMove;

        SeedData.Classes.SetGenericTitle(player);
        player.UpdateClientUI();

        if (!display)
            return;

        Services.Instance.Writer.WriteLine(
            $"<p class='improve'>You have advanced to level {player.Level}, you gain: {totalHP} HP, {totalMana} Mana, {totalMove} Moves.</p>",
            player
        );
    }

    /// <summary>
    /// Applies bonus affects to player
    /// </summary>
    /// <param name="direction"></param>
    public static void ApplyAffects(this Player player, Affect affect)
    {
        if (affect.Modifier.Strength != 0)
        {
            player.Attributes.Attribute[EffectLocation.Strength] += affect.Modifier.Strength;
        }

        if (affect.Modifier.Dexterity != 0)
        {
            player.Attributes.Attribute[EffectLocation.Dexterity] += affect.Modifier.Dexterity;
        }

        if (affect.Modifier.Constitution != 0)
        {
            player.Attributes.Attribute[EffectLocation.Constitution] += affect
                .Modifier
                .Constitution;
        }

        if (affect.Modifier.Intelligence != 0)
        {
            player.Attributes.Attribute[EffectLocation.Intelligence] += affect
                .Modifier
                .Intelligence;
        }

        if (affect.Modifier.Wisdom != 0)
        {
            player.Attributes.Attribute[EffectLocation.Wisdom] += affect.Modifier.Wisdom;
        }

        if (affect.Modifier.Charisma != 0)
        {
            player.Attributes.Attribute[EffectLocation.Charisma] += affect.Modifier.Charisma;
        }

        if (affect.Modifier.HitRoll != 0)
        {
            player.Attributes.Attribute[EffectLocation.HitRoll] += affect.Modifier.HitRoll;
        }

        if (affect.Modifier.DamRoll != 0)
        {
            player.Attributes.Attribute[EffectLocation.DamageRoll] += affect.Modifier.DamRoll;
        }

        if (affect.Modifier.Armour != 0)
        {
            player.ArmorRating.Armour += affect.Modifier.Armour;
            player.ArmorRating.Magic += affect.Modifier.Armour;
        }

        if (affect.Affects == DefineSpell.SpellAffect.Blind)
        {
            player.Affects.Blind = true;
        }
        if (affect.Affects == DefineSpell.SpellAffect.Berserk)
        {
            player.Affects.Berserk = true;
        }
        if (affect.Affects == DefineSpell.SpellAffect.NonDetect)
        {
            player.Affects.NonDectect = true;
        }
        if (affect.Affects == DefineSpell.SpellAffect.Invis)
        {
            player.Affects.Invis = true;
        }
        if (affect.Affects == DefineSpell.SpellAffect.DetectInvis)
        {
            player.Affects.DetectInvis = true;
        }
        if (affect.Affects == DefineSpell.SpellAffect.DetectHidden)
        {
            player.Affects.DetectHidden = true;
        }
        if (affect.Affects == DefineSpell.SpellAffect.Poison)
        {
            player.Affects.Poisoned = true;
        }
        if (affect.Affects == DefineSpell.SpellAffect.Haste)
        {
            player.Affects.Haste = true;
        }

        Services.Instance.UpdateClient.UpdateAffects(player);
    }

    public static string UpdateAffect(this Player player, Item.Item item, Affect affect)
    {
        var modBenefits = string.Empty;

        if (item.Modifier.Strength != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.Strength] += item.Modifier.Strength;

            affect.Modifier.Strength = item.Modifier.Strength;
            modBenefits =
                $"modifies STR by {item.Modifier.Strength} for {affect.Duration} minutes<br />";
        }

        if (item.Modifier.Dexterity != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.Dexterity] += item.Modifier.Dexterity;

            affect.Modifier.Dexterity = item.Modifier.Dexterity;
            modBenefits =
                $"modifies DEX by {item.Modifier.Dexterity} for {affect.Duration} minutes<br />";
        }

        if (item.Modifier.Constitution != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.Constitution] += item.Modifier.Constitution;

            affect.Modifier.Constitution = item.Modifier.Constitution;
            modBenefits =
                $"modifies CON by {item.Modifier.Constitution} for {affect.Duration} minutes<br />";
        }

        if (item.Modifier.Intelligence != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.Intelligence] += item.Modifier.Intelligence;
            affect.Modifier.Intelligence = item.Modifier.Intelligence;
            modBenefits =
                $"modifies INT by {item.Modifier.Intelligence} for {affect.Duration} minutes<br />";
        }

        if (item.Modifier.Wisdom != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.Wisdom] += item.Modifier.Wisdom;

            affect.Modifier.Wisdom = item.Modifier.Wisdom;
            modBenefits =
                $"modifies WIS by {item.Modifier.Wisdom} for {affect.Duration} minutes<br />";
        }

        if (item.Modifier.Charisma != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.Charisma] += item.Modifier.Charisma;

            affect.Modifier.Charisma = item.Modifier.Charisma;
            modBenefits =
                $"modifies CHA by {item.Modifier.Charisma} for {affect.Duration} minutes<br />";
        }

        if (item.Modifier.HP != 0)
        {
            player.Attributes.Attribute[EffectLocation.Hitpoints] += item.Modifier.HP;

            if (
                player.Attributes.Attribute[EffectLocation.Hitpoints]
                > player.MaxAttributes.Attribute[EffectLocation.Hitpoints]
            )
            {
                player.Attributes.Attribute[EffectLocation.Hitpoints] = player
                    .MaxAttributes
                    .Attribute[EffectLocation.Hitpoints];
            }
        }

        if (item.Modifier.Mana != 0)
        {
            player.Attributes.Attribute[EffectLocation.Mana] += item.Modifier.Mana;

            if (
                player.Attributes.Attribute[EffectLocation.Mana]
                > player.MaxAttributes.Attribute[EffectLocation.Mana]
            )
            {
                player.Attributes.Attribute[EffectLocation.Mana] = player.MaxAttributes.Attribute[
                    EffectLocation.Mana
                ];
            }
        }

        if (item.Modifier.Moves != 0)
        {
            player.Attributes.Attribute[EffectLocation.Moves] += item.Modifier.Moves;

            if (
                player.Attributes.Attribute[EffectLocation.Moves]
                > player.MaxAttributes.Attribute[EffectLocation.Moves]
            )
            {
                player.Attributes.Attribute[EffectLocation.Moves] = player.MaxAttributes.Attribute[
                    EffectLocation.Moves
                ];
            }
        }

        if (item.Modifier.HitRoll != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.HitRoll] += item.Modifier.HitRoll;
            affect.Modifier.HitRoll = item.Modifier.HitRoll;

            modBenefits =
                $"modifies Hit Roll by {item.Modifier.HitRoll} for {affect.Duration} minutes<br />";
        }

        if (item.Modifier.DamRoll != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.DamageRoll] += item.Modifier.DamRoll;

            affect.Modifier.DamRoll = item.Modifier.DamRoll;
            modBenefits =
                $"modifies Dam Roll by {item.Modifier.DamRoll} for {affect.Duration} minutes<br />";
        }

        Services.Instance.UpdateClient.UpdateAffects(player);

        // saves / saving spell

        return modBenefits;
    }

    public static void HarmTarget(this Player victim, int damage)
    {
        victim.Attributes.Attribute[EffectLocation.Hitpoints] -= damage;

        if (victim.Attributes.Attribute[EffectLocation.Hitpoints] < 0)
        {
            victim.Attributes.Attribute[EffectLocation.Hitpoints] = 0;
        }

        if (
            victim.Config.Wimpy > 0
            && victim.Attributes.Attribute[EffectLocation.Hitpoints] <= victim.Config.Wimpy
        )
        {
            victim.Buffer.Clear();
            victim.Buffer.Enqueue("flee");
        }
    }

    public static bool IsAlive(this Player victim)
    {
        return victim.Attributes.Attribute[EffectLocation.Hitpoints] > 0;
    }

    public static Item.Item FindObjectInInventory(this Player player, Tuple<int, string> keyword)
    {
        if (keyword.Item2.Equals("book"))
        {
            return keyword.Item1 == -1
                ? player.Inventory.FirstOrDefault(x => x.ItemType == Item.Item.ItemTypes.Book)
                : player.Inventory
                    .FindAll(x => x.ItemType == Item.Item.ItemTypes.Book)
                    .Skip(keyword.Item1 - 1)
                    .FirstOrDefault();
        }

        return keyword.Item1 == -1
            ? player.Inventory.FirstOrDefault(
                x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase)
            )
            : player.Inventory
                .FindAll(
                    x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase)
                )
                .Skip(keyword.Item1 - 1)
                .FirstOrDefault();
    }

    /// <summary>
    /// Her / His
    /// </summary>
    /// <param name="gender"></param>
    /// <returns></returns>
    public static string ReturnPronoun(this Character character)
    {
        return character.Gender switch
        {
            "Female" => "her",
            "Male" => "his",
            _ => "their",
        };
    }

    /// <summary>
    /// She / He
    /// </summary>
    /// <param name="gender"></param>
    /// <returns></returns>
    public static string ReturnSubjectPronoun(this Character character)
    {
        return character.Gender switch
        {
            "Female" => "she",
            "Male" => "he",
            _ => "it",
        };
    }

    /// <summary>
    /// Her / Him
    /// </summary>
    /// <param name="gender"></param>
    /// <returns></returns>
    public static string ReturnObjectPronoun(this Character character)
    {
        return character.Gender switch
        {
            "Female" => "her",
            "Male" => "him",
            _ => "it",
        };
    }

    public static bool IsCaster(this Character character)
    {
        return character.ClassName switch
        {
            "Mage" => true,
            "Cleric" => true,
            "Druid" => true,
            _ => false,
        };
    }

    public static bool CanSee(this Character character, Room room)
    {
        if (character.Affects.Blind)
            return false;

        if (room.IsLit)
            return true;

        if (character.Affects.DarkVision)
            return true;

        if (character.Equipped.Light != null)
            return true;

        foreach (var pc in room.Players)
        {
            if (pc.Equipped.Light != null)
                return true;
        }

        if (room.Type is Room.RoomType.Underground or Room.RoomType.Inside)
            return false;

        return !Services.Instance.Time.IsNightTime();
    }

    public static void UpdateClientUI(this Player player)
    {
        //update UI
        Services.Instance.UpdateClient.UpdateHP(player);
        Services.Instance.UpdateClient.UpdateMana(player);
        Services.Instance.UpdateClient.UpdateMoves(player);
        Services.Instance.UpdateClient.UpdateScore(player);
    }

    public static void DeathCry(this Player target, Room room)
    {
        Services.Instance.Writer.WriteToOthersInRoom(
            $"<p class='combat'>Your blood freezes as you hear {target.Name.ToLower()}'s death cry.</p>",
            room,
            target
        );

        // Exit checks
        var rooms = new List<Room>();

        if (room.Exits.NorthWest != null)
        {
            rooms.Add(
                Services.Instance.Cache.GetRoom(
                    $"{room.Exits.NorthWest.AreaId}{room.Exits.NorthWest.Coords.X}{room.Exits.NorthWest.Coords.Y}{room.Exits.NorthWest.Coords.Z}"
                )
            );
        }

        if (room.Exits.North != null)
        {
            rooms.Add(
                Services.Instance.Cache.GetRoom(
                    $"{room.Exits.North.AreaId}{room.Exits.North.Coords.X}{room.Exits.North.Coords.Y}{room.Exits.North.Coords.Z}"
                )
            );
        }

        if (room.Exits.NorthEast != null)
        {
            rooms.Add(
                Services.Instance.Cache.GetRoom(
                    $"{room.Exits.NorthEast.AreaId}{room.Exits.NorthEast.Coords.X}{room.Exits.NorthEast.Coords.Y}{room.Exits.NorthEast.Coords.Z}"
                )
            );
        }

        if (room.Exits.East != null)
        {
            rooms.Add(
                Services.Instance.Cache.GetRoom(
                    $"{room.Exits.East.AreaId}{room.Exits.East.Coords.X}{room.Exits.East.Coords.Y}{room.Exits.East.Coords.Z}"
                )
            );
        }

        if (room.Exits.SouthEast != null)
        {
            rooms.Add(
                Services.Instance.Cache.GetRoom(
                    $"{room.Exits.SouthEast.AreaId}{room.Exits.SouthEast.Coords.X}{room.Exits.SouthEast.Coords.Y}{room.Exits.SouthEast.Coords.Z}"
                )
            );
        }

        if (room.Exits.South != null)
        {
            rooms.Add(
                Services.Instance.Cache.GetRoom(
                    $"{room.Exits.South.AreaId}{room.Exits.South.Coords.X}{room.Exits.South.Coords.Y}{room.Exits.South.Coords.Z}"
                )
            );
        }

        if (room.Exits.SouthWest != null)
        {
            rooms.Add(
                Services.Instance.Cache.GetRoom(
                    $"{room.Exits.SouthWest.AreaId}{room.Exits.SouthWest.Coords.X}{room.Exits.SouthWest.Coords.Y}{room.Exits.SouthWest.Coords.Z}"
                )
            );
        }

        if (room.Exits.West != null)
        {
            rooms.Add(
                Services.Instance.Cache.GetRoom(
                    $"{room.Exits.West.AreaId}{room.Exits.West.Coords.X}{room.Exits.West.Coords.Y}{room.Exits.West.Coords.Z}"
                )
            );
        }

        if (room.Exits.Up != null)
        {
            rooms.Add(
                Services.Instance.Cache.GetRoom(
                    $"{room.Exits.Up.AreaId}{room.Exits.Up.Coords.X}{room.Exits.Up.Coords.Y}{room.Exits.Up.Coords.Z}"
                )
            );
        }

        if (room.Exits.Down != null)
        {
            rooms.Add(
                Services.Instance.Cache.GetRoom(
                    $"{room.Exits.Down.AreaId}{room.Exits.Down.Coords.X}{room.Exits.Down.Coords.Y}{room.Exits.Down.Coords.Z}"
                )
            );
        }

        foreach (var pc in rooms.SelectMany(adjacentRoom => adjacentRoom.Players))
        {
            Services.Instance.Writer.WriteLine(
                $"<p>Your blood freezes as you hear someone's death cry.</p>",
                pc
            );
        }
    }

    public static void AddToCombat(this Player character)
    {
        if (!Services.Instance.Cache.IsCharInCombat(character.Id.ToString()))
        {
            Services.Instance.Cache.AddCharToCombat(character.Id.ToString(), character);
        }
    }

    public static void DisplayEquipmentUI(this Player player)
    {
        var displayEquipment = new StringBuilder();

        try
        {
            displayEquipment
                .Append("<p>You are using:</p>")
                .Append("<table>")
                .Append("<tr><td style='width:175px;' title='Worn as light'>")
                .Append("&lt;used as light&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Light?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td title='Worn on finger'>")
                .Append(" &lt;worn on finger&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Finger?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Worn on finger'>")
                .Append(" &lt;worn on finger&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Finger2?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Worn around neck'>")
                .Append(" &lt;worn around neck&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Neck?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td title='Worn around neck'>")
                .Append(" &lt;worn around neck&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Neck2?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Worn on face'>")
                .Append(" &lt;worn on face&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Face?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Worn on head'>")
                .Append(" &lt;worn on head&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Head?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Worn on torso'>")
                .Append(" &lt;worn on torso&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Torso?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Worn on legs'>")
                .Append(" &lt;worn on legs&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Legs?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Worn on feet'>")
                .Append(" &lt;worn on feet&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Feet?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Worn on hands'>")
                .Append(" &lt;worn on hands&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Hands?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Worn on arms'>")
                .Append(" &lt;worn on arms&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Arms?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Worn about body'>")
                .Append(" &lt;worn about body&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.AboutBody?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Worn on waist'>")
                .Append(" &lt;worn about waist&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Waist?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Worn on wrist'>")
                .Append(" &lt;worn around wrist&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Wrist?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Worn on wrist'>")
                .Append(" &lt;worn around wrist&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Wrist2?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='worn as weapon'>")
                .Append(" &lt;wielded&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Wielded?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='worn as weapon'>")
                .Append(" &lt;secondary&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Secondary?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Worn as shield'>")
                .Append(" &lt;worn as shield&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Shield?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Held'>")
                .Append(" &lt;Held&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Held?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("<tr><td  title='Floating Nearby'>")
                .Append(" &lt;Floating nearby&gt;")
                .Append("</td>")
                .Append("<td>")
                .Append(player.Equipped.Floating?.ReturnWithFlags() ?? "(nothing)")
                .Append("</td></tr>")
                .Append("</table");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Equip.cs: " + ex);
        }

        Services.Instance.Writer.WriteLine(displayEquipment.ToString(), player);
    }

    public static void WearAll(this Player player, Room room)
    {
        var itemsToWear = player.Inventory.Where(x => x.Equipped == false);

        foreach (var itemToWear in itemsToWear)
        {
            if (!player.EqSlotSet(itemToWear.Slot))
            {
                player.Wear(itemToWear.Name, room);
            }
        }
    }

    public static void Remove(this Player player, string item, Room room)
    {
        if (item.Equals("all", StringComparison.CurrentCultureIgnoreCase))
        {
            player.RemoveAll(room);
            return;
        }

        var itemToRemove = player.Inventory.FirstOrDefault(
            x => x.Name.Contains(item, StringComparison.CurrentCultureIgnoreCase) && x.Equipped
        );

        if (itemToRemove == null)
        {
            Services.Instance.Writer.WriteLine("<p>You are not wearing that item.</p>", player);
            return;
        }

        if ((itemToRemove.ItemFlag & Item.Item.ItemFlags.Noremove) != 0)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You can't remove {itemToRemove.Name}. It appears to be cursed.</p>",
                player
            );
            return;
        }

        itemToRemove.Equipped = false;
        player.ArmorRating.Armour -= itemToRemove.ArmourRating.Armour + itemToRemove.Modifier.AcMod;
        player.ArmorRating.Magic -=
            itemToRemove.ArmourRating.Magic + itemToRemove.Modifier.AcMagicMod;
        player.Attributes.Attribute[EffectLocation.Strength] -= itemToRemove.Modifier.Strength;
        player.Attributes.Attribute[EffectLocation.Dexterity] -= itemToRemove.Modifier.Dexterity;
        player.Attributes.Attribute[EffectLocation.Constitution] -= itemToRemove
            .Modifier
            .Constitution;
        player.Attributes.Attribute[EffectLocation.Wisdom] -= itemToRemove.Modifier.Wisdom;
        player.Attributes.Attribute[EffectLocation.Intelligence] -= itemToRemove
            .Modifier
            .Intelligence;
        player.Attributes.Attribute[EffectLocation.Charisma] -= itemToRemove.Modifier.Charisma;

        player.Attributes.Attribute[EffectLocation.Hitpoints] -= itemToRemove.Modifier.HP;
        player.Attributes.Attribute[EffectLocation.Mana] -= itemToRemove.Modifier.Mana;
        player.Attributes.Attribute[EffectLocation.Moves] -= itemToRemove.Modifier.Moves;
        player.MaxAttributes.Attribute[EffectLocation.Hitpoints] -= itemToRemove.Modifier.HP;
        player.MaxAttributes.Attribute[EffectLocation.Mana] -= itemToRemove.Modifier.Mana;
        player.MaxAttributes.Attribute[EffectLocation.Moves] -= itemToRemove.Modifier.Moves;

        player.Attributes.Attribute[EffectLocation.DamageRoll] -= itemToRemove.Modifier.DamRoll;
        player.Attributes.Attribute[EffectLocation.HitRoll] -= itemToRemove.Modifier.HitRoll;
        switch (itemToRemove.Slot)
        {
            case EquipmentSlot.Arms:
                player.Equipped.Arms = null;
                break;
            case EquipmentSlot.Body:
                player.Equipped.AboutBody = null;
                break;
            case EquipmentSlot.Face:
                player.Equipped.Face = null;
                break;
            case EquipmentSlot.Feet:
                player.Equipped.Feet = null;
                break;
            case EquipmentSlot.Finger:
                player.Equipped.Finger = null; // TODO: slot 2
                break;
            case EquipmentSlot.Floating:
                player.Equipped.Floating = null;
                break;
            case EquipmentSlot.Hands:
                player.Equipped.Hands = null;
                break;
            case EquipmentSlot.Head:
                player.Equipped.Head = null;
                break;
            case EquipmentSlot.Held:
                player.Equipped.Held = null; // TODO: handle when wield and shield or 2hand item are equipped
                break;
            case EquipmentSlot.Legs:
                player.Equipped.Legs = null;
                break;
            case EquipmentSlot.Light:
                player.Equipped.Light = null;
                break;
            case EquipmentSlot.Neck:
                player.Equipped.Neck = null; // TODO: slot 2
                break;
            case EquipmentSlot.Shield:
                player.Equipped.Shield = null;
                break;
            case EquipmentSlot.Torso:
                player.Equipped.Torso = null;
                break;
            case EquipmentSlot.Waist:
                player.Equipped.Waist = null;
                break;
            case EquipmentSlot.Wielded:

                if (
                    player.Equipped.Secondary != null
                    && itemToRemove.Name.Equals(player.Equipped.Secondary.Name)
                )
                {
                    player.Equipped.Secondary = null;
                }
                else
                {
                    player.Equipped.Wielded = null;
                }
                break;
            case EquipmentSlot.Wrist:
                player.Equipped.Wrist = null; // TODO: slot 2
                break;
            case EquipmentSlot.Secondary:
                player.Equipped.Secondary = null;
                break;
            default:
                itemToRemove.Equipped = false;
                Services.Instance.Writer.WriteLine(
                    "<p>You don't know how to remove this.</p>",
                    player
                );
                return;
        }

        Services.Instance.Writer.WriteLine(
            $"<p>You stop using {itemToRemove.Name.ToLower()}.</p>",
            player
        );

        Services.Instance.Writer.WriteToOthersInRoom(
            $"<p>{player.Name} stops using {itemToRemove.Name.ToLower()}.</p>",
            room,
            player
        );

        player.UpdateClientUI();
        Services.Instance.UpdateClient.UpdateEquipment(player);
        Services.Instance.UpdateClient.UpdateInventory(player);
    }

    public static void RemoveAll(this Player player, Room room)
    {
        var itemsToRemove = player.Inventory.Where(x => x.Equipped == true);

        foreach (var itemToRemove in itemsToRemove)
        {
            player.Remove(itemToRemove.Name, room);
        }
    }

    // handle secondary equip
    public static void Wear(this Player player, string item, Room room, string type = "")
    {
        if (item.Equals("all", StringComparison.CurrentCultureIgnoreCase))
        {
            player.WearAll(room);
            return;
        }

        var itemToWear = player.Inventory.FirstOrDefault(
            x =>
                x.Name.Contains(item, StringComparison.CurrentCultureIgnoreCase)
                && x.Equipped == false
        );

        if (itemToWear == null)
        {
            Services.Instance.Writer.WriteLine("<p>You don't have that item.</p>", player);
            return;
        }

        var itemSlot = itemToWear.Slot;

        if (
            itemToWear.ItemType != Item.Item.ItemTypes.Armour
            && itemToWear.ItemType != Item.Item.ItemTypes.Weapon
            && itemToWear.ItemType != Item.Item.ItemTypes.Light
        )
        {
            itemSlot = EquipmentSlot.Held;
        }

        if (type == "dual")
        {
            itemSlot = EquipmentSlot.Secondary;
        }

        itemToWear.Equipped = true;
        player.ArmorRating.Armour += itemToWear.ArmourRating.Armour + itemToWear.Modifier.AcMod;
        player.ArmorRating.Magic += itemToWear.ArmourRating.Magic + itemToWear.Modifier.AcMagicMod;
        player.Attributes.Attribute[EffectLocation.Strength] += itemToWear.Modifier.Strength;
        player.Attributes.Attribute[EffectLocation.Dexterity] += itemToWear.Modifier.Dexterity;
        player.Attributes.Attribute[EffectLocation.Constitution] += itemToWear
            .Modifier
            .Constitution;
        player.Attributes.Attribute[EffectLocation.Wisdom] += itemToWear.Modifier.Wisdom;
        player.Attributes.Attribute[EffectLocation.Intelligence] += itemToWear
            .Modifier
            .Intelligence;
        player.Attributes.Attribute[EffectLocation.Charisma] += itemToWear.Modifier.Charisma;

        player.Attributes.Attribute[EffectLocation.Hitpoints] += itemToWear.Modifier.HP;
        player.Attributes.Attribute[EffectLocation.Mana] += itemToWear.Modifier.Mana;
        player.Attributes.Attribute[EffectLocation.Moves] += itemToWear.Modifier.Moves;
        player.MaxAttributes.Attribute[EffectLocation.Hitpoints] += itemToWear.Modifier.HP;
        player.MaxAttributes.Attribute[EffectLocation.Mana] += itemToWear.Modifier.Mana;
        player.MaxAttributes.Attribute[EffectLocation.Moves] += itemToWear.Modifier.Moves;

        player.Attributes.Attribute[EffectLocation.DamageRoll] += itemToWear.Modifier.DamRoll;
        player.Attributes.Attribute[EffectLocation.HitRoll] += itemToWear.Modifier.HitRoll;
        // player.Attributes.Attribute[EffectLocation.DamageRoll] += itemToWear.Modifier.SpellDam; // spell dam no exist
        // player.Attributes.Attribute[EffectLocation.SavingSpell] += itemToWear.Modifier.Saves; not implemented
        switch (itemSlot)
        {
            case EquipmentSlot.Arms:

                if (player.Equipped.Arms != null)
                {
                    player.Remove(player.Equipped.Arms.Name, room);
                }

                player.Equipped.Arms = itemToWear;
                break;
            case EquipmentSlot.Body:
                if (player.Equipped.AboutBody != null)
                {
                    player.Remove(player.Equipped.AboutBody.Name, room);
                }
                player.Equipped.AboutBody = itemToWear;
                break;
            case EquipmentSlot.Face:

                if (player.Equipped.Face != null)
                {
                    player.Remove(player.Equipped.Face.Name, room);
                }

                player.Equipped.Face = itemToWear;
                break;
            case EquipmentSlot.Feet:

                if (player.Equipped.Feet != null)
                {
                    player.Remove(player.Equipped.Feet.Name, room);
                }

                player.Equipped.Feet = itemToWear;
                break;
            case EquipmentSlot.Finger:

                if (player.Equipped.Finger != null)
                {
                    player.Remove(player.Equipped.Finger.Name, room);
                }

                player.Equipped.Finger = itemToWear; // TODO: slot 2
                break;
            case EquipmentSlot.Floating:

                if (player.Equipped.Floating != null)
                {
                    player.Remove(player.Equipped.Floating.Name, room);
                }

                player.Equipped.Floating = itemToWear;
                break;
            case EquipmentSlot.Hands:

                if (player.Equipped.Hands != null)
                {
                    player.Remove(player.Equipped.Hands.Name, room);
                }

                player.Equipped.Hands = itemToWear;
                break;
            case EquipmentSlot.Head:

                if (player.Equipped.Head != null)
                {
                    player.Remove(player.Equipped.Head.Name, room);
                }

                player.Equipped.Head = itemToWear;
                break;
            case EquipmentSlot.Held:
                if (player.Equipped.Held != null)
                {
                    player.Remove(player.Equipped.Held.Name, room);
                }

                player.Equipped.Held = itemToWear; // TODO: handle when wield and shield or 2hand item are equipped
                break;
            case EquipmentSlot.Legs:

                if (player.Equipped.Legs != null)
                {
                    player.Remove(player.Equipped.Legs.Name, room);
                }
                player.Equipped.Legs = itemToWear;
                break;
            case EquipmentSlot.Light:

                if (player.Equipped.Light != null)
                {
                    player.Remove(player.Equipped.Light.Name, room);
                }

                player.Equipped.Light = itemToWear;
                break;
            case EquipmentSlot.Neck:

                if (player.Equipped.Neck != null)
                {
                    player.Remove(player.Equipped.Neck.Name, room);
                }

                player.Equipped.Neck = itemToWear; // TODO: slot 2
                break;
            case EquipmentSlot.Shield:

                if (player.Equipped.Wielded != null && player.Equipped.Wielded.TwoHanded)
                {
                    Services.Instance.Writer.WriteLine(
                        "Your hands are tied up with your two-handed weapon!",
                        player
                    );
                    return;
                }

                if (player.Equipped.Shield != null)
                {
                    player.Remove(player.Equipped.Shield.Name, room);
                }

                if (player.Equipped.Secondary != null)
                {
                    player.Remove(player.Equipped.Secondary.Name, room);
                }

                player.Equipped.Shield = itemToWear;
                break;
            case EquipmentSlot.Torso:

                if (player.Equipped.Torso != null)
                {
                    player.Remove(player.Equipped.Torso.Name, room);
                }

                player.Equipped.Torso = itemToWear;
                break;
            case EquipmentSlot.Waist:

                if (player.Equipped.Waist != null)
                {
                    player.Remove(player.Equipped.Waist.Name, room);
                }

                player.Equipped.Waist = itemToWear;
                break;
            case EquipmentSlot.Wielded:

                if (itemToWear.TwoHanded && player.Equipped.Shield != null)
                {
                    Services.Instance.Writer.WriteLine(
                        "You need two hands free for that weapon, remove your shield and try again.",
                        player
                    );

                    return;
                }

                if (player.Equipped.Wielded != null)
                {
                    player.Remove(player.Equipped.Wielded.Name, room);
                }

                player.Equipped.Wielded = itemToWear;
                break;
            case EquipmentSlot.Wrist:

                if (player.Equipped.Wrist != null)
                {
                    player.Remove(player.Equipped.Wrist.Name, room);
                }

                player.Equipped.Wrist = itemToWear; // TODO: slot 2
                break;
            case EquipmentSlot.Secondary:

                if (player.Equipped.Secondary != null)
                {
                    player.Remove(player.Equipped.Secondary.Name, room);
                }

                player.Equipped.Secondary = itemToWear; // TODO: slot 2
                break;
            default:
                itemToWear.Equipped = false;
                Services.Instance.Writer.WriteLine(
                    "<p>You don't know how to wear this.</p>",
                    player
                );
                return;
        }

        Services.Instance.Writer.WriteLine(
            $"<p>You wield {itemToWear.Name.ToLower()} as your second weapon.</p>",
            player
        );
        Services.Instance.Writer.WriteToOthersInRoom(
            $"{itemToWear.Name.ToLower()} as {player.ReturnPronoun()} secondary weapon.",
            room,
            player
        );

        player.UpdateClientUI();
        Services.Instance.UpdateClient.UpdateEquipment(player);
        Services.Instance.UpdateClient.UpdateInventory(player);
    }

    public static bool EqSlotSet(this Player player, EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Arms:
                return player.Equipped.Arms != null;
            case EquipmentSlot.Body:
                return player.Equipped.AboutBody != null;
            case EquipmentSlot.Face:
                return player.Equipped.Face != null;
            case EquipmentSlot.Feet:
                return player.Equipped.Feet != null;
            case EquipmentSlot.Finger:
                return player.Equipped.Finger != null;
            case EquipmentSlot.Floating:
                return player.Equipped.Floating != null;
            case EquipmentSlot.Hands:
                return player.Equipped.Hands != null;
            case EquipmentSlot.Head:
                return player.Equipped.Head != null;
            case EquipmentSlot.Held:
                return player.Equipped.Held != null;
            case EquipmentSlot.Legs:
                return player.Equipped.Legs != null;
            case EquipmentSlot.Light:
                return player.Equipped.Light != null;
            case EquipmentSlot.Neck:
                return player.Equipped.Neck != null;
            case EquipmentSlot.Shield:
                return player.Equipped.Shield != null;
            case EquipmentSlot.Torso:
                return player.Equipped.Torso != null;
            case EquipmentSlot.Waist:
                return player.Equipped.Waist != null;
            case EquipmentSlot.Wielded:
                return player.Equipped.Wielded != null;
            case EquipmentSlot.Wrist:
                return player.Equipped.Wrist != null;
            case EquipmentSlot.Secondary:
                return player.Equipped.Secondary != null;
            default:
                return false;
        }
    }

    /// <summary>
    /// Used to Change Player room
    /// </summary>
    /// <param name="player"></param>
    /// <param name="oldRoom"></param>
    /// <param name="newRoom"></param>
    /// <param name="isFlee"></param>
    public static void ChangeRoom(this Player player, Room oldRoom, Room newRoom, bool isFlee)
    {
        player.Pose = string.Empty;

        player.ExitMessage(oldRoom, newRoom, isFlee);

        player.UpdateLocation(oldRoom, newRoom);

        player.EnterMessage(newRoom, oldRoom, isFlee);

        Services.Instance.UpdateClient.GetMap(
            player,
            Services.Instance.Cache.GetMap($"{newRoom.AreaId}{newRoom.Coords.Z}")
        );
        Services.Instance.UpdateClient.UpdateMoves(player);
        player.Buffer.Enqueue("look");
    }

    /// <summary>
    /// Updates the characters location
    /// </summary>
    /// <param name="character"></param>
    /// <param name="oldRoom"></param>
    /// <param name="newRoom"></param>
    private static void UpdateLocation(this Player character, Room oldRoom, Room newRoom)
    {
        if (character.ConnectionId != "mob")
        {
            // remove player from room
            oldRoom.Players.Remove(character);

            //add player to room
            character.RoomId =
                $"{newRoom.AreaId}{newRoom.Coords.X}{newRoom.Coords.Y}{newRoom.Coords.Z}";
            newRoom.Players.Add(character);

            //player entered new area TODO: Add area announce
            //if(oldRoom.AreaId != newRoom.AreaId)
            //    _areaActions.AreaEntered(player, newRoom);
        }
        else
        {
            // remove mob from room
            oldRoom.Mobs.Remove(character);

            //add mob to room
            character.RoomId =
                $"{newRoom.AreaId}{newRoom.Coords.X}{newRoom.Coords.Y}{newRoom.Coords.Z}";
            newRoom.Mobs.Add(character);
        }
    }

    private static void EnterMessage(this Player character, Room toRoom, Room fromRoom, bool isFlee)
    {
        var direction = "from nowhere";
        var movement = "appears";

        if (toRoom.Exits != null)
        {
            if (toRoom.Exits.Down?.RoomId == fromRoom.Id)
                direction = "in from below";
            if (toRoom.Exits.Up?.RoomId == fromRoom.Id)
                direction = "in from above";
            if (toRoom.Exits.North?.RoomId == fromRoom.Id)
                direction = "in from the north";
            if (toRoom.Exits.South?.RoomId == fromRoom.Id)
                direction = "in from the south";
            if (toRoom.Exits.East?.RoomId == fromRoom.Id)
                direction = "in from the east";
            if (toRoom.Exits.West?.RoomId == fromRoom.Id)
                direction = "in from the west";
            if (toRoom.Exits.NorthEast?.RoomId == fromRoom.Id)
                direction = "in from the northeast";
            if (toRoom.Exits.NorthWest?.RoomId == fromRoom.Id)
                direction = "in from the northwest";
            if (toRoom.Exits.SouthEast?.RoomId == fromRoom.Id)
                direction = "in from the southeast";
            if (toRoom.Exits.SouthWest?.RoomId == fromRoom.Id)
                direction = "in from the southwest";
        }

        switch (character.Status)
        {
            case CharacterStatus.Status.Floating:
                movement = "floats";
                break;
            case CharacterStatus.Status.Mounted:
                movement = "rides";
                break;
            case CharacterStatus.Status.Standing:
                Services.Instance.UpdateClient.PlaySound("walk", character);
                movement = "walks";
                break;
        }

        if (isFlee)
        {
            Services.Instance.UpdateClient.PlaySound("flee", character);
            movement = "rushes";
        }

        foreach (var p in toRoom.Players.Where(p => character.Name != p.Name))
        {
            Services.Instance.Writer.WriteLine(
                $"<span class='{(character.ConnectionId != "mob" ? "player" : "mob")}'>{character.Name} {movement} {direction}.</span>",
                p
            );
        }
    }

    private static void ExitMessage(
        this Player characterBase,
        Room fromRoom,
        Room toRoom,
        bool isFlee
    )
    {
        var direction = "to thin air";
        var movement = "vanishes";

        if (fromRoom.Exits != null)
        {
            if (fromRoom.Exits.Down?.RoomId == toRoom.Id)
                direction = "down";
            if (fromRoom.Exits.Up?.RoomId == toRoom.Id)
                direction = "up";
            if (fromRoom.Exits.North?.RoomId == toRoom.Id)
                direction = "to the north";
            if (fromRoom.Exits.South?.RoomId == toRoom.Id)
                direction = "to the south";
            if (fromRoom.Exits.East?.RoomId == toRoom.Id)
                direction = "to the east";
            if (fromRoom.Exits.West?.RoomId == toRoom.Id)
                direction = "to the west";
            if (fromRoom.Exits.NorthEast?.RoomId == toRoom.Id)
                direction = "to the northeast";
            if (fromRoom.Exits.NorthWest?.RoomId == toRoom.Id)
                direction = "to the northwest";
            if (fromRoom.Exits.SouthEast?.RoomId == toRoom.Id)
                direction = "to the southeast";
            if (fromRoom.Exits.SouthWest?.RoomId == toRoom.Id)
                direction = "to the southwest";
        }

        switch (characterBase.Status)
        {
            case CharacterStatus.Status.Floating:
                movement = "floats";
                break;
            case CharacterStatus.Status.Mounted:
                movement = "rides";
                break;
            case CharacterStatus.Status.Standing:
                movement = "walks";
                break;
        }

        if (isFlee)
        {
            movement = "flee";
        }

        foreach (var p in fromRoom.Players.Where(p => characterBase.Name != p.Name))
        {
            Services.Instance.Writer.WriteLine(
                $"<span class='{(characterBase.ConnectionId != "mob" ? "player" : "mob")}'>{characterBase.Name} {movement} {direction}.</span>",
                p
            );
        }
    }
}
