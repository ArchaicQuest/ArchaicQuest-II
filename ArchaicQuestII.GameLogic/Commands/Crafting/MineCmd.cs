using System;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class MineCmd : ICommand
{
    public MineCmd()
    {
        Aliases = new[] { "mine", };
        Description =
            "Mine ore to gain raw metal materials and gems, must have a PickAxe type item in inventory such as a pickaxe. An Pickaxe that is of type weapon will not work. Mining uses the foraging skill.";
        Usages = new[] { "Type: mine ore" };
        Title = "";
        DeniedStatus = new[]
        {
            CharacterStatus.Status.Busy,
            CharacterStatus.Status.Dead,
            CharacterStatus.Status.Fighting,
            CharacterStatus.Status.Ghost,
            CharacterStatus.Status.Fleeing,
            CharacterStatus.Status.Incapacitated,
            CharacterStatus.Status.Sleeping,
            CharacterStatus.Status.Stunned,
            CharacterStatus.Status.Resting,
            CharacterStatus.Status.Sitting,
        };
        UserRole = UserRole.Player;
    }

    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            Services.Instance.Writer.WriteLine("Chop what?", player.ConnectionId);
            return;
        }

        if (player.Status == CharacterStatus.Status.Busy)
        {
            Services.Instance.Writer.WriteLine("You are already doing it.", player.ConnectionId);
            return;
        }

        var thingToHarvest = room.Items.FirstOrDefault(
            x => x.Name.Contains(target, StringComparison.OrdinalIgnoreCase)
        );

        if (thingToHarvest == null)
        {
            Services.Instance.Writer.WriteLine($"You don't see that here.", player.ConnectionId);
            return;
        }

        if (thingToHarvest.ItemType != Item.Item.ItemTypes.Mineable)
        {
            Services.Instance.Writer.WriteLine("You can't mine this.", player.ConnectionId);
            return;
        }

        var hasChoppingTool = player.Inventory.FirstOrDefault(
            x => x.ItemType == Item.Item.ItemTypes.PickAxe
        );

        if (hasChoppingTool == null)
        {
            Services.Instance.Writer.WriteLine(
                $"You attempt to dig {thingToHarvest.Name.ToLower()} with your bare hands. Probably better to try with a Pickaxe.",
                player.ConnectionId
            );
            return;
        }

        if (!thingToHarvest.Container.Items.Any())
        {
            Services.Instance.Writer.WriteLine(
                "There's nothing left to mine.",
                player.ConnectionId
            );
            return;
        }

        Harvest(player, room, thingToHarvest, hasChoppingTool);
    }

    private async void Harvest(
        Player player,
        Room room,
        Item.Item thingToHarvest,
        Item.Item hasChoppingTool
    )
    {
        player.Status = CharacterStatus.Status.Busy;

        try
        {
            Services.Instance.UpdateClient.PlaySound("mining", player);

            Services.Instance.Writer.WriteLine(
                $"<p>You begin mining {thingToHarvest.Name}.</p>",
                player.ConnectionId
            );

            await Task.Delay(4000);

            if (!room.Players.Contains(player) || player.Status != CharacterStatus.Status.Busy)
            {
                return;
            }

            Services.Instance.UpdateClient.PlaySound("mining", player);

            Services.Instance.Writer.WriteLine(
                $"<p>You swing your {hasChoppingTool.Name} back and forth striking the rock with each swing.</p>",
                player.ConnectionId
            );

            await Task.Delay(4000);

            if (!room.Players.Contains(player) || player.Status != CharacterStatus.Status.Busy)
            {
                return;
            }

            Services.Instance.UpdateClient.PlaySound("mining", player);

            Services.Instance.Writer.WriteLine("<p>You continue mining.</p>", player.ConnectionId);

            await Task.Delay(4000);

            if (!room.Players.Contains(player) || player.Status != CharacterStatus.Status.Busy)
            {
                return;
            }

            var roll = DiceBag.Roll(1, 1, 10);

            if (roll <= 1)
            {
                // Landslide? hurt player? TODO: future update.
            }

            if (roll <= 3)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>{{yellow}}You cut yourself mining, OUCH!{{/}}</p>",
                    player.ConnectionId
                );
                player.Status = CharacterStatus.Status.Standing;
                return;
            }

            if (!player.RollSkill(SkillName.Foraging))
            {
                player.FailedSkill(SkillName.Foraging, out var message);
                Services.Instance.Writer.WriteLine(
                    "<p>You fail to mine a thing.</p>",
                    player.ConnectionId
                );
                Services.Instance.Writer.WriteLine(message, player.ConnectionId);
                player.Status = CharacterStatus.Status.Standing;
                return;
            }

            var collected = "";
            var collectedCount = 0;

            foreach (
                var harvestItem in thingToHarvest.Container.Items
                    .Where(harvestItem => DiceBag.Roll(1, 1, 10) <= 3)
                    .ToList()
            )
            {
                if (!collected.Contains(harvestItem.Name))
                {
                    if (collected.Length > 0)
                    {
                        collected += " and " + harvestItem.Name;
                    }
                    else
                    {
                        collected += harvestItem.Name + " ";
                    }
                }

                collectedCount++;
                thingToHarvest.Container.Items.Remove(harvestItem);

                // if the User has herbalism the conditions will be higher rated
                // on success skill check the roll could be 50, 100
                // if elven the condition can be 10 points higher
                harvestItem.Condition = DiceBag.Roll(1, 1, 65);

                switch (player.Race)
                {
                    case "Wood-Elf":
                        harvestItem.Condition += DiceBag.Roll(1, 10, 20);
                        break;
                    case "Elf":
                    case "Drow":
                        harvestItem.Condition += DiceBag.Roll(1, 5, 10);
                        break;
                }

                player.Inventory.Add(harvestItem);
            }

            if (string.IsNullOrEmpty(collected))
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>You fail to collect a single thing.</p>",
                    player.ConnectionId
                );
            }
            else
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>Ah you have collected some {collected}</p>",
                    player.ConnectionId
                );
            }

            player.Status = CharacterStatus.Status.Standing;
            Services.Instance.UpdateClient.UpdateInventory(player);
        }
        catch (Exception ex)
        {
            Console.WriteLine("MineCommand.cs: " + ex);
            player.Status = CharacterStatus.Status.Standing;
        }
    }

    private void InitFightStatus(Player player, Player target)
    {
        player.Target = string.IsNullOrEmpty(player.Target) ? target.Name : player.Target;
        player.Status = CharacterStatus.Status.Fighting;
        target.Status =
            (target.Status & CharacterStatus.Status.Stunned) != 0
                ? CharacterStatus.Status.Stunned
                : CharacterStatus.Status.Fighting;
        target.Target = string.IsNullOrEmpty(target.Target) ? player.Name : target.Target; //for group combat, if target is ganged, there target should not be changed when combat is initiated.

        if (player.Target == player.Name)
        {
            player.Status = CharacterStatus.Status.Standing;
            return;
        }

        if (!Services.Instance.Cache.IsCharInCombat(player.Id.ToString()))
        {
            Services.Instance.Cache.AddCharToCombat(player.Id.ToString(), player);
        }

        if (!Services.Instance.Cache.IsCharInCombat(target.Id.ToString()))
        {
            Services.Instance.Cache.AddCharToCombat(target.Id.ToString(), target);
        }
    }
}
