
namespace ArchaicQuestII.GameLogic.World.Room
{
    public interface IRoom
    {
        void UpdateRoom(Room data);
        Room GetRoom(int id);
        bool IsDark { get; }
    }
}
