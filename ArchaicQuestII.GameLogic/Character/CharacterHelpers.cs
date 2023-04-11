using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Model;
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
        var skill = player.Skills.FirstOrDefault(x => x.Name == skillName);

        if (skill != null)
            return true;

        return false;
    }

    public static bool RollSkill(
        this Player player,
        SkillName skillName,
        bool display,
        string customErrorText = "",
        int modifier = 0
    )
    {
        var roll = DiceBag.Roll(1, 1, 100);
        var skill = player.Skills.FirstOrDefault(x => x.Name == skillName);
        var success = skill.Proficiency > roll + modifier;

        if (roll == 1 && display)
        {
            var errorText = skill.IsSpell
                ? $"<p>You tried to cast {skill.Name} but failed miserably.</p>"
                : $"<p>You tried to {skill.Name} but failed miserably.</p>";

            Services.Instance.Writer.WriteLine(errorText, player.ConnectionId);
            return false;
        }

        if (!success && display)
        {
            var failedSkillMessage = customErrorText ?? $"<p>You try to {skill.Name} but fail.</p>";

            var errorText = skill.IsSpell ? $"<p>You lost concentration.</p>" : failedSkillMessage;

            Services.Instance.Writer.WriteLine(errorText, player.ConnectionId);
        }

        return success;
    }

    public static void FailedSkill(this Player player, SkillName name, bool display)
    {
        var skill = player.Skills.FirstOrDefault(x => x.Name == name);

        var increase = DiceBag.Roll(1, 1, 5);

        if (skill == null)
        {
            Services.Instance.Writer.WriteLine("Skill not found");
            return;
        }

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
                + $"<p class='improve'>Your knowledge of {skill.Name} increases by {increase}%.</p>"
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
                : $"<p class='improve'>You receive {expWorth} experience points.</p>"
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
                : $"<p class='improve'>You receive {amount} experience points.</p>"
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
            $"<p class='improve'>You have advanced to level {player.Level}, you gain: {totalHP} HP, {totalMana} Mana, {totalMove} Moves.</p>"
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
                pc.ConnectionId
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
}
