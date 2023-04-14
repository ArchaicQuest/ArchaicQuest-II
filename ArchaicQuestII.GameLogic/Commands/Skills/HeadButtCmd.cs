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
    public class HeadButtCmd : SkillCore, ICommand
    {
        public HeadButtCmd()
            : base()
        {
            Aliases = new[] { "headbutt", "head" };
            Description =
                "A strong blow to the face with your head, deals double damage if the opponent has nothing worn in the head slot.";
            Usages = new[] { "Type: headbutt gary, head gary" };
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Resting,
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Mounted,
                CharacterStatus.Status.Stunned
            };
            Title = SkillName.Headbutt.ToString();
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
            if (!player.HasSkill(SkillName.Headbutt))
                return;

            var obj = input.ElementAtOrDefault(1)?.ToLower() ?? player.Target;
            if (string.IsNullOrEmpty(obj))
            {
                Services.Instance.Writer.WriteLine("Headbutt What!?.", player);
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
                SkillName.Headbutt,
                true,
                $"You try to headbutt {target.Name} but miss."
            );
            if (!skillSuccess)
            {
                textToTarget = $"{player.Name} tries to headbutt you.";
                textToRoom = $"{player.Name} tries to headbutt {target.Name}.";

                EmoteAction(textToTarget, textToRoom, target.Name, room, player);
                player.FailedSkill(SkillName.Headbutt, true);
                player.Lag += 1;
                return;
            }

            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = DiceBag.Roll("1d12") + str / 5;

            if (player.Equipped.Head == null)
            {
                damage *= 2;
            }

            textToTarget = $"{player.Name} smashes their head into your face!";
            textToRoom = $"{player.Name} smashes their head into the face of {target.Name}!";

            EmoteAction(textToTarget, textToRoom, target.Name, room, player);

            DamagePlayer(SkillName.Headbutt.ToString(), damage, player, target, room);

            player.Lag += 1;
        }
    }
}
