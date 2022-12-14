using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public interface IRoomActions
    {
        void Look(string target, Room room, Player player);
        void Look(string target, Room room, Player player, bool brief);
        void LookInContainer(string target, Room room, Player player);
        void LookInPortal(Item.Item portal, Room room, Player player);
        void ExamineObject(string target, Room room, Player player);
        void SmellObject(string target, Room room, Player player);
        void TasteObject(string target, Room room, Player player);
        void TouchObject(string target, Room room, Player player);
        string FindValidExits(Room room, bool showVerboseExits);
        string DisplayItems(Room room, Player player);
        string DisplayMobs(Room room, Player player);
        bool RoomIsDark(Room room, Player player);
        bool LoreSuccess(int? skillLevel);
        void RoomChange(Player player, Room oldRoom, Room newRoom);
    }
}
