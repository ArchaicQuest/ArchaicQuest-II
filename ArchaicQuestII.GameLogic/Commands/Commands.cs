using ArchaicQuestII.GameLogic.Character;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Commands.Movement;
using ArchaicQuestII.GameLogic.World.Room;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.Character.MobFunctions;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Commands.Communication;
using ArchaicQuestII.GameLogic.Commands.Debug;
using ArchaicQuestII.GameLogic.Commands.Inventory;
using ArchaicQuestII.GameLogic.Commands.Objects;
using ArchaicQuestII.GameLogic.Commands.Score;
using ArchaicQuestII.GameLogic.Commands.Skills;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Socials;
using ArchaicQuestII.GameLogic.Spell.Interface;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.Commands
{
    public class Commands : ICommands
    {

        private readonly IMovement _movement;
        private readonly ISkills _skills;
        private readonly ISpells _spells;
        private readonly IRoomActions _roomActions;
        private readonly IDebug _debug;
        private readonly IObject _object;
        private readonly IInventory _inventory;
        private readonly Icommunication _communication;
        private readonly IEquip _equipment;
        private readonly IScore _score;
        private readonly ICombat _combat;
        private readonly ICache _cache;
        private readonly ISocials _socials;
        private readonly ICommandHandler _commandHandler;
        private readonly ICore _core;
        private readonly IMobFunctions _mobFunctions;
        private readonly IHelp _help;
        private readonly IMobScripts _mobScripts;

        public Commands(
            IMovement movement,
            IRoomActions roomActions,
            IDebug debug,
            ISkills skills,
            ISpells spells,
            IObject objects,
            IInventory inventory,
            Icommunication communication,
            IEquip equipment,
            IScore score,
            ICombat combat,
            ICache cache,
            ISocials socials,
            ICommandHandler commandHandler,
            ICore core,
            IMobFunctions mobFunctions,
            IHelp help,
            IMobScripts mobScripts
            )
        {
            _movement = movement;
            _roomActions = roomActions;
            _debug = debug;
            _skills = skills;
            _spells = spells;
            _object = objects;
            _inventory = inventory;
            _communication = communication;
            _equipment = equipment;
            _score = score;
            _combat = combat;
            _cache = cache;
            _socials = socials;
            _commandHandler = commandHandler;
            _core = core;
            _mobFunctions = mobFunctions;
            _help = help;
            _mobScripts = mobScripts;
        }
 
        public void CommandList(string key, string obj, string target, string fullCommand, Player player, Room room)
        {
            switch (key)
            {
                case "north west":
                case "nw":
                    _movement.Move(room, player, "North West");
                    break;
                case "north east":
                case "ne":
                    _movement.Move(room, player, "North East");
                    break;
                case "south east":
                case "se":
                    _movement.Move(room, player, "South East");
                    break;
                case "south west":
                case "sw":
                    _movement.Move(room, player, "South West");
                    break;
                case "north":
                case "n":
                    _movement.Move(room, player, "North");
                    break;
                case "east":
                case "e":
                    _movement.Move(room, player, "East");
                    break;
                case "south":
                case "s":
                    _movement.Move(room, player, "South");
                    break;
                case "west":
                case "w":
                    _movement.Move(room, player, "West");
                    break;
                case "up":
                case "u":
                    _movement.Move(room, player, "Up");
                    break;
                case "down":
                case "d":
                    _movement.Move(room, player, "Down");
                    break;
                case "flee":
                case "fle":
                    _movement.Flee(room, player, "");
                    break;
                case "look":
                case "l":
                    _roomActions.Look(obj, room, player);
                    break;
                case "look in":
                case "l in":
                    _roomActions.LookInContainer(obj, room, player);
                    break;
                case "examine":
                case "exam":
                    _roomActions.ExamineObject(obj, room, player);
                    break;
                case "taste":
                case "lick":
                    _roomActions.TasteObject(obj, room, player);
                    break;
                case "touch":
                case "feel":
                    _roomActions.TouchObject(obj, room, player);
                    break;
                case "smell":
                    _roomActions.SmellObject(obj, room, player);
                    break;
                case "i":
                case "inv":
                case "inventory":
                    _inventory.List(player);
                    break;
                case "close":
                    _object.Close(obj, room, player);
                    break; ;
                case "open":
                    _object.Open(obj, room, player);
                    break;
                case "give":
                case "hand":
                    _object.Give(obj, target, room, player, fullCommand);
                    break;
                case "loot":
                case "get":
                case "take":
                    _object.Get(obj, target, room, player);
                    break;
                case "drop":
                case "put":
                    _object.Drop(obj, target, room, player, fullCommand);
                    break;
                case "cast":
                case "c":
                    _spells.DoSpell(obj, player, target, room);
                    break;
                case "skill":
                case "skills":
                case "slist":
                case "spells":
                    _skills.ShowSkills(player);
                    break;
                case "/debug":
                    _debug.DebugRoom(room, player);
                    break;
                 case "newbie":
                     _communication.Newbie(obj, room, player);
                    break;
                case "ooc":
                    _communication.OOC(obj, room, player);
                    break;
                case "gossip":
                    _communication.Gossip(obj, room, player);
                    break;
                case "say":
                case "'":
                    _communication.Say(obj, room, player);
                    break;
                case "sayto":
                case ">":
                    _communication.SayTo(obj, target, room, player);
                    break;
                case "yell":
                    _communication.Yell(obj, room, player);
                    break;
                case "tell":
                    _communication.Tells(obj, target, player);
                    break;
                case "reply":
                    _communication.Reply(obj, player);
                    break;
                case "wear":
                    _equipment.Wear(obj, room, player);
                    break;
                case "eq":
                case "equipment":
                    _equipment.ShowEquipment(player);
                    break;
                case "score":
                    _score.DisplayScore(player);
                    break;
                case "kill":
                case "k":
                    _combat.Fight(player, obj, room, false);
                    break;
                case "sit":
                    _movement.Sit(player, room, obj);
                    break;
                case "stand":
                case "st":
                    _movement.Stand(player, room, obj);
                    break;
                case "sleep":
                case "sl":
                    _movement.Sleep(player, room, obj);
                    break;
                case "wake":
                case "wa":
                    _movement.Wake(player, room, obj);
                    break;
                case "rest":
                case "re":
                    _movement.Rest(player, room, obj);
                    break;
                case "social":
                case "socials":
                case "soc":
                    _socials.DisplaySocials(player);
                    break;
                case "follow":
                    _movement.Follow(player, room, obj);
                    break;
                case "group":
                    _movement.Group(player, room, obj);
                    break;
                case "who":
                case "wh":
                    _core.Who(player);
                    break;
                case "where":
                case "whe":
                    _core.Where(player, room);
                    break;
                case "con":
                case "consider":
                    _combat.Consider(player,obj, room);
                    break;
                case "ql":
                case "questlog":
                    _core.QuestLog(player);
                    break;
                case "unlock":
                    _object.Unlock(obj, room, player);
                    break;
                case "lock":
                    _object.Lock(obj, room, player);
                    break;
                case "list":
                case "li":
                    _mobFunctions.List(room, player);
                    break;
                case "buy":
                case "by":
                case "b":
                    _mobFunctions.BuyItem(obj, room, player);
                    break;
                case "sell":
                    _mobFunctions.SellItem(obj, room, player);
                    break; 
                case "inspect":
                case "ins":
                    _mobFunctions.InspectItem(obj, room, player);
                    break;
                case "help":
                    _help.DisplayHelpFile(obj, player);
                    break;
                case "recall":
                case "reca":
                case "rc":
                    _core.Recall(player, room);
                    break;
                case "train":
                    _core.Train(player, room, obj);
                    break;
                case "save":
                    _core.Save(player);
                    break;
                default:
                        _commandHandler.HandleCommand(key,obj,target, player, room);
                    break;

            }
        }

 

        public void ProcessCommand(string command, Player player, Room room)
        {

            var cleanCommand = command;
            var commandParts = cleanCommand.Split(' ');
            var key = commandParts[0].ToLower();
            
            
            if (commandParts.Length >= 2)
            {
                if (commandParts[1].ToLower() == "in")
                {
                    key = commandParts[0] + " " + commandParts[1].ToLower();
                    commandParts = commandParts.Where(x => x != "in").ToArray();
                }
            }
            var parameters = MakeCommandPartsSafe(commandParts);

            try
            {
                foreach (var mob in room.Mobs)
                {

                    if (!string.IsNullOrEmpty(mob.Events.Act))
                    {
                        UserData.RegisterType<MobScripts>();

                        Script script = new Script();

                        DynValue obj = UserData.Create(_mobScripts);
                        script.Globals.Set("obj", obj);
                        UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
                        UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(player));
                        UserData.RegisterProxyType<ProxyCommand, string>(r => new ProxyCommand(command));


                        script.Globals["room"] = room;
                        script.Globals["command"] = command;
                        script.Globals["player"] = player;
                        script.Globals["mob"] = mob;


                        DynValue res = script.DoString(mob.Events.Act);
                    }
                }
            }
            catch(Exception ex)
            {

            }

            CommandList(key, parameters.Item1, parameters.Item2, cleanCommand, player, room);
        }

        public Tuple<string, string> MakeCommandPartsSafe(string[] commands)
        {

            var cmdCount = commands.Length;

            if (commands[0].ToLower() == "say" || commands[0] == "'")
            {
                var say = string.Join(" ", commands);

                if (commands[0].ToLower() == "say")
                {
                    say = say.Remove(0, 4);
                }
                else
                {
                    say = say.Remove(0, 2);
                }
                return new Tuple<string, string>(say, string.Empty);
            }

            if (commands[0].ToLower() == "sayto" || commands[0] == ">")
            {
                var say = string.Join(" ", commands);

                say = say.Remove(0, commands[0].Length + 1 + commands[1].Length);

                return new Tuple<string, string>(say, commands[1]);

            }

            if (commands[0].ToLower() == "yell")
            {
                var say = string.Join(" ", commands);

                say = say.Remove(0, 5);
                return new Tuple<string, string>(say, string.Empty);
            }

            if (commands[0].ToLower() == "newbie")
            {
                var say = string.Join(" ", commands);

                say = say.Remove(0, 7);
                return new Tuple<string, string>(say, string.Empty);
            }

            if (commands[0].ToLower() == "ooc")
            {
                var say = string.Join(" ", commands);

                say = say.Remove(0, 4);
                return new Tuple<string, string>(say, string.Empty);
            }

            if (commands[0].ToLower() == "gossip")
            {
                var say = string.Join(" ", commands);

                say = say.Remove(0, 7);
                return new Tuple<string, string>(say, string.Empty);
            }

            if (commands[0].ToLower() == "reply")
            {
                var say = string.Join(" ", commands);

                say = say.Remove(0, 6);
                return new Tuple<string, string>(say, string.Empty);
            }

            if (commands[0].ToLower() == "tell")
            {
                var say = string.Join(" ", commands);

                say = say.Remove(0, 5);
                return new Tuple<string, string>(commands[1], say.Remove(0, commands[1].Length+1));
            }

            if (cmdCount == 1)
            {
                return new Tuple<string, string>(commands[0], string.Empty);
            }

            if (cmdCount == 2)
            {
                return new Tuple<string, string>(commands[1], string.Empty);
            }

            if (cmdCount == 3)
            {
                return new Tuple<string, string>(commands[1], commands[2]);
            }

            if (cmdCount > 3)
            {
                return new Tuple<string, string>(commands[1], commands[2]);
            }

            return null;
        }

    }


}

