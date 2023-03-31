using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.SeedData;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Loops
{
	public class CorpseLoop : ILoop
	{
        private ICore _core;
        public int TickDelay => 120000; // 2 Minutes

        public bool ConfigureAwait => true;

        private List<Room> _rooms;

        public void Init(ICore core, ICommandHandler commandHandler)
        {
            _core = core;
            _rooms = new List<Room>();
        }

        public void PreTick()
        {
            var rooms = _core.Cache.GetAllRoomsToRepop();
        }

        public void Tick()
        {
            foreach (var room in _rooms)
            {
                //get corpse and remove
                var corpses = room.Items.FindAll(x =>
                    x.Description.Room.Contains("corpse", StringComparison.CurrentCultureIgnoreCase));

                foreach (var corpse in corpses)
                {
                    switch (corpse.Decay)
                    {
                        case 10:
                        case 9:
                        case 8:
                        case 7:
                        case 6:
                            corpse.Description.Room = $"{corpse.Name.ToLower()} lies here.";
                            break;
                        case 5:
                        case 4:
                            corpse.Description.Room = $"{corpse.Name.ToLower()} is buzzing with flies.";
                            break;
                        case 3:
                            corpse.Description.Room = $"{corpse.Name.ToLower()} fills the air with a foul stench.";
                            break;
                        case 2:
                            corpse.Description.Room = $"{corpse.Name.ToLower()} is crawling with vermin.";
                            break;
                        case 1:
                            corpse.Description.Room = $"{corpse.Name.ToLower()} is in the last stages of decay.";
                            break;
                        case 0:

                            foreach (var pc in room.Players)
                            {
                                _core.Writer.WriteLine($"<p>A quivering horde of maggots consumes {corpse.Name.ToLower()}.</p>", pc.ConnectionId);
                            }
                            room.Items.Remove(corpse);
                            break;
                        default:
                            corpse.Description.Room = $"{corpse.Name.ToLower()} lies here.";
                            break;
                    }

                    corpse.Decay--;
                }
            }
        }

        public void PostTick()
        {
            _rooms.Clear();
        }
    }
}

