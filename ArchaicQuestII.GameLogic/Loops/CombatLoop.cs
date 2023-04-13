using System;
using System.Linq;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Character.Status;

namespace ArchaicQuestII.GameLogic.Loops
{
    public class CombatLoop : ILoop
    {
        public int TickDelay => 3200;

        public bool ConfigureAwait => false;

        private List<Player> _combatants;

        public void PreTick()
        {
            _combatants = Services.Instance.Cache
                .GetCombatList()
                .Where(x => x.Status == CharacterStatus.Status.Fighting)
                .ToList();
        }

        public void Tick()
        {
            //Console.WriteLine("CombatLoop");

            foreach (var player in _combatants)
            {
                if (
                    player.Lag > 0
                    && !player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase)
                )
                {
                    player.Lag -= 1;
                    continue;
                }

                var attackCount = 1;

                var hasSecondAttack = player.HasSkill(SkillName.SecondAttack);

                var hasThirdAttack = player.HasSkill(SkillName.ThirdAttack);

                var hasForthAttack = player.HasSkill(SkillName.FourthAttack);

                var hasFithAttack = player.HasSkill(SkillName.FifthAttack);

                if (hasSecondAttack)
                {
                    attackCount += 1;
                }

                if (hasThirdAttack)
                {
                    attackCount += 1;
                }

                if (hasForthAttack)
                {
                    attackCount += 1;
                }

                if (hasFithAttack)
                {
                    attackCount += 1;
                }

                if (player.Affects.Haste)
                {
                    attackCount += 1;
                }

                for (var i = 0; i < attackCount; i++)
                {
                    Services.Instance.Combat.Fight(
                        player,
                        player.Target,
                        Services.Instance.Cache.GetRoom(player.RoomId),
                        false
                    );
                }
            }
        }

        public void PostTick()
        {
            _combatants.Clear();
        }
    }
}
