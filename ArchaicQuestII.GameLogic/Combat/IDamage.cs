using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.Core
{
    /// <summary>
    /// Display damage to player and victim
    /// </summary>
    public interface IDamage
    {
        public KeyValuePair<string, string> DamageText(int damage);
    }
}
