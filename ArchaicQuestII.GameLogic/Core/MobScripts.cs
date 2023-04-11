using System;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.Core
{
    public class MyProxy
    {
        private Room room;
        private Player player;
        private Player mob;
        private string command;

        [MoonSharpHidden]
        public MyProxy(Room room)
        {
            this.room = room;
            //this.player = player;
            //this.mob = mob;
        }

        public Room GetRoom()
        {
            return room;
        }

        public Player GetPlayer()
        {
            return player;
        }

        public Player GetMob()
        {
            return mob;
        }

        public string GetCommand()
        {
            return command;
        }
    }

    public class ProxyPlayer
    {
        private Player player;

        [MoonSharpHidden]
        public ProxyPlayer(Player player)
        {
            this.player = player;
        }

        public Player GetPlayer()
        {
            return player;
        }
    }

    public class ProxyCommand
    {
        private string command;

        [MoonSharpHidden]
        public ProxyCommand(string command)
        {
            this.command = command;
        }

        public string getCommand()
        {
            return command;
        }
    }

    public class MobScripts : IMobScripts
    {
        public Player _player;
        public Player _mob;
        public Room _room;

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
            Services.Instance.Writer.WriteLine(
                $"<p class='mob-emote'>{n.Replace("#name#", player.Name)}</p>",
                player.ConnectionId,
                delay
            );
        }

        public void Say(string n, int delay, Room room, Player player, Player mob)
        {
            if (!IsInRoom(room, player))
            {
                return;
            }
            Services.Instance.Writer.WriteLine(
                $"<p class='mob-emote'>{mob.Name} says, \"{n}\"</p>",
                player.ConnectionId,
                delay
            );
        }

        public string GetName(Player player)
        {
            return player.Name;
        }

        public void UpdateInv(Player player)
        {
            player.Inventory.Add(
                new Item.Item()
                {
                    Name = "test",
                    Description = new Description() { Room = "A test LUA item" },
                    Id = 9999
                }
            );
        }

        public void AttackPlayer(Room room, Player player, Player mob)
        {
            Services.Instance.Combat.Fight(mob, GetName(player), room, true);
        }

        public void AddEventState(Player player, string key, int value)
        {
            player.EventState.TryAdd(key, value);
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
            return DiceBag.Roll(1, min, max);
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

        public int GetLevel(Player player)
        {
            return player.Level;
        }

        public bool IsMobHere(string name, Room room)
        {
            return room.Mobs.FirstOrDefault(
                    x => x.Name.StartsWith(name, StringComparison.CurrentCultureIgnoreCase)
                ) != null;
        }

        public bool IsObjectHere(string name, Room room)
        {
            return room.Items.FirstOrDefault(
                    x => x.Name.StartsWith(name, StringComparison.CurrentCultureIgnoreCase)
                ) != null;
        }

        public bool IsImm(Player player)
        {
            throw new NotImplementedException();
        }

        public void GiveItem(Player player, Player mob, string name)
        {
            var item = mob.Inventory.FirstOrDefault(
                x => x.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase)
            );

            if (item != null)
            {
                player.Inventory.Add(item);

                Services.Instance.UpdateClient.UpdateInventory(player);
            }
            else
            {
                // get item from cache
            }
        }

        public void RemoveItem(Player player, string name, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                var item = player.Inventory.FirstOrDefault(
                    x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)
                );
                if (item != null)
                {
                    player.Inventory.Remove(item);
                }
            }
            Services.Instance.UpdateClient.UpdateInventory(player);
        }

        public void GiveGold(int value, Player player)
        {
            if (player.Money == null)
            {
                player.Money = new Character.Model.Money();
            }
            player.Money.Gold += value;
        }

        public void Harm(int maxValue, Player player, Room room)
        {
            var damage = DiceBag.Roll(1, 1, maxValue);

            var dummyPlayer = new Player() { Name = "Script damage" };

            player.HarmTarget(damage);
            Services.Instance.UpdateClient.UpdateScore(player);
            Services.Instance.UpdateClient.UpdateHP(player);
            if (!player.IsAlive())
            {
                Services.Instance.Combat.TargetKilled(dummyPlayer, player, room);
            }
        }

        public bool HasObject(Player player, string name)
        {
            return player.Inventory.FirstOrDefault(
                    x => x.Name.StartsWith(name, StringComparison.CurrentCultureIgnoreCase)
                ) != null;
        }

        public bool HasObjectCount(Player player, string name, int count)
        {
            var countx = player.Inventory
                .FindAll(x => x.Name.StartsWith(name, StringComparison.CurrentCultureIgnoreCase))
                .Count();
            var x =
                player.Inventory
                    .FindAll(
                        x => x.Name.StartsWith(name, StringComparison.CurrentCultureIgnoreCase)
                    )
                    .Count() >= count;
            return x;
        }

        public bool Contains(string word, string expected)
        {
            var cleanWord = word.Substring(word.IndexOf(" ") + 1);
            return cleanWord.Contains(expected);
        }

        public bool StartsWith(string word, string expected)
        {
            return word.StartsWith(expected);
        }

        public void UpdateQuest(Player player, int questId, string message)
        {
            var quest = player.QuestLog.FirstOrDefault(x => x.Id == questId);

            if (quest != null)
            {
                quest.Description += $"\r\n{message}";
            }

            Services.Instance.Writer.WriteLine(
                $"<p class='gain'>Updated Quest: {quest.Title}!</p>",
                player.ConnectionId
            );
        }

        public void AddQuest(Player player, int questId)
        {
            var quest = Services.Instance.Cache.GetQuest(questId);

            if (player.QuestLog.FirstOrDefault(x => x.Id == quest.Id) == null)
            {
                player.QuestLog.Add(
                    new Quest()
                    {
                        Id = quest.Id,
                        Area = quest.Area,
                        Title = quest.Title,
                        Description = quest.Description,
                        Type = quest.Type,
                        ExpGain = quest.ExpGain,
                        GoldGain = quest.GoldGain,
                        MobsToKill = quest.MobsToKill,
                        ItemGain = quest.ItemGain,
                    }
                );
            }

            Services.Instance.Writer.WriteLine(
                $"<p class='gain'>New Quest: {quest.Title}!</p>",
                player.ConnectionId
            );
            Services.Instance.UpdateClient.UpdateQuest(player);
        }

        public void CompleteQuest(Player player, int questId)
        {
            var quest = player.QuestLog.FirstOrDefault(x => x.Id == questId);

            if (quest != null)
            {
                quest.Completed = true;
                Services.Instance.Writer.WriteLine(
                    $"<p class='improve'>Quest Complete: {quest.Title}!</p>",
                    player.ConnectionId
                );
                Services.Instance.Writer.WriteLine(
                    $"<p class='improve'>You gain {quest.ExpGain} experience points{(quest.GoldGain == 0 ? "." : $" and {quest.GoldGain} gold. ")}</p>",
                    player.ConnectionId
                );

                player.GainExperiencePoints(quest.ExpGain, out _);
                player.Money.Gold = quest.GoldGain;
            }

            Services.Instance.UpdateClient.UpdateQuest(player);
            Services.Instance.UpdateClient.UpdateExp(player);
            Services.Instance.UpdateClient.UpdateScore(player);
        }

        public void GainXP(Player player, int xp)
        {
            player.GainExperiencePoints(xp, out var message);
            Services.Instance.Writer.WriteLine($"message", player.ConnectionId);
            Services.Instance.UpdateClient.UpdateExp(player);
            Services.Instance.UpdateClient.UpdateScore(player);
        }

        public void Sleep(int milliseconds)
        {
            Task.Delay(milliseconds).Wait();
        }

        public void DoSkill(Player player, Player mob, Room room)
        {
            // TODO: FIX ME
            //   _spells.DoSpell("armour", mob, player.Name, room);
        }

        public void MobSay(string n, Room room, Player player, Player mob, int delay = 0)
        {
            if (!IsInRoom(room, player))
            {
                return;
            }

            var says = "says";
            if (n.EndsWith("!"))
            {
                says = "exclaims";
            }

            if (n.EndsWith("?"))
            {
                says = "wonders";
            }

            Services.Instance.Writer.WriteLine(
                $"<p class='mob'>{mob.Name} {says}, '{n.Replace("#name#", player.Name)}'</p>",
                player.ConnectionId,
                delay
            );
        }

        public void MobEmote(string n, Room room, Player player, int delay)
        {
            if (!IsInRoom(room, player))
            {
                return;
            }
            Services.Instance.Writer.WriteLine(
                $"<p class='mob-emote'>{n.Replace("#name#", player.Name)}</p>",
                player.ConnectionId,
                delay
            );
        }

        public void RemoveMobFromRoom(Player mob, Room room)
        {
            room.Mobs.Remove(mob);
        }

        public void Follow(Player player, Player mob)
        {
            mob.Following = player.Name;
            player.Followers.Add(mob);
        }

        public void UnFollow(Player player, Player mob)
        {
            player.Following = string.Empty;
            player.Followers.Remove(mob);
        }

        public bool CanFollow(Player player)
        {
            return player.Config.CanFollow;
        }

        public void UnFollow(Player player, Room room, string mobname)
        {
            var mob = room.Mobs.FirstOrDefault(x => x.Name.Equals(mobname));
            player.Following = string.Empty;
            player.Followers.Remove(mob);
        }

        public void KillMob(Player mob, Room room)
        {
            room.Mobs.Remove(mob);
        }

        public void KillMob(Room room, string mobname)
        {
            var mob = room.Mobs.FirstOrDefault(x => x.Name.Equals(mobname));
            room.Mobs.Remove(mob);
        }
    }
}
