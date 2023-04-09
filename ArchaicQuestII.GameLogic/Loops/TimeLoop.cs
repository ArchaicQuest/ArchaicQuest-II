using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Loops
{
    public class TimeLoop : ILoop
	{
        public int TickDelay => 60000;

        public bool ConfigureAwait => false;
        private List<Player> _players;

        public void Init()
        {

        }

        public void PreTick()
        {
            _players = CoreHandler.Instance.Cache.GetPlayerCache().Values.ToList();
        }

        public void Tick()
        {
            CoreHandler.Instance.Time.DisplayTimeOfDayMessage(CoreHandler.Instance.Time.UpdateTime());

            var weather = $"<span class='weather'>{CoreHandler.Instance.Weather.SimulateWeatherTransitions()}</span>";

            foreach (var player in _players)
            {
                //check if player is not indoors
                // TODO:
                CoreHandler.Instance.UpdateClient.UpdateTime(player);
                var room = CoreHandler.Instance.Cache.GetRoom(player.RoomId);

                if (room == null)
                {
                    return;
                }

                if (room.Terrain != Room.TerrainType.Inside && room.Terrain != Room.TerrainType.Underground)
                {
                    CoreHandler.Instance.Writer.WriteLine(weather, player.ConnectionId);

                }
            }
        }

        public void PostTick()
        {
            _players.Clear();
        }
    }
}

