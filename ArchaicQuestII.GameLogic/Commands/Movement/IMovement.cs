using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement
{
    public interface IMovement
    {
        void Move(Room room, Player character, string direction, bool silence, bool flee = false);

        void Flee(Room room, Player character, string direction);

        public void Sit(Player player, Room room, string target);
        public void Stand(Player player, Room room, string target);
        public void Sleep(Player player, Room room, string target);
        public void Wake(Player player, Room room, string target);
        public void Rest(Player player, Room room, string target);
        public void Follow(Player player, Room room, string target);
        public void ChangePlayerLocation(Player player, Room room);
        public void Group(Player player, Room room, string target);
        /// <summary>
        /// Entering portals
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        /// <param name="target"></param>
        public void Enter(Player player, Room room, string target);
        public void SetCharacterStatus(Player player, string longName, CharacterStatus.Status status);

        public void UpdateLightCondition(Player player, Room room);

    }
}
