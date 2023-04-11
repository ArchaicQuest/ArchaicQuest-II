using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Skills
{
    public class DirtKickCmd : SkillCore, ICommand
    {
        public DirtKickCmd()
            : base()
        {
            Aliases = new[] { "dirtkick", "dirt", "dk" };
            Description =
                "Kick dirt to the eyes of your target blinding them, it's a cheap move but effective.";
            Usages = new[] { "Type: dirtkick bob, dk bob, dirt bob" };
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Resting,
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Mounted,
                CharacterStatus.Status.Stunned
            };
            Title = DefineSkill.DirtKick().Name;
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
            var canDoSkill = CanPerformSkill(DefineSkill.DirtKick(), player);
            if (!canDoSkill)
            {
                return;
            }

            var obj = input.ElementAtOrDefault(1)?.ToLower() ?? player.Target;
            if (string.IsNullOrEmpty(obj))
            {
                Services.Instance.Writer.WriteLine("Dirt kick What!?.", player.ConnectionId);
                return;
            }

            var target = FindTargetInRoom(obj, room, player);
            if (target == null)
            {
                return;
            }

            if (target.Affects.Blind)
            {
                Services.Instance.Writer.WriteLine(
                    $"{target.Name} has already been blinded.",
                    player.ConnectionId
                );
            }
            var textToTarget = string.Empty;
            var textToRoom = string.Empty;
            if (
                player.RollSkill(SkillName.DirtKick, true)
                && DexterityAndLevelCheck(player, target) == true
            )
            {
                Services.Instance.Writer.WriteLine(
                    $"You kick dirt in {target.Name}'s eyes!",
                    player.ConnectionId
                );
                textToTarget = $"{player.Name} kicks dirt into your eyes!";
                textToRoom = $"{player.Name} kicks dirt into {target.Name}'s eyes!";

                EmoteAction(textToTarget, textToRoom, target.Name, room, player);

                Services.Instance.Writer.WriteLine("You can't see a thing!", target.ConnectionId);

                target.Affects.Blind = true;

                var affect = new Affect()
                {
                    Duration = 2,
                    Modifier = new Modifier() { Dexterity = -4, HitRoll = -4 },
                    Affects = DefineSpell.SpellAffect.Blind,
                    Name = "Blindness from dirt kick"
                };

                target.Affects.Custom.Add(affect);

                target.ApplyAffects(affect);
            }
            else
            {
                Services.Instance.Writer.WriteLine(
                    $"You try to kick dirt but {target.Name} shut their eyes in time.",
                    player.ConnectionId
                );
                textToTarget = $"{player.Name} tries to kick dirt into your eyes.";
                textToRoom = $"{player.Name} tries to kicks dirt into {target.Name}'s eyes!";

                EmoteAction(textToTarget, textToRoom, target.Name, room, player);
                player.FailedSkill(SkillName.DirtKick, true);
            }

            player.Lag += 1;

            updateCombat(player, target, room);
        }
    }
}
