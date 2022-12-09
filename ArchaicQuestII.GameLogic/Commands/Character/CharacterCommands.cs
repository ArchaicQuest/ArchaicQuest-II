using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Commands.Character;

/// <summary>
/// Handles Character Commands
/// </summary>
public class CharacterCommands : ICharacterCommands
{
    private readonly IWriteToClient _writer;

    public CharacterCommands(IWriteToClient writer)
    {
        _writer = writer;
    }
    
    /// <summary>
    /// Set health player should automatically flee
    /// Or disable automatic fleeing
    /// Or display current wimpy
    /// </summary>
    /// <param name="player">Player setting wimpy</param>
    /// <param name="health">Health amount to set wimpy to (0 disables)</param>
    public void Wimpy(Player player, string health)
    {
        var result = int.TryParse(health, out var wimpy);

        if (!result)
        {
            _writer.WriteLine($"Wimpy is set to {player.Config.Wimpy}", player.ConnectionId);
            return;
        }

        if (wimpy == 0)
        {
            player.Config.Wimpy = 0;
            _writer.WriteLine("Wimpy has been disabled.", player.ConnectionId);
            return;
        }
        
        if (wimpy > player.Stats.HitPoints / 3)
        {
            _writer.WriteLine("Wimpy cannot be set to more than 1/3 of your max hitpoints.", player.ConnectionId);
            return;
        }

        if (wimpy < 0)
        {
            _writer.WriteLine("Wimpy cannot be set to a negative.", player.ConnectionId);
            return;
        }
        
        player.Config.Wimpy = wimpy;
        _writer.WriteLine($"Wimpy set to {wimpy}.", player.ConnectionId);
    }
}