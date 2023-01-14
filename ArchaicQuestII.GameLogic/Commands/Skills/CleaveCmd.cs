
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
    public class CleaveCmd :  SkillCore, ICommand
    {
        public CleaveCmd(ICore core): base (core)
        {
            Aliases = new[] { "cleave", "clea"};
            Description = "Cleave is a strong swing of your blade or axe in the attempt to split your opponent in half.";
            Usages = new[] { "Type: cleave bob", "clea bob" };
            DeniedStatus = new [] { CharacterStatus.Status.Sleeping, CharacterStatus.Status.Resting, CharacterStatus.Status.Dead, CharacterStatus.Status.Mounted, CharacterStatus.Status.Stunned };
            Title = DefineSkill.Cleave().Name;
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
   
            var canDoSkill = CanPerformSkill(DefineSkill.Cleave(), player);
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
                Core.Writer.WriteLine("Cleave What!?.", player.ConnectionId);
                return;
            }
          
            var target = FindTargetInRoom(obj, room, player);
            if (target == null)
            {
                return;
            }
            
            var textToTarget = string.Empty;
            var textToRoom = string.Empty;

            var skillSuccess = SkillSuccess(player, DefineSkill.Cleave(), $"You attempt to cleave {target.Name} but miss.");
            if (!skillSuccess)
            { 
                textToTarget = $"{player.Name} tries to cleave you with {player.Equipped.Wielded.Name.ToLower()} but misses."; 
                textToRoom = $"{player.Name} tries to cleave {target.Name} with {player.Equipped.Wielded.Name.ToLower()} but misses.";
                
                EmoteAction(textToTarget, textToRoom, target.Name, room, player);
                player.Lag += 1;
                return;
            }
            var weaponDam = player.Equipped.Wielded.Damage.Maximum;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = weaponDam + DiceBag.Roll(1, 3, 8) + str / 5;


            DamagePlayer(DefineSkill.Cleave().Name, damage, player, target, room);

            player.Lag += 1;

            updateCombat(player, target, room);
            
        }
    }

}