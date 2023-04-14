using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.Combat;

public class Combatant
{
    public Player player;
    public Player target;
    public string Command;
    public bool aggressor;

    public Combatant(Player character, bool attacker)
    {
        player = character;
        aggressor = attacker;
    }
}
