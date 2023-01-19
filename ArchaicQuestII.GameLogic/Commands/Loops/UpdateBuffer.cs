using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Commands.Loops;

public class UpdateBuffer : IGameLoop
{
    public int TickDelay => 125;
    public ICoreHandler Handler { get; set; }
    public bool Enabled { get; set; }
    public void Loop()
    {
        var players = Handler.Character.GetPlayerCache();
        var validPlayers = players.Where(x => x.Value.Buffer.Count > 0);

        foreach (var player in validPlayers)
        {
            // don't action commands if player is lagged
            if (player.Value.Lag > 0)
            {
                continue;
            }

            var command = player.Value.Buffer.Dequeue();
            var room = Handler.World.GetRoom(player.Value.RoomId);
            player.Value.LastCommandTime = DateTime.Now;

            if (player.Value.CommandLog.Count >= 2500)
            {
                player.Value.CommandLog = new List<string>();
            }

            player.Value.CommandLog.Add($"{string.Format("{0:f}", DateTime.Now)} - {command}");
            Handler.Command.HandleCommand(player.Value, room, command);

        }
    }
}