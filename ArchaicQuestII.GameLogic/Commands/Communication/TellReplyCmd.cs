using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class TellReplyCmd : ICommand
{

    public TellReplyCmd(ICoreHandler coreHandler)
    {
        Aliases = new[] {"tellreply", "reply"};
        Description = "Replies to your last incoming tell";
        Usages = new[] {"Type: tellreply 'message'"};
        Title = "";
        DeniedStatus = null;
        UserRole = UserRole.Player;

        Handler = coreHandler;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }
    public ICoreHandler Handler { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        if (string.IsNullOrEmpty(player.ReplyTo))
        {
            Handler.Client.WriteLine("<p>You have no one to reply too.</p>", player.ConnectionId);
            return;
        }
        
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Handler.Client.WriteLine("Reply what?", player.ConnectionId);
            return;
        }

        var replyInput = input.ToList();
        replyInput.Insert(1, player.ReplyTo);

        Handler.Command.GetCommand("tell").Execute(player, room, replyInput.ToArray());
    }
}