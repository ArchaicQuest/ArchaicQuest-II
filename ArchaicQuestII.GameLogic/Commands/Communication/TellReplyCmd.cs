using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class TellReplyCmd : ICommand
{
    public TellReplyCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"tellreply"};
        Description = "Replies to your last incoming tell";
        Usages = new[] {"Type: tellreply 'message'"};
        UserRole = UserRole.Player;
        Writer = writeToClient;
        Cache = cache;
        UpdateClient = updateClient;
        RoomActions = roomActions;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public UserRole UserRole { get; }
    public IWriteToClient Writer { get; }
    public ICache Cache { get; }
    public IUpdateClientUI UpdateClient { get; }
    public IRoomActions RoomActions { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        if (string.IsNullOrEmpty(player.ReplyTo))
        {
            Writer.WriteLine("<p>You have no one to reply too.</p>", player.ConnectionId);
            return;
        }
        
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Writer.WriteLine("Reply what?", player.ConnectionId);
            return;
        }

        var replyInput = input.ToList();
        replyInput.Insert(1, player.ReplyTo);

        Cache.GetCommand("tell", out var command);
        
        command.Execute(player, room, replyInput.ToArray());
    }
}