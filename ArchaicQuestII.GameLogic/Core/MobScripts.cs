using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.Core
{

    public class MyProxy
    {
        private Room room;
        private Player player;
        private Player mob;
        [MoonSharpHidden]
        public MyProxy(Room room)
        {
            this.room = room;
            //this.player = player;
            //this.mob = mob;
        }

        public Room GetRoom() { return room; }
        public Player GetPlayer() { return player; }
        public Player GetMob() { return mob; }
    }


    public class ProxyPlayer
    {
        private Player player;
        [MoonSharpHidden]
        public ProxyPlayer(Player player)
        {
            this.player = player;
        }

        public Player GetPlayer() { return player; }

    }
    public class MobScripts: IMobScripts
    {
        public Player _player;
        public Player _mob;
        public Room _room;
        public ICombat _combat;
        private readonly ICache _cache;
        private readonly IWriteToClient _writeToClient;
        public MobScripts(ICache cache, ICombat
            combat, IWriteToClient writeToClient)
        {
            _cache = cache;
            _combat = combat;
            _writeToClient = writeToClient;

        }
        public bool IsInRoom(Room room, Player player)
        {
            return room.Players.Contains(player);
        }
        public void Say(string n, int delay, Room room, Player player)
        {
            if (!IsInRoom(room, player))
            {
                return;

            }
            _writeToClient.WriteLine($"<p class='mob-emote'>{n}</p>", player.ConnectionId, delay);
        }
        public string GetName(Player player)
        {

            return player.Name;
        }

        public void UpdateInv(Player player)
        {
            player.Inventory.Add(new Item.Item() { Name = "test", Description = new Description() { Room = "A test LUA item" }, Id = 9999 });
        }

        public void AttackPlayer(Room room, Player player, Player mob)
        {
            _combat.Fight(mob, GetName(player), room, true);
        }

        public void AddEventState(Player player, string key, int value)
        {
            player.EventState.Add(key, value);
        }

        public void UpdateEventState(Player player, string key, int value)
        {
            player.EventState[key] = value;
        }


        public int ReadEventState(Player player, string key)
        {
            player.EventState.TryGetValue(key, out var state);
            return state;
        }

        public bool HasEventState(Player player, string key)
        {
            return player.EventState.ContainsKey(key);
        }
    }
}
