using System.Linq;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Character.Loops;

public class UpdatePlayerLag : IGameLoop
{
    public int TickDelay => 4000;
    public ICoreHandler Handler { get; set; }
    public bool Enabled { get; set; }
    public void Loop()
    {
        var players = Handler.Character.GetPlayerCache();
        var validPlayers = players.Where(x => x.Value.Lag > 0);

        foreach (var player in validPlayers)
        {

            player.Value.Lag -= 1;
        }
    }
}