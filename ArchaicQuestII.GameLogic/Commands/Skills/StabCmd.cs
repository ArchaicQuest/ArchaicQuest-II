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
    public class StabCmd : SkillCore, ICommand
    {
        public StabCmd()
            : base()
        {
            Aliases = new[] { "stab" };
            Description =
                "Delivers a powerful stab, a few of these will drop anyone like a sack of potatoes. Weapon Damage + 1d6";
            Usages = new[] { "Type: stab bob" };
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Resting,
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Mounted,
                CharacterStatus.Status.Stunned
            };
            Title = SkillName.Stab.ToString();
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
            if (!player.HasSkill(SkillName.Stab))
                return;

            if (player.Equipped.Wielded == null)
            {
                Services.Instance.Writer.WriteLine(
                    "You need to have a weapon equipped to do this.",
                    player
                );
                return;
            }

            var obj = input.ElementAtOrDefault(1)?.ToLower() ?? player.Target;
            if (string.IsNullOrEmpty(obj))
            {
                Services.Instance.Writer.WriteLine("Stab What!?.", player);
                return;
            }

            var target = FindTargetInRoom(obj, room, player);
            if (target == null)
            {
                return;
            }

            var textToTarget = string.Empty;
            var textToRoom = string.Empty;

            var skillSuccess = player.RollSkill(
                SkillName.Stab,
                true,
                $"You attempt to stab {target.Name} but miss."
            );
            if (!skillSuccess)
            {
                textToTarget = $"{player.Name} tries to stab you but misses.";
                textToRoom = $"{player.Name} tries to stab {target.Name} but misses.";

                EmoteAction(textToTarget, textToRoom, target.Name, room, player);
                player.FailedSkill(SkillName.Stab, true);
                player.Lag += 1;
                return;
            }

            var weaponDam =
                (player.Equipped.Wielded.Damage.Maximum + player.Equipped.Wielded.Damage.Minimum)
                / 2;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = (weaponDam + DiceBag.Roll(1, 1, 6)) + str / 5;

            DamagePlayer(SkillName.Stab.ToString(), damage, player, target, room);

            player.Lag += 1;
        }
    }
}
