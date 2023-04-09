
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
    public class SlashCmd :  SkillCore, ICommand
    {
        public SlashCmd(ICore core): base (core)
        {
            Aliases = new[] { "slash"};
            Description = "Does what it says, a strong slash of your weapon. Weapon max damage + 1d10";
            Usages = new[] { "Type: slash bob" };
            DeniedStatus = new [] { CharacterStatus.Status.Sleeping, CharacterStatus.Status.Resting, CharacterStatus.Status.Dead, CharacterStatus.Status.Mounted, CharacterStatus.Status.Stunned };
            Title = DefineSkill.Slash().Name;
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
   
            var canDoSkill = CanPerformSkill(DefineSkill.Slash(), player);
            if (!canDoSkill)
            { 
                return;
            }
            
            if (player.Equipped.Wielded == null)
            {
                Core.Writer.WriteLine("You need to have a weapon equipped to do this.", player.ConnectionId);
                return;
            }

            var obj = input.ElementAtOrDefault(1)?.ToLower() ?? player.Target;
            if (string.IsNullOrEmpty(obj))
            {
                Core.Writer.WriteLine("Slash What!?.", player.ConnectionId);
                return;
            }
          
            var target = FindTargetInRoom(obj, room, player);
            if (target == null)
            {
                return;
            }
            
            var textToTarget = string.Empty;
            var textToRoom = string.Empty;

            var skillSuccess = SkillSuccessWithMessage(player, DefineSkill.Lunge(), $"You attempt to slash {target.Name} but miss.");
            if (!skillSuccess)
            { 
                textToTarget = $"{player.Name} tries to slash you but misses."; 
                textToRoom = $"{player.Name} tries to slash {target.Name} but misses.";
                
                EmoteAction(textToTarget, textToRoom, target.Name, room, player);
                player.FailedSkill(SkillName.Slash, out var message);
                Core.Writer.WriteLine(message, player.ConnectionId);
                player.Lag += 1;
                return;
            }
            
            var weaponDam = player.Equipped.Wielded.Damage.Maximum;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = weaponDam + DiceBag.Roll(1, 2, 10) + str / 5;

            DamagePlayer(DefineSkill.Slash().Name, damage, player, target, room);

            player.Lag += 1;

            updateCombat(player, target, room);
            
        }
    }

}