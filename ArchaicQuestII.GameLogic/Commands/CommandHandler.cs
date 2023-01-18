using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Emote;
using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands
{
    /// <summary>
    /// Handles all incoming player input
    /// </summary>
    public class CommandHandler : ICommandHandler
    {
        private readonly ICoreHandler _coreHandler;

        private readonly Dictionary<string, ICommand> _commands = new();
        private readonly ConcurrentDictionary<int, Skill.Model.Skill> _skillCache = new();
        private readonly Dictionary<string, Emote> _socials = new();
        private readonly ConcurrentDictionary<int, Help> _helpCache = new();
        
        public CommandHandler(ICoreHandler coreHandler, IDataBase dataBase)
        {
            _coreHandler = coreHandler;

            var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ICommand).IsAssignableFrom(p) && !p.IsInterface);

            foreach (var t in commandTypes)
            {
                var command = (ICommand)Activator.CreateInstance(t, _coreHandler);

                if (command == null) continue;

                foreach (var alias in command.Aliases)
                {
                    if (IsCommand(alias))
                        Helpers.LogError(dataBase,"CommandHandler.cs", "Duplicate Alias", ErrorPriority.Low);
                    else
                        AddCommand(alias, command);
                }
            }
        }
        
        public async Task Tick()
        {
            
        }

        #region CACHE

        public void AddCommand(string key, ICommand action)
        {
            _commands.Add(key, action);
        }

        public Dictionary<string, ICommand> GetCommands()
        {
            return _commands;
        }

        public bool IsCommand(string key)
        {
            return _commands.TryGetValue(key, out var c);
        }
        
        public ICommand GetCommand(string key)
        {
            _commands.TryGetValue(key, out var command);
            return command;
        }
        
        public void AddSocial(string key, Emote emote)
        {
            _socials.Add(key, emote);
        }

        public Dictionary<string, Emote> GetSocials()
        {
            return _socials;
        }
        
        public List<Skill.Model.Skill> GetAllSkills()
        {
            return _skillCache.Values.ToList();
        }
        
        public bool AddSkill(int id, Skill.Model.Skill skill)
        {
            return _skillCache.TryAdd(id, skill);
        }

        public Skill.Model.Skill GetSkill(int id)
        {
            _skillCache.TryGetValue(id == 0 ? 1 : id, out var skill);

            return skill;
        }

        public List<Skill.Model.Skill> ReturnSkills()
        {
            return _skillCache.Values.ToList();
        }
        
        public bool AddHelp(int id, Help help)
        {
            return _helpCache.TryAdd(id, help);
        }

        public Help GetHelp(int id)
        {
            _helpCache.TryGetValue(id, out var help);

            return help;
        }
        
        public List<Help> FindHelp(string id)
        {
            return _helpCache.Values.Where(x => x.Keywords.Contains(id, StringComparison.CurrentCultureIgnoreCase) && x.Deleted.Equals(false)).ToList();
        }
        
        #endregion

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

            var command = GetCommand(commandInput[0]);
            
            // Handle social emote that are entered by just typing the name such as smile or smile Harvey
            // here manipulate the command to add social in front of it so the social command is called.
            var social = GetSocials().Keys.FirstOrDefault(x => x.Equals(commandInput[0]));
            if (social != null)
            {
                var emoteTarget = ""; 
                
                if (commandInput.Length == 2)
                { 
                    emoteTarget = commandInput[1];
                }
                commandInput = new[] { "social", commandInput[0], emoteTarget};
                command = GetCommand(commandInput[0]);
            }
            

            if (command == null)
            {
                _coreHandler.Client.WriteLine("<p>{yellow}That is not a command.{yellow}</p>", player.ConnectionId);
                return;
            }

            if (player.UserRole < command.UserRole)
            {
                _coreHandler.Client.WriteLine("<p>{red}You dont have the required role to use that command.{/red}</p>", player.ConnectionId);
                return;
            }
            
            if (CheckStatus(player, command.DeniedStatus))
            {
                command.Execute(player, room, commandInput);
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
                    _coreHandler.Client.WriteLine("<p>{yellow}You can't do that while standing.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Sitting:
                    _coreHandler.Client.WriteLine("<p>{yellow}You can't do that while sitting.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Sleeping:
                    _coreHandler.Client.WriteLine("<p>{yellow}You can't do that while sleeping.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Fighting:
                    _coreHandler.Client.WriteLine("<p>{yellow}You can't do that while fighting.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Resting:
                    _coreHandler.Client.WriteLine("<p>{yellow}You can't do that while resting.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Incapacitated:
                    _coreHandler.Client.WriteLine("<p>{yellow}You can't do that while incapacitated.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Dead:
                    _coreHandler.Client.WriteLine("<p>{yellow}You can't do that while dead.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Ghost:
                    _coreHandler.Client.WriteLine("<p>{yellow}You can't do that while a ghost.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Busy:
                    _coreHandler.Client.WriteLine("<p>{yellow}You can't do that while busy.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Floating:
                    _coreHandler.Client.WriteLine("<p>{yellow}You can't do that while floating.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Mounted:
                    _coreHandler.Client.WriteLine("<p>{yellow}You can't do that while mounted.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Stunned:
                    _coreHandler.Client.WriteLine("<p>{yellow}You can't do that while stunned.{/}</p>", player.ConnectionId);
                    break;
                case CharacterStatus.Status.Fleeing:
                    _coreHandler.Client.WriteLine("<p>{yellow}You can't do that while fleeing.{/}</p>", player.ConnectionId);
                    break;
            }

            return false;
        }
        
        public bool TargetCheck(string target, Player player, string errorMessage = "What?")
        {
            if (!string.IsNullOrEmpty(target)) return true;
            _coreHandler.Client.WriteLine(errorMessage, player.ConnectionId);
            return false;
        }
    }
}
