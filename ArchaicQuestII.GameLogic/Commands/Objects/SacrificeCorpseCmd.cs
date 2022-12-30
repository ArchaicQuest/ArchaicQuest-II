using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class SacrificeCorpseCmd : ICommand
{
    public SacrificeCorpseCmd(ICore core)
    {
        Aliases = new[] {"sacrifice", "sac"};
        Description = @"'{yellow}sacrifice{/}' is used to sacrifice a corpse of a dead mob to the gods who will reward the player with gold. 

Examples:
sacrifice corpse
sac rat

";
        Usages = new[] {"Type: sacrifice rat"};
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
        Core = core;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }
    public ICore Core { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            Core.Writer.WriteLine("<p>Sacrifice whom?</p>", player.ConnectionId);
            return;
        }

        var itemToRemove = room.Items.FirstOrDefault(u => u.Name.Contains(target));

        if (itemToRemove != null)
        {
            room.Items.Remove(itemToRemove);
            var coinCount = new Dice().Roll(1, 1, 12);
            player.Money.Gold += coinCount;
            
            Core.Writer.WriteLine(
                coinCount == 1
                    ? "The gods give you a measly gold coin for your sacrifice."
                    : $"The gods give you {coinCount} gold coins for your sacrifice.",
                player.ConnectionId);

            Core.Writer.WriteToOthersInRoom($"{player.Name} sacrifices {itemToRemove.Name.ToLower()}.", room,
                player);

            Core.UpdateClient.UpdateScore(player);
        }
    }
}