using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Combat.Loops;

public class UpdateCombat : IGameLoop
{
    public int TickDelay => 3200;
    public ICoreHandler Handler { get; set; }
    public bool Enabled { get; set; }

    public void Loop()
    {
        var players = Handler.Combat.GetCombatList();
        var validPlayers = players.Where(x => x.Status == CharacterStatus.Status.Fighting);

        foreach (var player in validPlayers)
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
                x.SkillName.Equals("Fith Attack", StringComparison.CurrentCultureIgnoreCase));

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
                Handler.Combat.Fight(player, player.Target, Handler.World.GetRoom(player.RoomId), false);
            }

        }

    }
}