using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands
{
    public interface ICommandHandler
    {
        public void HandleCommand(string key, string obj, string target, Player player, Room room);
    }
}
