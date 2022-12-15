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
        public QuitCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
        {
            Aliases = new[] {"quit"};
            Description = "Leave the game.";
            Usages = new[] {"Type: quit"};
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
            if (player.Status == CharacterStatus.Status.Fighting)
            {
                Writer.WriteLine("You can't quit in a fight!", player.ConnectionId);
                return;
            }

            player.Buffer = new Queue<string>();
            var lastLoginTime = player.LastLoginTime;
            var playTime = DateTime.Now.Subtract(lastLoginTime).TotalMinutes;
            player.PlayTime += (int)DateTime.Now.Subtract(lastLoginTime).TotalMinutes;

            var account = Cache.GetPlayerDatabase().GetById<Account.Account>(player.AccountId, PlayerDataBase.Collections.Account);
            account.Stats.TotalPlayTime += playTime;

            Cache.GetPlayerDatabase().Save(account, PlayerDataBase.Collections.Account);
            Cache.GetPlayerDatabase().Save(player, PlayerDataBase.Collections.Players);
            
            Writer.WriteLine("Character saved.", player.ConnectionId);

            foreach (var pc in room.Players)
            {
                if (pc.Name.Equals(player.Name))
                {
                    Writer.WriteLine("You wave goodbye and vanish.", pc.ConnectionId);
                    continue;
                }
                Writer.WriteLine($"{player.Name} waves goodbye and vanishes.", pc.ConnectionId);
            }

            room.Players.Remove(player);
            Writer.WriteLine($"We await your return {player.Name}. If you enjoyed your time here, help spread the word by tweeting, writing a blog posts or posting reviews online.", player.ConnectionId);
            Helpers.PostToDiscord($"{player.Name} quit after playing for {Math.Floor(DateTime.Now.Subtract(player.LastLoginTime).TotalMinutes)} minutes.", "event", Cache.GetConfig());
            Cache.RemovePlayer(player.ConnectionId);
        }
    }
}