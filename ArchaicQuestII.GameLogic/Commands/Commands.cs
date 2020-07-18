using ArchaicQuestII.GameLogic.Character;
using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Commands.Movement;
using ArchaicQuestII.GameLogic.World.Room;
using System.Linq;
using ArchaicQuestII.GameLogic.Commands.Debug;
using ArchaicQuestII.GameLogic.Commands.Skills;
using ArchaicQuestII.GameLogic.Spell.Interface;

namespace ArchaicQuestII.GameLogic.Commands
{
   public class Commands: ICommands
    {
        private readonly IMovement _movement;
        private readonly ISkills _skills;
        private readonly ISpells _spells;
        private readonly IRoomActions _roomActions;
        private readonly IDebug _debug;

        public Commands(IMovement movement, IRoomActions roomActions, IDebug debug, ISkills skills, ISpells spells)
        {
            _movement = movement;
            _roomActions = roomActions;
            _debug = debug;
            _skills = skills;
            _spells = spells;
        }
 
        public void CommandList(string key, string obj, string target, Player player, Room room)
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
                case "look":
                case "l":
                    _roomActions.Look(room, player);
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
            }
        }    

        public void ProcessCommand(string command, Player player, Room room)
        {

            var cleanCommand = command.Trim().ToLower();
            var commandParts = cleanCommand.Split(' ');
           var parameters = MakeCommandPartsSafe(commandParts);
            CommandList(commandParts[0], parameters.Item1,  parameters.Item2, player, room);
        }

        public Tuple<string, string> MakeCommandPartsSafe(string[] commands)
        {

            var cmdCount = commands.Length;

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

            return null;
        }

    }


}

