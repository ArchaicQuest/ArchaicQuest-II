﻿using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands
{
    /// <summary>
    /// Handles all incoming player input
    /// </summary>
    public class CommandHandler : ICommandHandler
    {
        private readonly ICore _core;

        public CommandHandler(ICore core)
        {
            _core = core;
            
            var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ICommand).IsAssignableFrom(p) && !p.IsInterface);

            foreach (var t in commandTypes)
            {
                var command = (ICommand)Activator.CreateInstance(t, _core);

                if (command == null) continue;

                foreach (var alias in command.Aliases)
                {
                    _core.Cache.AddCommand(alias, command);
                }
            }
        }

        /// <summary>
        /// Checks and processes commands
        /// </summary>
        /// <param name="input"></param>
        /// <param name="player"></param>
        /// <param name="room"></param>
        public void HandleCommand(Player player, Room room, string input)
        {
            var commandInput = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);;
            
            if (!_core.Cache.GetCommand(commandInput[0].ToLower(), out var command))
            {
                _core.Writer.WriteLine("That is not a command.", player.ConnectionId);
                return;
            }

            command.Execute(player, room, commandInput);
            
            //TODO: Fix role with Account update
            //if (player.Role >= command.UserRole)
            //    command.Execute(player, room, commandInput);
            //else
            //    _writeToClient.WriteLine("You dont have the required role.", player.ConnectionId);
        }
    }
}
