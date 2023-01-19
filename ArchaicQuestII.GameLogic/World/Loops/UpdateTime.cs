using System.Linq;
using System.Web;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;

namespace ArchaicQuestII.GameLogic.World.Loops;

public class UpdateTime : IGameLoop
{
    public int TickDelay => 60000;
    public ICoreHandler Handler { get; set; }
    public bool Enabled { get; set; }

    public void Loop()
    {
        Handler.World.DisplayTimeOfDayMessage();
        
        var weather = $"<span class='weather'>{Handler.World.SimulateWeatherTransitions()}</span>";

        foreach (var player in Handler.Character.GetPlayerCache().Values.ToList())
        {
            //check if player is not indoors
            // TODO:
            Handler.Client.UpdateTime(player, Handler.World.Time);
            var room = Handler.World.GetRoom(player.RoomId);

            if (room == null)
            {
                return;
            }

            if (room.Terrain != Room.Room.TerrainType.Inside && room.Terrain != Room.Room.TerrainType.Underground)
            {
                Handler.Client.WriteLine(weather, player.ConnectionId);

            }

            //reduce frequency of hints to only 50% of the time
            if (player.Config.Hints && DiceBag.Roll(1, 0, 1) == 1)
            {
                Handler.Client.WriteLine(
                    $"<span style='color:lawngreen'>[Hint]</span> {HttpUtility.HtmlEncode(Hints.Get()[DiceBag.Roll(1, 0, Hints.Get().Count)])}",
                    player.ConnectionId);
            }
        }
    }
}