using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Commands;

namespace ArchaicQuestII.GameLogic.Loops
{
	public class CombatLoop : ILoop
	{
        public int TickDelay => 3200;

        public bool ConfigureAwait => true;

        private ICore _core;
        private List<Player> _combatants;

        public void Init(ICore core, ICommandHandler commandHandler)
        {
            _core = core;
        }

        public void PreTick()
        {
            _combatants = _core.Cache.GetCombatList().Where(x => x.Status == CharacterStatus.Status.Fighting).ToList();
        }

        public void Tick()
        {
            foreach (var player in _combatants)
            {
                if (player.Lag > 0 &&
                    player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
                {
                    player.Lag -= 1;
                    continue;
                }

                var attackCount = 1;

                var hasSecondAttack = player.Skills.FirstOrDefault(x =>
                    x.SkillName.Equals("Second Attack", StringComparison.CurrentCultureIgnoreCase));
                if (hasSecondAttack != null)
                {
                    hasSecondAttack = player.Level >= hasSecondAttack.Level ? hasSecondAttack : null;
                }

                var hasThirdAttack = player.Skills.FirstOrDefault(x =>
                    x.SkillName.Equals("Third Attack", StringComparison.CurrentCultureIgnoreCase));
                if (hasThirdAttack != null)
                {
                    hasThirdAttack = player.Level >= hasThirdAttack.Level ? hasThirdAttack : null;
                }

                var hasFouthAttack = player.Skills.FirstOrDefault(x =>
                    x.SkillName.Equals("Fourth Attack", StringComparison.CurrentCultureIgnoreCase));
                if (hasFouthAttack != null)
                {
                    hasFouthAttack = player.Level >= hasFouthAttack.Level ? hasFouthAttack : null;
                }
                var hasFithAttack = player.Skills.FirstOrDefault(x =>
                    x.SkillName.Equals("Fifth Attack", StringComparison.CurrentCultureIgnoreCase));

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
                    _core.Combat.Fight(player, player.Target, _core.Cache.GetRoom(player.RoomId), false);
                }

            }
        }

        public void PostTick()
        {
            _combatants.Clear();
        }
    }
}

