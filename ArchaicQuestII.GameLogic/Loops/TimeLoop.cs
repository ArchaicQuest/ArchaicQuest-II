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
        private List<Player> _players = new List<Player>();

        public void PreTick()
        {
            _players = Services.Instance.Cache.GetPlayerCache().Values.ToList();
        }

        public void Tick()
        {
            //Console.WriteLine("TimeLoop");

            Services.Instance.Time.DisplayTimeOfDayMessage(Services.Instance.Time.UpdateTime());

            var weather =
                $"<span class='weather'>{Services.Instance.Weather.SimulateWeatherTransitions()}</span>";

            foreach (var player in _players)
            {
                //check if player is not indoors
                // TODO:
                Services.Instance.UpdateClient.UpdateTime(player);
                var room = Services.Instance.Cache.GetRoom(player.RoomId);

                if (room == null)
                {
                    return;
                }

                if (
                    room.Terrain != Room.TerrainType.Inside
                    && room.Terrain != Room.TerrainType.Underground
                )
                {
                    Services.Instance.Writer.WriteLine(weather, player);
                }
            }
        }

        public void PostTick()
        {
            _players.Clear();
        }
    }
}
