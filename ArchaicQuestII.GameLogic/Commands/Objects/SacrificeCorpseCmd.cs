using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class SacrificeCorpseCmd : ICommand
{
    public SacrificeCorpseCmd(ICore core)
    {
        Aliases = new[] {"sacrifice", "sac"};
        Description = "You sacrifice a corpse.";
        Usages = new[] {"Type: sacrifice rat"};
        UserRole = UserRole.Player;
        Core = core;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
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