using System.Linq;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class OOCCmd : ICommand
{
    public OOCCmd()
    {
        Aliases = new[] { "ooc" };
        Description = "Sends a message to out of character channel";
        Usages = new[] { "Type: ooc Did anyone see the game last night?" };
        Title = "";
        DeniedStatus = null;
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
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Services.Instance.Writer.WriteLine("<p>ooc what?</p>", player.ConnectionId);
            return;
        }

        var text = string.Join(" ", input.Skip(1));

        Services.Instance.Writer.WriteLine(
            $"<p class='ooc'>[<span>OOC</span>] You: {text}</p>",
            player.ConnectionId
        );
        Services.Instance.Writer.WriteToOthersInGame(
            $"<p class='ooc'>[<span>OOC</span>] {player.Name}: {text}</p>",
            player
        );

        foreach (
            var pc in Services.Instance.Cache.GetAllPlayers().Where(pc => pc.Config.OocChannel)
        )
        {
            Services.Instance.UpdateClient.UpdateCommunication(
                pc,
                $"<p class='ooc'>[<span>OOC</span>] {(player.Name == pc.Name ? "You" : player.Name)}: {text}</p>",
                "ooc"
            );
        }

        Helpers.PostToDiscordBot(
            $"{player.Name}: {text}",
            1092857287758057593,
            Services.Instance.Cache.GetConfig().ChannelDiscordWebHookURL
        );
    }
}
