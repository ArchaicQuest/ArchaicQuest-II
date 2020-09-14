using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Commands
{
    public interface ICommands
    {
        void CommandList(string key, string obj, string target, Player player, Room room); 
        void ProcessCommand(string command, Player player, Room room);
    }
}
