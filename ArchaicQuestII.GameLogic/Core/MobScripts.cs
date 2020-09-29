using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Effect;
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
        private readonly IDice _dice;
        private readonly IWriteToClient _writeToClient;
        public MobScripts(ICache cache, ICombat
            combat, IWriteToClient writeToClient, IDice dice)
        {
            _cache = cache;
            _combat = combat;
            _writeToClient = writeToClient;
            _dice = dice;

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
        public void Say(string n, int delay, Room room, Player player, Player mob)
        {
            if (!IsInRoom(room, player))
            {
                return;

            }
            _writeToClient.WriteLine($"<p class='mob-emote'>{mob.Name} says, \"{n}\"</p>", player.ConnectionId, delay);
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

        public int GetPlayerAttribute(Player player, string attribute)
        {
            return attribute switch
            {
                "str" => player.Attributes.Attribute[EffectLocation.Strength],
                "dex" => player.Attributes.Attribute[EffectLocation.Dexterity],
                "con" => player.Attributes.Attribute[EffectLocation.Constitution],
                "int" => player.Attributes.Attribute[EffectLocation.Intelligence],
                "wis" => player.Attributes.Attribute[EffectLocation.Wisdom],
                "cha" => player.Attributes.Attribute[EffectLocation.Charisma],
                "lvl" => player.Attributes.Attribute[EffectLocation.Level],
                "age" => player.Attributes.Attribute[EffectLocation.Age],
                "hp" => player.Attributes.Attribute[EffectLocation.Hitpoints],
                "mana" => player.Attributes.Attribute[EffectLocation.Mana],
                "mvs" => player.Attributes.Attribute[EffectLocation.Moves],
                "htr" => player.Attributes.Attribute[EffectLocation.HitRoll],
                "dmr" => player.Attributes.Attribute[EffectLocation.DamageRoll],
                "gen" => player.Attributes.Attribute[EffectLocation.Gender],
                "sas" => player.Attributes.Attribute[EffectLocation.SavingSpell],
                "wei" => player.Attributes.Attribute[EffectLocation.Weight],
                "hei" => player.Attributes.Attribute[EffectLocation.Height],
                "luc" => player.Attributes.Attribute[EffectLocation.Luck],
                _ => 0,
            };
        }

        public int Random(int min, int max)
        {
            return _dice.Roll(1, min, max);
        }

        public string GetClass(Player player)
        {
            return player.ClassName;
        }

        public bool IsPC(Player player)
        {
            return !player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase);
        }

        public bool IsMob(Player player)
        {
            return !IsPC(player);
        }

        public bool IsGood(Player player)
        {
            return player.AlignmentScore > 100;
        }

        public bool IsEvil(Player player)
        {
            return player.AlignmentScore < -100;
        }

        public bool IsNeut(Player player)
        {
            return player.AlignmentScore >= -100 && player.AlignmentScore <= 100;
        }

        public bool IsMobHere(string name, Room room)
        {
            return room.Mobs.FirstOrDefault(x => x.Name.StartsWith(name, StringComparison.CurrentCultureIgnoreCase)) !=
                   null;
        }

        public bool IsObjectHere(string name, Room room)
        {
            return room.Items.FirstOrDefault(x => x.Name.StartsWith(name, StringComparison.CurrentCultureIgnoreCase)) !=
                   null;
        }

        public bool IsImm(Player player)
        {
            throw new NotImplementedException();
        }

        public bool HasObject(Player player, string name)
        {
            return player.Inventory.FirstOrDefault(x => x.Name.StartsWith(name, StringComparison.CurrentCultureIgnoreCase)) !=
                   null;
        }
    }
}
