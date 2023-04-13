using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;
using MoonSharp.Interpreter;
using Newtonsoft.Json;

namespace ArchaicQuestII.GameLogic.Loops
{
    public class RoomLoop : ILoop
    {
        public int TickDelay => 125;
        public bool ConfigureAwait => false;
        private List<Player> _currentPlayers = new List<Player>();
        private List<Room> _rooms = Services.Instance.Cache.GetAllRoomsToRepop();
        private int _emoteTick = 36;
        private int _corpsesTick = 960;
        private int _mobsTick = 960;
        private int _itemsTick = 960;
        private int _resetTick = 960;

        public void PreTick()
        {
            _corpsesTick--;
            _emoteTick--;
            _mobsTick--;
            _itemsTick--;
            _resetTick--;
        }

        public void Tick()
        {
            foreach (var room in _rooms)
            {
                //do an emote in room
                if (_emoteTick <= 0)
                    Emote(room);

                //update all corpses in room
                if (_corpsesTick <= 0)
                    Corpse(room);

                //update all mobs in room
                if (_mobsTick <= 0)
                    Mobs(room);

                //update any items in room
                if (_itemsTick <= 0)
                    Items(room);

                //reset the room
                if (_resetTick <= 0)
                    Reset(room);

                //check for new players in room
                foreach (var player in room.Players)
                {
                    if (player.ConnectionId == "mob")
                        continue;

                    if (_currentPlayers.Contains(player) && !room.Players.Contains(player))
                        PlayerLeft(room, player);
                    if (!_currentPlayers.Contains(player) && room.Players.Contains(player))
                        PlayerEntered(room, player);
                }
            }
        }

        public void PostTick() { }

        private void Reset(Room room)
        {
            // reset doors
            room.Exits = room.Exits;

            //set room clean
            // if (room.Players.Count == 0)
            //   {
            room.Clean = true;

            foreach (var player in room.Players)
            {
                Services.Instance.Writer.WriteLine(
                    "<p>The hairs on your neck stand up.</p>",
                    player
                );
            }
            //  }

            _resetTick = 960;
        }

        private void Items(Room room)
        {
            foreach (var item in room.Items)
            {
                // need to check if item exists before adding
                var itemExist = room.Items.FirstOrDefault(x => x.Id.Equals(item.Id));
                var itemCount = room.Items.FindAll(x => x.Id.Equals(item.Id));
                var originalItemCount = room.Items.FindAll(x => x.Id.Equals(item.Id));

                if (itemCount.Count < originalItemCount.Count)
                {
                    room.Items.Add(item);
                }

                if (
                    itemExist != null && itemExist.ItemType == Item.Item.ItemTypes.Container
                    || itemExist.ItemType == Item.Item.ItemTypes.Forage
                )
                {
                    itemExist.Container.IsOpen = item.Container.IsOpen;
                    itemExist.Container.IsLocked = item.Container.IsLocked;
                    itemExist.Container.GoldPieces = item.Container.GoldPieces;
                    foreach (var items in item.Container.Items)
                    {
                        if (itemExist.Container.Items.Count < item.Container.Items.Count)
                        {
                            itemExist.Container.Items.Add(items);
                        }
                    }
                }
            }

            _itemsTick = 960;
        }

        private void Mobs(Room room)
        {
            //max 187MB allocated type: string too much memory used here
            var originalRoom = JsonConvert.DeserializeObject<Room>(
                JsonConvert.SerializeObject(
                    Services.Instance.Cache.GetOriginalRoom(Helpers.ReturnRoomId(room))
                )
            );

            foreach (var mob in originalRoom.Mobs)
            {
                var mobExist = _rooms
                    .Find(x => x.Mobs.Any(y => y.UniqueId.Equals(mob.UniqueId)))
                    ?.Mobs.FirstOrDefault(z => z.UniqueId.Equals(mob.UniqueId));

                if (mobExist == null)
                {
                    room.Mobs.Add(mob);
                }
                else
                {
                    if (mob.Status != CharacterStatus.Status.Fighting)
                    {
                        mobExist.Attributes.Attribute[EffectLocation.Hitpoints] +=
                            DiceBag.Roll(1, 2, 5) + mobExist.Level;
                        mobExist.Attributes.Attribute[EffectLocation.Mana] +=
                            DiceBag.Roll(1, 2, 5) + mobExist.Level;
                        mobExist.Attributes.Attribute[EffectLocation.Moves] +=
                            DiceBag.Roll(1, 2, 5) + mobExist.Level;
                    }
                    else
                    {
                        mobExist.Attributes.Attribute[EffectLocation.Hitpoints] +=
                            (DiceBag.Roll(1, 1, 5) + mobExist.Level) / 2;
                        mobExist.Attributes.Attribute[EffectLocation.Mana] +=
                            (DiceBag.Roll(1, 1, 5) + mobExist.Level) / 2;
                        mobExist.Attributes.Attribute[EffectLocation.Moves] +=
                            (DiceBag.Roll(1, 1, 5) + mobExist.Level) / 2;
                    }

                    if (
                        mobExist.Attributes.Attribute[EffectLocation.Hitpoints]
                        > mobExist.MaxAttributes.Attribute[EffectLocation.Hitpoints]
                    )
                    {
                        mobExist.Attributes.Attribute[EffectLocation.Hitpoints] = mobExist
                            .MaxAttributes
                            .Attribute[EffectLocation.Hitpoints];
                    }

                    if (
                        mobExist.Attributes.Attribute[EffectLocation.Mana]
                        > mobExist.MaxAttributes.Attribute[EffectLocation.Mana]
                    )
                    {
                        mobExist.Attributes.Attribute[EffectLocation.Mana] = mobExist
                            .MaxAttributes
                            .Attribute[EffectLocation.Mana];
                    }

                    if (
                        mobExist.Attributes.Attribute[EffectLocation.Moves]
                        > mobExist.MaxAttributes.Attribute[EffectLocation.Moves]
                    )
                    {
                        mobExist.Attributes.Attribute[EffectLocation.Moves] = mobExist
                            .MaxAttributes
                            .Attribute[EffectLocation.Moves];
                    }
                }
            }

            _mobsTick = 960;
        }

