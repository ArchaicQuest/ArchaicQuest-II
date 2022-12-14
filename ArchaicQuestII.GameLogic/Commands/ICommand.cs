using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
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
        /// Player role required to execute command
        /// </summary>
        UserRole UserRole { get; }
        
        /// <summary>
        /// Stored writer
        /// </summary>
        IWriteToClient Writer { get; }
        
        /// <summary>
        /// Cached items
        /// </summary>
        ICache Cache { get; }
        
        /// <summary>
        /// Client update
        /// </summary>
        IUpdateClientUI UpdateClient { get; }
        
        IRoomActions RoomActions { get; }

        /// <summary>
        /// Executes the command
        /// </summary>
        void Execute(Player player, Room room, string[] input);
    }
}