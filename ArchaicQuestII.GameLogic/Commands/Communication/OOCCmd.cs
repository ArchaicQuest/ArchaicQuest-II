using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class OOCCmd : ICommand
{
    public OOCCmd(ICore core)
    {
        Aliases = new[] {"ooc"};
        Description = "Sends a message to out of character channel";
        Usages = new[] {"Type: ooc Did anyone see the game last night?"};
            Title = "";
    DeniedStatus = null;
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
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Core.Writer.WriteLine("<p>ooc what?</p>", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        Core.Writer.WriteLine($"<p class='ooc'>[<span>OOC</span>] You: {text}</p>", player.ConnectionId);
       // Core.Writer.WriteToOthersInGame($"<p class='ooc'>[<span>OOC</span>] {player.Name}: {text}</p>", player);
        
        foreach (var pc in Core.Cache.GetAllPlayers().Where(pc => pc.Config.OocChannel))
        {
            Core.UpdateClient.UpdateCommunication(pc, $"<p class='ooc'>[<span>OOC</span>] {(player.Name == pc.Name ? "You" : player.Name)}: {text}</p>", "ooc");
        }

        Helpers.PostToDiscordBot($"{player.Name}: {text}",1092857287758057593,  Core.Cache.GetConfig().ChannelDiscordWebHookURL);

    }
}