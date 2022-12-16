using System;
using System.Collections.Generic;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class QuitCmd : ICommand
    {
        public QuitCmd(ICore core)
        {
            Aliases = new[] {"quit"};
            Description = "Leave the game.";
            Usages = new[] {"Type: quit"};
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Busy,
                CharacterStatus.Status.Fighting
            };
            UserRole = UserRole.Player;
            Core = core;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            if (player.Status == CharacterStatus.Status.Fighting)
            {
                Core.Writer.WriteLine("You can't quit in a fight!", player.ConnectionId);
                return;
            }

            player.Buffer = new Queue<string>();
            var lastLoginTime = player.LastLoginTime;
            var playTime = DateTime.Now.Subtract(lastLoginTime).TotalMinutes;
            player.PlayTime += (int)DateTime.Now.Subtract(lastLoginTime).TotalMinutes;

            var account = Core.PlayerDataBase.GetById<Account.Account>(player.AccountId, PlayerDataBase.Collections.Account);
            account.Stats.TotalPlayTime += playTime;

            Core.PlayerDataBase.Save(account, PlayerDataBase.Collections.Account);
            Core.PlayerDataBase.Save(player, PlayerDataBase.Collections.Players);
            
            Core.Writer.WriteLine("Character saved.", player.ConnectionId);

            foreach (var pc in room.Players)
            {
                if (pc.Name.Equals(player.Name))
                {
                    Core.Writer.WriteLine("You wave goodbye and vanish.", pc.ConnectionId);
                    continue;
                }
                Core.Writer.WriteLine($"{player.Name} waves goodbye and vanishes.", pc.ConnectionId);
            }

            room.Players.Remove(player);
            Core.Writer.WriteLine($"We await your return {player.Name}. If you enjoyed your time here, help spread the word by tweeting, writing a blog posts or posting reviews online.", player.ConnectionId);
            Helpers.PostToDiscord($"{player.Name} quit after playing for {Math.Floor(DateTime.Now.Subtract(player.LastLoginTime).TotalMinutes)} minutes.", "event", Core.Cache.GetConfig());
            Core.Cache.RemovePlayer(player.ConnectionId);
        }
    }
}