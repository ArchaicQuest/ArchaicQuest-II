using System;
using System.Collections.Generic;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class QuitCmd : ICommand
    {
        public QuitCmd(ICoreHandler coreHandler)
        {
            Aliases = new[] {"quit"};
            Description = "Leave the game, it auto saves and removes your character from the game. If you don't quit you will go link dead and at risk of getting killed and robbed.";
            Usages = new[] {"Type: quit"};
            Title = "";
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Busy,
                CharacterStatus.Status.Fighting
            };
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
            player.Buffer = new Queue<string>();
            var lastLoginTime = player.LastLoginTime;
            var playTime = DateTime.Now.Subtract(lastLoginTime).TotalMinutes;
            player.PlayTime += (int)DateTime.Now.Subtract(lastLoginTime).TotalMinutes;

            var account = Handler.Pdb.GetById<Account.Account>(player.AccountId, PlayerDataBase.Collections.Account);
            account.Stats.TotalPlayTime += playTime;

            Handler.Pdb.Save(account, PlayerDataBase.Collections.Account);
            Handler.Pdb.Save(player, PlayerDataBase.Collections.Players);
            
            Handler.Client.WriteLine("<p>Character saved.</p>", player.ConnectionId);
            Handler.Client.WriteLine("<p>You wave goodbye and vanish.</p>", player.ConnectionId);
            Handler.Client.WriteToOthersInRoom($"<p>{player.Name} waves goodbye and vanishes.</p>", room, player);

            room.Players.Remove(player);
            Handler.Client.WriteLine($"<p>We await your return {player.Name}. If you enjoyed your time here, help spread the word by tweeting, writing a blog posts or posting reviews online.</p>", player.ConnectionId);
            
            if(Handler.Config.PostToDiscord)
                Helpers.PostToDiscord($"{player.Name} quit after playing for {Math.Floor(DateTime.Now.Subtract(player.LastLoginTime).TotalMinutes)} minutes.", Handler.Config.EventsDiscordWebHookURL);
            
            Handler.Character.RemovePlayer(player.ConnectionId);
        }
    }
}