using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.Commands.Character;

public interface ICharacterCommands
{
    /// <summary>
    /// Set health player should automatically flee
    /// Or disable automatic fleeing
    /// Or display current wimpy
    /// </summary>
    /// <param name="player">Player setting wimpy</param>
    /// <param name="health">Health amount to set wimpy to (0 disables)</param>
    void Wimpy(Player player, string health);
}