using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Skills
{
    public class UppercutCmd : SkillCore, ICommand
    {
        public UppercutCmd()
            : base()
        {
            Aliases = new[] { "uppercut", "upper", "uc" };
            Description =
                "Throw a strong uppercut to an unsuspecting chin, a small chance to causing the receiver to be stunned.";
            Usages = new[] { "Type: uppercut bob, uc bob" };
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Resting,
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Mounted,
                CharacterStatus.Status.Stunned
            };
            Title = SkillName.UpperCut.ToString();
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
            if (!player.HasSkill(SkillName.UpperCut))
                return;

            var obj = input.ElementAtOrDefault(1)?.ToLower() ?? player.Target;
            if (string.IsNullOrEmpty(obj))
            {
                Services.Instance.Writer.WriteLine("Uppercut What!?.", player);
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
                SkillName.UpperCut,
                true,
                $"You attempt to uppercut {target.Name} but miss."
            );
            if (!skillSuccess)
            {
                textToTarget = $"{player.Name} tries to uppercut you but misses.";
                textToRoom = $"{player.Name} tries to uppercut {target.Name} but misses.";

                EmoteAction(textToTarget, textToRoom, target.Name, room, player);
                player.FailedSkill(SkillName.UpperCut, true);
                player.Lag += 1;
                return;
            }

            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = DiceBag.Roll(1, 1, 6) + str / 5;

            var helmet = target.Equipped.Head;
            var chance = DiceBag.Roll(1, 1, 100);
            if (helmet != null)
            {
                if (chance <= 15)
                {
                    room.Items.Add(helmet);
                    target.Equipped.Head = null;

                    Services.Instance.Writer.WriteLine(
                        $"Your uppercut knocks {helmet.Name.ToLower()} off {target.Name}'s head.",
                        player
                    );
                    textToTarget = $"{player.Name} knocks {helmet.Name.ToLower()} off your head.";
                    textToRoom =
                        $"{player.Name} knocks {helmet.Name.ToLower()} off {target.Name}'s head.";
                    EmoteAction(textToTarget, textToRoom, target.Name, room, player);
                }
            }
            else
            {
                if (chance <= 15)
                {
                    Services.Instance.Writer.WriteLine(
                        $"Your uppercut stuns {target.Name}.",
                        player
                    );
                    textToTarget = $"{player.Name}'s uppercut stuns you!";
                    textToRoom = $"{player.Name}'s uppercut stuns {target.Name}.";
                    EmoteAction(textToTarget, textToRoom, target.Name, room, player);

                    target.Lag += 2;
                }
            }

            DamagePlayer(SkillName.UpperCut.ToString(), damage, player, target, room);

            player.Lag += 1;
        }
    }
}
