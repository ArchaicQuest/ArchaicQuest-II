using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{
 public interface ICore
    {
        void Who(Player player);
        void Where(Player player, Room room);
        void QuestLog(Player player);
        void Recall(Player player, Room room);
        void Train(Player player, Room room, string stat);
    }
}
