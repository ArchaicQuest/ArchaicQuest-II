using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Client;

public interface IClientHandler
{
    Task Tick();
    void WriteLineMobSay(string mobName, string message, string id);
    void WriteLine(string message, string id);
    void WriteLine(string message, string id, int delay);
    void WriteLine(string message);
    void WriteLineRoom(string message, Room room);
    void WriteToOthersInRoom(string message, Room room, Player player);
    /// <summary>
    /// Update HP UI
    /// </summary>
    /// <param name="player"></param>
    void UpdateHP(Player player);
    /// <summary>
    /// Update mana UI
    /// </summary>
    /// <param name="player"></param>
    void UpdateMana(Player player);
    /// <summary>
    /// Update moves UI
    /// </summary>
    /// <param name="player"></param>
    void UpdateMoves(Player player);
    /// <summary>
    /// Update Exp UI
    /// </summary>
    /// <param name="player"></param>
    void UpdateExp(Player player);
    void UpdateAffects(Player player);

    void UpdateEquipment(Player player);

    void UpdateInventory(Player player);

    void UpdateScore(Player player);
    void UpdateCommunication(Player player, string message, string type);

    void GetMap(Player player, string rooms);
    void UpdateQuest(Player player);

    void UpdateContentPopUp(Player player, WriteBook bookContent);

    void UpdateTime(Player player, Time time);
        
    void PlaySound(string soundName, Player player);
    void UpdateClientUI(Player player);
}