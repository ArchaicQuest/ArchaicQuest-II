using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Commands;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{

    public class GameLoop : IGameLoop
    {


        private IWriteToClient _writeToClient;
        private ICache _cache;
        private ICommands _commands;
        private ICombat _combat;
        private IDataBase _db;
        private IDice _dice;
        private IUpdateClientUI _client;
        private ITime _time;
        private ICore _core;

        public GameLoop(IWriteToClient writeToClient, ICache cache, ICommands commands, ICombat combat, IDataBase database, IDice dice, IUpdateClientUI client, ITime time, ICore core)
        {
            _writeToClient = writeToClient;
            _cache = cache;
            _commands = commands;
            _combat = combat;
            _db = database;
            _dice = dice;
            _client = client;
            _time = time;
            _core = core;
        }

        public int GainAmount(int value, Player player)
        {
            return player.Status switch
            {
                CharacterStatus.Status.Sleeping => value *= 3,
                CharacterStatus.Status.Resting => value *= 2,
                _ => value
            };
        }

        public async Task UpdateTime()
        {


            Console.WriteLine("started loop");
            while (true)
            {
                //2 mins
                await Task.Delay(120000);
                var rooms = _cache.GetAllRoomsToRepop();
                var players = _cache.GetPlayerCache().Values.ToList();

                foreach (var room in rooms)
                {
                    var originalRoom = _db.GetById<Room>(room.Id, DataBase.Collections.Room);

                    foreach (var mob in originalRoom.Mobs)
                    {
                        // need to check if mob exists before adding
                        var mobExist = room.Mobs.FirstOrDefault(x => x.Id.Equals(mob.Id));

                        if (originalRoom.Mobs.Count != room.Mobs.Count && room.Players.Count == 0)
                        {

                            room.Mobs = originalRoom.Mobs;
                        }
                        else
                        {
                            if (mobExist != null)
                            {
                                mobExist.Attributes.Attribute[EffectLocation.Hitpoints] +=
                                    _dice.Roll(1, 2, 5) * mobExist.Level;
                                mobExist.Attributes.Attribute[EffectLocation.Mana] +=
                                    _dice.Roll(1, 2, 5) * mobExist.Level;
                                mobExist.Attributes.Attribute[EffectLocation.Moves] +=
                                    _dice.Roll(1, 2, 5) * mobExist.Level;

                                if (mobExist.Attributes.Attribute[EffectLocation.Hitpoints] >
                                    mobExist.MaxAttributes.Attribute[EffectLocation.Hitpoints])
                                {
                                    mobExist.Attributes.Attribute[EffectLocation.Hitpoints] =
                                        mobExist.MaxAttributes.Attribute[EffectLocation.Hitpoints];
                                }

                                if (mobExist.Attributes.Attribute[EffectLocation.Mana] >
                                    mobExist.MaxAttributes.Attribute[EffectLocation.Mana])
                                {
                                    mobExist.Attributes.Attribute[EffectLocation.Mana] =
                                        mobExist.MaxAttributes.Attribute[EffectLocation.Mana];
                                }

                                if (mobExist.Attributes.Attribute[EffectLocation.Moves] >
                                    mobExist.MaxAttributes.Attribute[EffectLocation.Moves])
                                {
                                    mobExist.Attributes.Attribute[EffectLocation.Moves] =
                                        mobExist.MaxAttributes.Attribute[EffectLocation.Moves];
                                }
                            }
                        }
                    }

                    

                    foreach (var item in originalRoom.Items)
                    {
                        // need to check if item exists before adding
                        var itemExist = room.Items.FirstOrDefault(x => x.Id.Equals(item.Id));

                        if (itemExist == null)
                        {
                            room.Items.Add(item);
                        }
                          itemExist = room.Items.FirstOrDefault(x => x.Id.Equals(item.Id));

                        if (itemExist.Container.Items.Count < item.Container.Items.Count)
                        {
                            itemExist.Container.Items = item.Container.Items;
                            itemExist.Container.IsOpen = item.Container.IsOpen;
                            itemExist.Container.IsLocked = item.Container.IsLocked;
                        }
                    }

                    // reset doors
                    room.Exits = originalRoom.Exits;

                    //set room clean
                    room.Clean = true;

                    foreach (var player in room.Players)
                    {
                        _writeToClient.WriteLine("<p>The hairs on your neck stand up.</p>", player.ConnectionId);
                    }

                }

                foreach (var player in players)
                {
                    IdleCheck(player);
                    var hP = (_dice.Roll(1, 2, 5) * player.Level);
                    var mana = (_dice.Roll(1, 2, 5) * player.Level);
                    var moves = (_dice.Roll(1, 2, 5) * player.Level);
                    player.Attributes.Attribute[EffectLocation.Hitpoints] += GainAmount(hP, player);
                    player.Attributes.Attribute[EffectLocation.Mana] += GainAmount(mana, player);
                    player.Attributes.Attribute[EffectLocation.Moves] += GainAmount(moves, player);

                    if (player.Attributes.Attribute[EffectLocation.Hitpoints] > player.MaxAttributes.Attribute[EffectLocation.Hitpoints])
                    {
                        player.Attributes.Attribute[EffectLocation.Hitpoints] = player.MaxAttributes.Attribute[EffectLocation.Hitpoints];
                    }

                    if (player.Attributes.Attribute[EffectLocation.Mana] > player.MaxAttributes.Attribute[EffectLocation.Mana])
                    {
                        player.Attributes.Attribute[EffectLocation.Mana] = player.MaxAttributes.Attribute[EffectLocation.Mana];
                    }

                    if (player.Attributes.Attribute[EffectLocation.Moves] > player.MaxAttributes.Attribute[EffectLocation.Moves])
                    {
                        player.Attributes.Attribute[EffectLocation.Moves] = player.MaxAttributes.Attribute[EffectLocation.Moves];
                    }

                    _client.UpdateHP(player);
                    _client.UpdateMana(player);
                    _client.UpdateMoves(player);
                    _client.UpdateScore(player);

                }

            }
        }

        public async Task UpdateCombat()
        {
            // create a combat cache to add mobs too so they can fight back
            // block movement while fighting
            // end fight if target is not there / dead
            // create flee commant
            Console.WriteLine("started combat loop");
            while (true)
            {

                try
                {
                    await Task.Delay(4000);
                    Console.WriteLine("combat loop");

                    var players = _cache.GetCombatList();
                    var validPlayers = players.Where(x => x.Status == CharacterStatus.Status.Fighting);
                    Console.WriteLine("Number of fighters " + players.Count + " valid " + validPlayers.Count());
                    foreach (var player in validPlayers)
                    {
                        _combat.Fight(player, player.Target, _cache.GetRoom(player.RoomId), false);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public async Task UpdateWorldTime()
        {
            Console.WriteLine("started world time loop");
            while (true)
            {
                try
                {
                    await Task.Delay(500);
                   _time.DisplayTimeOfDayMessage(_time.UpdateTime());
  
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void IdleCheck(Player player)
        {
            var idleTime5Mins = player.LastCommandTime.AddMinutes(6) <= DateTime.Now;

            if (!player.Idle && idleTime5Mins)
            {
                _writeToClient.WriteLine("You enter the void.", player.ConnectionId);
                player.Idle = true;
                return;
            }

            var idleTime10Mins = player.LastCommandTime.AddMinutes(11) <= DateTime.Now;
            var idleTime15Mins = player.LastCommandTime.AddMinutes(16) <= DateTime.Now;
            if (idleTime10Mins && !idleTime15Mins)
            {
                _writeToClient.WriteLine("You go deeper into the void.", player.ConnectionId);
            }

            if (idleTime15Mins)
            {
                _core.Quit(player, _cache.GetRoom(player.RecallId));
            }
        }

        public async Task UpdateRoomEmote()
        {

            while (true)
            {
                try
                {
                    await Task.Delay(45000).ConfigureAwait(false);

                    var rooms = _cache.GetAllRooms();

                    if (!rooms.Any())
                    {
                        continue;
                    }

                    foreach (var room in rooms)
                    {
                        if (!room.Emotes.Any() || _dice.Roll(1, 1, 10) < 7)
                        {
                            continue;
                        }

                        var emote = room.Emotes[_dice.Roll(1, 0, room.Emotes.Count - 1)];

                        foreach (var player in room.Players)
                        {
                            _writeToClient.WriteLine($"<p class='room-emote'>{emote}</p>",
                                player.ConnectionId);
                        }
                    }
                }
                catch (Exception ex)
                {

                }

            }

        }

        public async Task UpdateMobEmote()
        {


            while (true)
            {
                try
                {
                    await Task.Delay(30000).ConfigureAwait(false);

                    var rooms = _cache.GetAllRooms().Where(x => x.Mobs.Any());

                    if (!rooms.Any())
                    {
                        continue;
                    }
                    var mobIds = new List<Guid>();
                    foreach (var room in rooms)
                    {

                        foreach (var mob in room.Mobs.Where(x => x.Status != CharacterStatus.Status.Fighting).ToList())
                        {
                            if (mob.Emotes.Any() && mob.Emotes[0] != null)
                            {

                                var emote = mob.Emotes[_dice.Roll(1, 0, mob.Emotes.Count - 1)];
                                foreach (var player in room.Players)
                                {
                                    //example mob emote: Larissa flicks through her journal.
                                    if (emote == null)
                                    {
                                        continue;
                                    }
                                    _writeToClient.WriteLine($"<p class='mob-emote'>{mob.Name} {emote}</p>",
                                        player.ConnectionId);
                                }
                            }

                            if (mobIds.Contains(mob.Id))
                            {
                                continue;
                            }


                            if (mob.Roam && _dice.Roll(1, 1, 100) >= 50)
                            {
                                var exits = Helpers.GetListOfExits(room.Exits);
                                if (exits != null)
                                {
                                    var direction = exits[_dice.Roll(1, 0, exits.Count - 1)];

                                    mob.Buffer.Enqueue(direction);
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

                                _commands.ProcessCommand(mobCommand, mob, room);
                            }

                            mobIds.Add(mob.Id);
                        }
                    }
                }
                catch (Exception ex)
                {

                }

            }

        }

        public async Task UpdatePlayers()
        {
            while (true)
            {

                try
                {
                    await Task.Delay(125);
                    var players = _cache.GetPlayerCache();
                    var validPlayers = players.Where(x => x.Value.Buffer.Count > 0);

                    foreach (var player in validPlayers)
                    {

                        var command = player.Value.Buffer.Dequeue();
                        var room = _cache.GetRoom(player.Value.RoomId);
                        player.Value.LastCommandTime = DateTime.Now;
                        _commands.ProcessCommand(command, player.Value, room);

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

    }


}