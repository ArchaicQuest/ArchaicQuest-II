using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement
{
    public interface IMovement
    {
        void Move(Room room, Player character, string direction);

        void MoveNorth(Room room, Player character);
    }
}
