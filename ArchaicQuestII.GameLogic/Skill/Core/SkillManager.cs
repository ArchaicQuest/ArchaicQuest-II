using System;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill.Core
{
    public class SkillManager : ISkillManager
    {
        public string ReplacePlaceholders(string str, Player player, bool isTarget)
        {
            var newString = String.Empty;
            if (isTarget)
            {
                newString = str.Replace("#target#", "You");

                return newString;
            }

            newString = str.Replace("#target#", player.Name);

            return newString;
        }

        public void DamagePlayer(
            string spellName,
            int damage,
            Player player,
            Player target,
            Room room
        )
        {
            if (target.IsAlive())
            {
                var totalDam = CombatHandler.CalculateSkillDamage(player, target, damage);

                Services.Instance.Writer.WriteLine(
                    $"<p>Your {spellName} {Services.Instance.Damage.DamageText(totalDam).Value} {target.Name}  <span class='damage'>[{damage}]</span></p>",
                    player
                );
                Services.Instance.Writer.WriteLine(
                    $"<p>{player.Name}'s {spellName} {Services.Instance.Damage.DamageText(totalDam).Value} you!  <span class='damage'>[{damage}]</span></p>",
                    target
                );

                foreach (var pc in room.Players)
                {
                    if (
                        pc.ConnectionId.Equals(player.ConnectionId)
                        || pc.ConnectionId.Equals(target.ConnectionId)
                    )
                    {
                        continue;
                    }

                    Services.Instance.Writer.WriteLine(
                        $"<p>{player.Name}'s {spellName} {Services.Instance.Damage.DamageText(totalDam).Value} {target.Name}  <span class='damage'>[{damage}]</span></p>",
                        pc
                    );
                }

                target.Attributes.Attribute[EffectLocation.Hitpoints] -= totalDam;

                if (!target.IsAlive())
                {
                    CombatHandler.TargetKilled(player, target, room);

                    Services.Instance.UpdateClient.UpdateHP(target);
                    return;
                    //TODO: create corpse, refactor fight method from combat.cs
                }

                //update UI
                Services.Instance.UpdateClient.UpdateHP(target);

                var combat = new Fight(player, target, room, false);
                Services.Instance.Cache.AddCombat(combat);
            }
        }

        public void UpdateClientUI(Player player)
        {
            //update UI
            Services.Instance.UpdateClient.UpdateHP(player);
            Services.Instance.UpdateClient.UpdateMana(player);
            Services.Instance.UpdateClient.UpdateMoves(player);
            Services.Instance.UpdateClient.UpdateScore(player);
        }

        public void EmoteAction(Player player, Player target, Room room, SkillMessage emote)
        {
            if (target.ConnectionId == player.ConnectionId)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>{ReplacePlaceholders(emote.Hit.ToPlayer, target, true)}</p>",
                    target
                );
            }
            else
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>{ReplacePlaceholders(emote.Hit.ToPlayer, target, false)}</p>",
                    player
                );
            }

            if (!string.IsNullOrEmpty(emote.Hit.ToTarget))
            {
                Services.Instance.Writer.WriteLine($"<p>{emote.Hit.ToTarget}</p>", target);
            }

            foreach (var pc in room.Players)
            {
                if (
                    pc.ConnectionId.Equals(player.ConnectionId)
                    || pc.ConnectionId.Equals(target.ConnectionId)
                )
                {
                    continue;
                }

                Services.Instance.Writer.WriteLine(
                    $"<p>{ReplacePlaceholders(emote.Hit.ToRoom, target, false)}</p>",
                    pc
                );
            }
        }
    }
}
