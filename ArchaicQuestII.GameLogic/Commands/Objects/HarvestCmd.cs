using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class HarvestCmd : ICommand
{
    public HarvestCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"harvest", "forage"};
        Description = "You try to harvest something";
        Usages = new[] {"Type: harvest ore", "Example: forage bushes"};
        UserRole = UserRole.Player;
        Writer = writeToClient;
        Cache = cache;
        UpdateClient = updateClient;
        RoomActions = roomActions;
        _dice = new Dice();
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public UserRole UserRole { get; }
    public IWriteToClient Writer { get; }
    public ICache Cache { get; }
    public IUpdateClientUI UpdateClient { get; }
    public IRoomActions RoomActions { get; }

    private IDice _dice { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            Writer.WriteLine("Harvest what?", player.ConnectionId);
            return;
        }
        
        if (player.Status == CharacterStatus.Status.Busy)
        {
            Writer.WriteLine("You are already doing it.", player.ConnectionId);
            return;
        }
        
        var thingToHarvest =
            room.Items.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.OrdinalIgnoreCase));

        if (thingToHarvest == null)
        {
            Writer.WriteLine($"You don't see that here.", player.ConnectionId);
            return;
        }

        if (!thingToHarvest.Container.Items.Any())
        {
            Writer.WriteLine("There's nothing left to harvest.", player.ConnectionId);
            return;
        }

        Harvest(player, room, thingToHarvest);
    }

    private async void Harvest(Player player, Room room, Item.Item thingToHarvest)
    {
        player.Status = CharacterStatus.Status.Busy;

        UpdateClient.PlaySound("foraging", player);

        Writer.WriteLine($"You begin harvesting from {thingToHarvest.Name}.", player.ConnectionId);
        
        await Task.Delay(4000);
        
        if (!room.Players.Contains(player) || player.Status != CharacterStatus.Status.Busy)
        {
            return;
        }

        UpdateClient.PlaySound("foraging", player);

        Writer.WriteLine("You rummage through the foliage looking for something to harvest",
            player.ConnectionId);
        
        await Task.Delay(4000);
        
        if (!room.Players.Contains(player) || player.Status != CharacterStatus.Status.Busy)
        {
            return;
        }

        UpdateClient.PlaySound("foraging", player);

        Writer.WriteLine("You continue searching.", player.ConnectionId);

        await Task.Delay(4000);
        
        if (!room.Players.Contains(player) || player.Status != CharacterStatus.Status.Busy)
        {
            return;
        }
        
        var roll = _dice.Roll(1, 1, 10);

        var randomMobObj = roll switch
        {
            1 => new
            {
                Name = "A snake",
                Description = "A black and red snake",
                LongName = "A black snake slithers along here.",
                DefaultAttack = "bite"
            },
            2 => new
            {
                Name = "A giant hornet",
                Description = "An red and orange hornet",
                LongName = "An angry red and orange hornet buzzes around here.",
                DefaultAttack = "sting"
            },
            3 => new
            {
                Name = "An angry fairy",
                Description = "A short winged fairy, angry at being disturbed",
                LongName = "An angry fairy, annoyed at being disturbed is here.",
                DefaultAttack = "punch"
            },
            _ => new
            {
                Name = "A racoon", Description = "A large grey racoon dog",
                LongName = "A large racoon looks at you angrily", DefaultAttack = "bite"
            }
        };


        var randomMob = new Player
        {
            Name = randomMobObj.Name,
            ClassName = "Fighter",
            Target = String.Empty,
            Status = CharacterStatus.Status.Standing,
            Race = "Other",
            Level = roll + 2,
            RoomId = player.RoomId,
            Attributes = new Attributes()
            {
                Attribute = new Dictionary<EffectLocation, int>
                {
                    { EffectLocation.Mana, 30 },
                    { EffectLocation.Hitpoints, 300 },
                    { EffectLocation.Moves, 30 },
                    { EffectLocation.Strength, 30 },
                    { EffectLocation.Dexterity, 30 },
                    { EffectLocation.Constitution, 30 },
                    { EffectLocation.Intelligence, 30 },
                    { EffectLocation.Wisdom, 30 },
                    { EffectLocation.Charisma, 30 },
                    { EffectLocation.HitRoll, 2 },
                    { EffectLocation.DamageRoll, 1 },
                    { EffectLocation.SavingSpell, 1 },
                }
            },
            MaxAttributes = new Attributes
            {
                Attribute = new Dictionary<EffectLocation, int>
                {
                    { EffectLocation.Mana, 30 },
                    { EffectLocation.Hitpoints, 300 },
                    { EffectLocation.Moves, 30 },
                    { EffectLocation.Strength, 30 },
                    { EffectLocation.Dexterity, 30 },
                    { EffectLocation.Constitution, 30 },
                    { EffectLocation.Intelligence, 30 },
                    { EffectLocation.Wisdom, 30 },
                    { EffectLocation.Charisma, 30 },
                    { EffectLocation.HitRoll, 2 },
                    { EffectLocation.DamageRoll, 1 },
                    { EffectLocation.SavingSpell, 1 },
                }
            },

            Description = randomMobObj.Description,
            LongName = randomMobObj.LongName,
            ArmorRating = new ArmourRating
            {
                Armour = 5,
                Magic = 5
            },
            DefaultAttack = randomMobObj.DefaultAttack,
            UniqueId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };
        
        if (roll <= 1)
        {
            Writer.WriteLine(
                $"{{yellow}}{randomMob.Name} jumps out from the {thingToHarvest.Name} and attacks you !{{/}}",
                player.ConnectionId);
            room.Mobs.Add(randomMob);
            player.Status = CharacterStatus.Status.Standing;
            InitFightStatus(randomMob, player);
            return;
        }

        if (roll <= 3)
        {
            Writer.WriteLine(
                $"{{yellow}}You cut yourself foraging, OUCH!{{/}}",
                player.ConnectionId);
            player.Status = CharacterStatus.Status.Standing;
            return;
        }

        var canDoSkill = Helpers.SkillSuccessCheck(player, "foraging");

        if (!canDoSkill)
        {
            Writer.WriteLine("You fail to harvest a thing.", player.ConnectionId);
            //TODO: fix after moving update ui to automagic
            //Helpers.SkillLearnMistakes(player, "foraging");
            player.Status = CharacterStatus.Status.Standing;
            return;
        }

        var collected = "";
        var collectedCount = 1;
        
        foreach (var harvestItem in thingToHarvest.Container.Items.Where(harvestItem => _dice.Roll(1, 1, 10) <= 3))
        {
            collected = harvestItem.Name;
            collectedCount++;

            // if the User has herbalism the conditions will be higher rated
            // on success skill check the roll could be 50, 100
            // if elven the condition can be 10 points higher
            harvestItem.Condition = _dice.Roll(1, 1, 65);

            switch (player.Race)
            {
                case "Wood-Elf":
                    harvestItem.Condition += _dice.Roll(1, 10, 20);
                    break;
                case "Elf":
                case "Drow":
                    harvestItem.Condition += _dice.Roll(1, 5, 10);
                    ;
                    break;
            }

            player.Inventory.Add(harvestItem);
        }

        for (var i = 0; i < thingToHarvest.Container.Items.Count; i++)
        {
            thingToHarvest.Container.Items.Remove(thingToHarvest.Container.Items[i]);
        }

        Writer.WriteLine(
            $"Ah you have collected {collectedCount} {collected}{(collectedCount > 1 ? "'s" : "")}",
            player.ConnectionId);
        player.Status = CharacterStatus.Status.Standing;
    }

    private void InitFightStatus(Player player, Player target)
    {
        player.Target = string.IsNullOrEmpty(player.Target) ? target.Name : player.Target;
        player.Status = CharacterStatus.Status.Fighting;
        target.Status = (target.Status & CharacterStatus.Status.Stunned) != 0 ? CharacterStatus.Status.Stunned : CharacterStatus.Status.Fighting;
        target.Target = string.IsNullOrEmpty(target.Target) ? player.Name : target.Target; //for group combat, if target is ganged, there target should not be changed when combat is initiated.

        if (player.Target == player.Name)
        {
            player.Status = CharacterStatus.Status.Standing;
            return;
        }

        if (!Cache.IsCharInCombat(player.Id.ToString()))
        {
            Cache.AddCharToCombat(player.Id.ToString(), player);
        }

        if (!Cache.IsCharInCombat(target.Id.ToString()))
        {
            Cache.AddCharToCombat(target.Id.ToString(), target);
        }
    }
}