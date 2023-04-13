using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Combat;

namespace ArchaicQuestII.GameLogic.Loops
{
    public class CombatLoop : ILoop
    {
        public int TickDelay => 3200;

        public bool ConfigureAwait => false;

        private List<Fight> _combat;

        public void PreTick()
        {
            _combat = Services.Instance.Cache.GetCombatList();
        }

        public void Tick()
        {
            foreach (var combat in _combat)
            {
                combat.Do();
            }
        }

        public void PostTick()
        {
            _combat.Clear();
        }
    }
}
