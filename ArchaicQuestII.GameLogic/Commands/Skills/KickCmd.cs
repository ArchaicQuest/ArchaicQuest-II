using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;
using DefineSkill = ArchaicQuestII.GameLogic.Skill.Model.DefineSkill;

namespace ArchaicQuestII.GameLogic.Commands.Skills
{
    public class KickCmd :  SkillCore, ICommand
    {
        public KickCmd(ICore core): base (core)
        {
            Aliases = new[] { "kick", "kic" };
            Description = "Kicking allows the adventurer to receive an extra attack in combat, a powerful " +
                          "kick. However, a failed kick may throw an unwary fighter off balance.";
            Usages = new[] { "Type: kick cow - kick the target, during combat only kick can be entered." };
            DeniedStatus = new [] { CharacterStatus.Status.Sleeping, CharacterStatus.Status.Resting, CharacterStatus.Status.Dead, CharacterStatus.Status.Mounted, CharacterStatus.Status.Stunned };
            Title = DefineSkill.Kick().Name;
            UserRole = UserRole.Player;
            Core = core;
        }

        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }

        public void Execute(Player player, Room room, string[] input)
        {
   
            var canDoSkill = CanPerformSkill(DefineSkill.Kick(), player);
            if (!canDoSkill)
            { 
                return;
            }

            var obj = input.ElementAtOrDefault(1)?.ToLower() ?? player.Target;
            if (string.IsNullOrEmpty(obj))
            {
                Core.Writer.WriteLine("Kick What!?.", player.ConnectionId);
                return;
            }
          
            var target = FindTargetInRoom(obj, room, player);
            if (target == null)
            {
                return;
            }     
            
            var textToTarget = string.Empty;
            var textToRoom = string.Empty;

            var skillSuccess = SkillSuccessWithMessage(player, DefineSkill.Kick(), "You miss your kick and stumble.");
            if (!skillSuccess)
            {
                 textToTarget = $"{player.Name} tries to kick you but stumbles.";
                 textToRoom = $"{player.Name} tries to kick {target.Name} but stumbles.";
                EmoteAction(textToTarget, textToRoom, target.Name, room, player);
                updateCombat(player, target, room);
                player.FailedSkill(SkillName.Kick, out var message);
                Core.Writer.WriteLine(message, player.ConnectionId);
                player.Lag += 1;
                return;
            }
            
             textToTarget = $"{player.Name} lashes out with a hard kick."; 
             textToRoom = $"{player.Name} lands a strong kick to {target.Name}.";
            EmoteAction(textToTarget, textToRoom, target.Name, room, player);


            var damage = DiceBag.Roll(1, 1, 8) + player.Attributes.Attribute[EffectLocation.Strength] / 4;
            player.Lag += 1;

          DamagePlayer("Kick", damage, player, target, room);
          updateCombat(player, target, room);
        }
    }

}