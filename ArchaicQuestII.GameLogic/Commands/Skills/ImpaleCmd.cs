using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Skills
{
    public class ImpaleCmd : SkillCore, ICommand
    {
        public ImpaleCmd()
            : base()
        {
            Aliases = new[] { "impale", "imp" };
            Description = "Like a stab but much more forceful and devastating.";
            Usages = new[] { "Type: impale bob", "imp bob" };
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Resting,
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Mounted,
                CharacterStatus.Status.Stunned
            };
            Title = DefineSkill.Impale().Name;
            UserRole = UserRole.Player;
        }

        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var canDoSkill = CanPerformSkill(DefineSkill.Impale(), player);
            if (!canDoSkill)
            {
                return;
            }

            if (player.Equipped.Wielded == null)
            {
                Services.Instance.Writer.WriteLine(
                    "You need to have a weapon equipped to do this.",
                    player.ConnectionId
                );
                return;
            }

            var obj = input.ElementAtOrDefault(1)?.ToLower() ?? player.Target;
            if (string.IsNullOrEmpty(obj))
            {
                Services.Instance.Writer.WriteLine("Impale What!?.", player.ConnectionId);
                return;
            }

            var target = FindTargetInRoom(obj, room, player);
            if (target == null)
            {
                return;
            }

            var textToTarget = string.Empty;
            var textToRoom = string.Empty;

            var skillSuccess = SkillSuccessWithMessage(
                player,
                DefineSkill.Impale(),
                $"You attempt to impale {target.Name} but miss."
            );
            if (!skillSuccess)
            {
                textToTarget =
                    $"{player.Name} tries to impale you with {player.Equipped.Wielded.Name.ToLower()} but misses.";
                textToRoom =
                    $"{player.Name} tries to impale {target.Name} with {player.Equipped.Wielded.Name.ToLower()} but misses.";

                EmoteAction(textToTarget, textToRoom, target.Name, room, player);
                player.FailedSkill(SkillName.Impale, out var message);
                Services.Instance.Writer.WriteLine(message, player.ConnectionId);
                player.Lag += 1;
                return;
            }

            var weaponDam = player.Equipped.Wielded.Damage.Maximum;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = weaponDam + DiceBag.Roll(1, 2, 10) + str / 5;

            DamagePlayer(DefineSkill.Impale().Name, damage, player, target, room);

            player.Lag += 1;

            updateCombat(player, target, room);
        }
    }
}
