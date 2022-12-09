using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.World.Area
{
    public interface IAreaActions
    {
        void AreaInfo(Player player, Room.Room room);
        void AreaEntered(Player player, Room.Room room);
        void AreaConsider(Player player, Room.Room room);
        void AreaPopulation(Player player, Room.Room room);
    }
}