        private void Emote(Room room)
        {
            if (!room.Emotes.Any() || !_currentPlayers.Any())
                return;

            if (DiceBag.Roll(1, 1, 10) < 7)
                return;

            var emote = room.Emotes[DiceBag.Roll(1, 0, room.Emotes.Count - 1)];

            foreach (var player in room.Players)
            {
                Services.Instance.Writer.WriteLine($"<p class='room-emote'>{emote}</p>", player);
            }

            _emoteTick = 32;
        }

        private void Corpse(Room room)
        {
            //get corpse and remove
            var corpses = room.Items.FindAll(
                x =>
                    x.Description.Room.Contains("corpse", StringComparison.CurrentCultureIgnoreCase)
            );

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
                        corpse.Description.Room =
                            $"{corpse.Name.ToLower()} fills the air with a foul stench.";
                        break;
                    case 2:
                        corpse.Description.Room =
                            $"{corpse.Name.ToLower()} is crawling with vermin.";
                        break;
                    case 1:
                        corpse.Description.Room =
                            $"{corpse.Name.ToLower()} is in the last stages of decay.";
                        break;
                    case 0:
                        foreach (var pc in room.Players)
                        {
                            Services.Instance.Writer.WriteLine(
                                $"<p>A quivering horde of maggots consumes {corpse.Name.ToLower()}.</p>",
                                pc
                            );
                        }
                        room.Items.Remove(corpse);
                        break;
                    default:
                        corpse.Description.Room = $"{corpse.Name.ToLower()} lies here.";
                        break;
                }

                corpse.Decay--;
            }

            _corpsesTick = 960;
        }

        private void PlayerEntered(Room room, Player player)
        {
            _currentPlayers.Add(player);

            foreach (var mob in room.Mobs.ToList())
            {
                if (!string.IsNullOrEmpty(mob.Events.Enter))
                {
                    try
                    {
                        UserData.RegisterType<MobScripts>();

                        var script = new Script();

                        var obj = UserData.Create(Services.Instance.MobScripts);
                        script.Globals.Set("obj", obj);
                        UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
                        UserData.RegisterProxyType<ProxyPlayer, Player>(
                            r => new ProxyPlayer(player)
                        );

                        script.Globals["room"] = room;
                        script.Globals["player"] = player;
                        script.Globals["mob"] = mob;

                        var res = script.DoString(mob.Events.Enter);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("RoomActions.cs: " + ex);
                    }
                }

                if (mob.Aggro && mob.Status != CharacterStatus.Status.Fighting)
                {
                    Services.Instance.Writer.WriteLine($"{mob.Name} attacks you!", player);
                    Services.Instance.MobScripts.AttackPlayer(room, player, mob);
                }
            }
        }

        private void PlayerLeft(Room room, Player player)
        {
            _currentPlayers.Remove(player);

            foreach (var mob in room.Mobs.Where(mob => !string.IsNullOrEmpty(mob.Events.Leave)))
            {
                UserData.RegisterType<MobScripts>();

                var script = new Script();

                var obj = UserData.Create(Services.Instance.MobScripts);
                script.Globals.Set("obj", obj);
                UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
                UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(player));

                script.Globals["room"] = room;
                script.Globals["player"] = player;
                script.Globals["mob"] = mob;

                var res = script.DoString(mob.Events.Leave);
            }
        }
    }
}
