using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// Command Aliases
        /// </summary>
        string[] Aliases { get;}

        /// <summary>
        /// Command Description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// How to use command
        /// </summary>
        string[] Usages { get; }
        
        /// <summary>
        /// Optional, set title for help file,
        /// if not set, uses target as default help title
        /// </summary>
        string Title { get;  }
        
        CharacterStatus.Status[] DeniedStatus { get; }

        /// <summary>
        /// Player role required to execute command
        /// </summary>
        UserRole UserRole { get; }
        
        /// <summary>
        /// Holds all the Handlers
        /// </summary>
        ICoreHandler Handler { get; }

        /// <summary>
        /// Executes the command
        /// </summary>
        void Execute(Player player, Room room, string[] input);
    }
}