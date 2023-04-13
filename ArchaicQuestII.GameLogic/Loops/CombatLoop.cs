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
                    && player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase)
                )
                {
                    player.Lag -= 1;
                    continue;
                }

                var attackCount = 1;

                var hasSecondAttack = player.Skills.FirstOrDefault(
                    x => x.Name == SkillName.SecondAttack
                );
                if (hasSecondAttack != null)
                {
                    hasSecondAttack =
                        player.Level >= hasSecondAttack.Level ? hasSecondAttack : null;
                }

                var hasThirdAttack = player.Skills.FirstOrDefault(
                    x => x.Name == SkillName.ThirdAttack
                );
                if (hasThirdAttack != null)
                {
                    hasThirdAttack = player.Level >= hasThirdAttack.Level ? hasThirdAttack : null;
                }

                var hasFouthAttack = player.Skills.FirstOrDefault(
                    x => x.Name == SkillName.FourthAttack
                );
                if (hasFouthAttack != null)
                {
                    hasFouthAttack = player.Level >= hasFouthAttack.Level ? hasFouthAttack : null;
                }
                var hasFithAttack = player.Skills.FirstOrDefault(
                    x => x.Name == SkillName.FifthAttack
                );

                if (hasFithAttack != null)
                {
                    hasFithAttack = player.Level >= hasFithAttack.Level ? hasFithAttack : null;
                }

                if (hasSecondAttack != null)
                {
                    attackCount += 1;
                }

                if (hasThirdAttack != null)
                {
                    attackCount += 1;
                }

                if (hasFouthAttack != null)
                {
                    attackCount += 1;
                }

                if (hasFithAttack != null)
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
