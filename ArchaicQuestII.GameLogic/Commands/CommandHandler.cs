using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.Commands
{
    /// <summary>
    /// Handles all incoming player input
    /// </summary>
    public class CommandHandler : ICommandHandler
    {
        public ICore Core { get; }
        public CommandHandler(ICore core)
        {
            Core = core;
            var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ICommand).IsAssignableFrom(p) && !p.IsInterface);

            foreach (var t in commandTypes)
            {
                var command = (ICommand)Activator.CreateInstance(t, Core);
                if (command == null) continue;

                foreach (var alias in command.Aliases)
                {
                    if (Core.Cache.IsCommand(alias))
                        Core.ErrorLog.Write("CommandHandler.cs", "Duplicate Alias", ErrorLog.Priority.Low);
                    else
                        Core.Cache.AddCommand(alias, command);
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
            var commandInput = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            commandInput[0] = commandInput[0].ToLower();

            var command = Core.Cache.GetCommand(commandInput[0]);
            
            // Handle social emote that are entered by just typing the name such as smile or smile Harvey
            // here manipulate the command to add social in front of it so the social command is called.
            var social = Core.Cache.GetSocials().Keys.FirstOrDefault(x => x.Equals(commandInput[0]));
            if (command == null && social != null)
            {
                var emoteTarget = ""; 
                
                if (commandInput.Length == 2)
                  { 
                      emoteTarget = commandInput[1];
                  }
                commandInput = new[] { "social", commandInput[0], emoteTarget};
                command = Core.Cache.GetCommand(commandInput[0]);
            }
            

            if (command == null)
            {
                Core.Writer.WriteLine("<p>{yellow}That is not a command.{yellow}</p>", player.ConnectionId);
                return;
            }

            if (player.UserRole < command.UserRole)
            {
                Core.Writer.WriteLine("<p>{red}You dont have the required role to use that command.{/}</p>", player.ConnectionId);
                return;
            }
            
            if (CheckSkillRequirements(player, command) && CheckStatus(player, command.DeniedStatus) )
            {
               

                try
                {
                    command.Execute(player, room, commandInput);

                    foreach (var mob in room.Mobs)
                    {

                        if (!string.IsNullOrEmpty(mob.Events.Act) && room.Players.Any())
                        {
                            var commandText = string.Join(" ", commandInput);
                            UserData.RegisterType<MobScripts>();

                            Script script = new Script();

                            DynValue obj = UserData.Create(Core.MobScripts);
                            script.Globals.Set("obj", obj);
                            UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
                            UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(player));
                            UserData.RegisterProxyType<ProxyCommand, string>(r => new ProxyCommand(commandText));


                            script.Globals["room"] = room;
                            script.Globals["command"] = commandText.ToLower();
                            script.Globals["player"] = player;
                            script.Globals["mob"] = mob;


                            DynValue res = script.DoString(mob.Events.Act);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Checks if the player can use the command with their current status
        /// </summary>
        /// <param name="player"></param>
        /// <param name="deniedlist"></param>
        /// <returns></returns>
        private bool CheckStatus(Player player, IEnumerable<CharacterStatus.Status> deniedlist)
        {
            if (deniedlist == null || !deniedlist.Contains(player.Status)) return true;
            
            switch (player.Status)
            {
                case CharacterStatus.Status.Standing:
                    Core.Writer.WriteLine("<p>{yellow}You can't do that while standing.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Sitting:
                    Core.Writer.WriteLine("<p>{yellow}You can't do that while sitting.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Sleeping:
                    Core.Writer.WriteLine("<p>{yellow}You can't do that while sleeping.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Fighting:
                    Core.Writer.WriteLine("<p>{yellow}You can't do that while fighting.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Resting:
                    Core.Writer.WriteLine("<p>{yellow}You can't do that while resting.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Incapacitated:
                    Core.Writer.WriteLine("<p>{yellow}You can't do that while incapacitated.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Dead:
                    Core.Writer.WriteLine("<p>{yellow}You can't do that while dead.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Ghost:
                    Core.Writer.WriteLine("<p>{yellow}You can't do that while a ghost.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Busy:
                    Core.Writer.WriteLine("<p>{yellow}You can't do that while busy.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Floating:
                    Core.Writer.WriteLine("<p>{yellow}You can't do that while floating.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Mounted:
                    Core.Writer.WriteLine("<p>{yellow}You can't do that while mounted.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Stunned:
                    Core.Writer.WriteLine("<p>{yellow}You can't do that while stunned.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Fleeing:
                    Core.Writer.WriteLine("<p>{yellow}You can't do that while fleeing.{/}</p>", player.ConnectionId);
                    break;
            }

            return false;
        }
        
        /// <summary>
        /// Checks if the player has the skill to use this command
        /// </summary>
        /// <param name="player"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private bool CheckSkillRequirements(Player player, ICommand command)
          {
              // Check if command uses ISkillCommand interface
              var isSkill = command.GetType().GetInterface("ISkillCommand");
              if (isSkill == null)
              {
                  return true;
              }
              
              // Check if player has skill
              if (player.Skills.FirstOrDefault(x => x.SkillName.Equals(command.Title)) == null)
              {
                  Core.Writer.WriteLine("You do not know that skill.");
                  return false;
              }
              
              // Check level requirements met
              if (player.Level < Core.Cache.GetClass(player.ClassName).Skills.FirstOrDefault(x => x.SkillName == command.Title)?.Level)
              {
                  Core.Writer.WriteLine("You are not skilled enough to use this skill");
                  return false;
              }

            return true;
        }
    }
}