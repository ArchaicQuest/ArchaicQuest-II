using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.SeedData;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Loops
{
    public class MobEmoteLoop : ILoop
    {
        public int TickDelay => 30000;
        public bool ConfigureAwait => false;
        private ICore _core;
        private ICommandHandler _commandHandler;
        private List<Room> _mobRooms;

        public void Init(ICore core, ICommandHandler commandHandler)
        {
            _core = core;
            _commandHandler = commandHandler;
        }

        public void PreTick()
        {
            _mobRooms = _core.Cache.GetAllRooms().Where(x => x.Mobs.Any()).ToList();
        }

        public void Tick()
        {
            var mobIds = new List<Guid>();
            foreach (var room in _mobRooms)
            {

                foreach (var mob in room.Mobs.Where(x => x.Status == CharacterStatus.Status.Standing).ToList())
                {
                    if (mob.Emotes.Any() && mob.Emotes[0] != null && DiceBag.Roll(1, 0, 1) == 1)
                    {

                        var emote = mob.Emotes[DiceBag.Roll(1, 0, mob.Emotes.Count - 1)];
                        foreach (var player in room.Players)
                        {
                            //example mob emote: Larissa flicks through her journal.
                            if (emote == null)
                            {
                                continue;
                            }
                            _core.Writer.WriteLine($"<p class='mob-emote'>{mob.Name} {emote}</p>",
                                player.ConnectionId);
                        }
                    }

                    if (mobIds.Contains(mob.Id))
                    {
                        continue;
                    }



                    if (mob.Roam && DiceBag.Roll(1, 1, 100) >= 50)
                    {

                        if (mob.Buffer.Count == 0)
                        {
                            var exits = Helpers.GetListOfExits(room.Exits);
                            var reveseDirection = "";
                            var reveseDirection2 = "";
                            var reveseDirection3 = "";
                            if (exits != null)
                            {
                                var direction = exits[DiceBag.Roll(1, 0, exits.Count - 1)];

                                var newExit = Helpers.IsExit(direction, room);

                                //mob can't roam to a new area or change Z axis
                                // to stop mobs appearing in the wrong area
                                if (newExit.AreaId == room.AreaId && newExit.Coords.Z == room.Coords.Z)
                                {

                                    reveseDirection = Helpers.ReturnOpositeExitName(direction);

                                    mob.Buffer.Enqueue(direction);

                                    // Max mob can roam 3 times away from start loc

                                    var newRoom = _core.Cache.GetRoom($"{newExit.AreaId}{newExit.Coords.X}{newExit.Coords.Y}{newExit.Coords.Z}");

                                    if (newRoom != null)
                                    {
                                        exits = Helpers.GetListOfExits(newRoom.Exits);
                                        direction = exits[DiceBag.Roll(1, 0, exits.Count - 1)];

                                        newExit = Helpers.IsExit(direction, newRoom);

                                        // to stop mobs appearing in the wrong area
                                        if (newExit.AreaId == newRoom.AreaId && newExit.Coords.Z == newRoom.Coords.Z)
                                        {
                                            reveseDirection2 = Helpers.ReturnOpositeExitName(direction);

                                            mob.Buffer.Enqueue(direction);


                                            // 3rd room
                                            newRoom = _core.Cache.GetRoom($"{newExit.AreaId}{newExit.Coords.X}{newExit.Coords.Y}{newExit.Coords.Z}");

                                            if (newRoom != null)
                                            {
                                                exits = Helpers.GetListOfExits(newRoom.Exits);
                                                direction = exits[DiceBag.Roll(1, 0, exits.Count - 1)];

                                                newExit = Helpers.IsExit(direction, newRoom);

                                                // to stop mobs appearing in the wrong area
                                                if (newExit.AreaId == newRoom.AreaId && newExit.Coords.Z == newRoom.Coords.Z)
                                                {
                                                    reveseDirection3 = Helpers.ReturnOpositeExitName(direction);

                                                    mob.Buffer.Enqueue(direction);
                                                    mob.Buffer.Enqueue(reveseDirection3);
                                                    mob.Buffer.Enqueue(reveseDirection2);
                                                    mob.Buffer.Enqueue(reveseDirection);
                                                }
                                            }
                                            else
                                            {
                                                mob.Buffer.Enqueue(reveseDirection2);
                                                mob.Buffer.Enqueue(reveseDirection);
                                            }
                                        }
                                        else
                                        {
                                            mob.Buffer.Enqueue(reveseDirection2);
                                            mob.Buffer.Enqueue(reveseDirection);
                                        }
                                    }
                                    else
                                    {
                                        mob.Buffer.Enqueue(reveseDirection);
                                    }


                                }

                            }
                        }

                        if (!string.IsNullOrEmpty(mob.Commands) && mob.Buffer.Count == 0)
                        {
                            mob.RoomId = $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}";
                            var commands = mob.Commands.Split(";");

                            foreach (var command in commands)
                            {
                                mob.Buffer.Enqueue(command);
                            }

                        }

                        if (mob.Buffer.Count > 0)
                        {
                            var mobCommand = mob.Buffer.Dequeue();

                            _commandHandler.HandleCommand(mob, room, mobCommand);
                        }

                        mobIds.Add(mob.Id);
                    }

                }
            }
        }

        public void PostTick()
        {
            _mobRooms.Clear();
        }
    }
}

