using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Loops
{
	public class TimeLoop : ILoop
	{
        public int TickDelay => 60000;

        public bool ConfigureAwait => false;

        private ICore _core;
        private List<Player> _players;

        public void Init(ICore core, ICommandHandler commandHandler)
        {
            _core = core;
        }

        public void PreTick()
        {
            _players = _core.Cache.GetPlayerCache().Values.ToList();
        }

        public void Tick()
        {
            _core.Time.DisplayTimeOfDayMessage(_core.Time.UpdateTime());

            var weather = $"<span class='weather'>{_core.Weather.SimulateWeatherTransitions()}</span>";

            foreach (var player in _players)
            {
                //check if player is not indoors
                // TODO:
                _core.UpdateClient.UpdateTime(player);
                var room = _core.Cache.GetRoom(player.RoomId);

                if (room == null)
                {
                    return;
                }

                if (room.Terrain != Room.TerrainType.Inside && room.Terrain != Room.TerrainType.Underground)
                {
                    _core.Writer.WriteLine(weather, player.ConnectionId);

                }
            }
        }

        public void PostTick()
        {
            _players.Clear();
        }
    }
}

