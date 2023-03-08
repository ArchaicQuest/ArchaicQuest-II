
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
    public class UppercutCmd :  SkillCore, ICommand
    {
        public UppercutCmd(ICore core): base (core)
        {
            Aliases = new[] { "uppercut", "upper", "uc"};
            Description = "Throw a strong uppercut to an unsuspecting chin, a small chance to causing the receiver to be stunned.";
            Usages = new[] { "Type: uppercut bob, uc bob" };
            DeniedStatus = new [] { CharacterStatus.Status.Sleeping, CharacterStatus.Status.Resting, CharacterStatus.Status.Dead, CharacterStatus.Status.Mounted, CharacterStatus.Status.Stunned };
            Title = DefineSkill.UpperCut().Name;
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
   
            var canDoSkill = CanPerformSkill(DefineSkill.UpperCut(), player);
            if (!canDoSkill)
            { 
                return;
            }
 
            var obj = input.ElementAtOrDefault(1)?.ToLower() ?? player.Target;
            if (string.IsNullOrEmpty(obj))
            {
                Core.Writer.WriteLine("Uppercut What!?.", player.ConnectionId);
                return;
            }
          
            var target = FindTargetInRoom(obj, room, player);
            if (target == null)
            {
                return;
            }
            
            var textToTarget = string.Empty;
            var textToRoom = string.Empty;

            var skillSuccess = SkillSuccessWithMessage(player, DefineSkill.Stab(), $"You attempt to uppercut {target.Name} but miss.");
            if (!skillSuccess)
            { 
                textToTarget = $"{player.Name} tries to uppercut you but misses."; 
                textToRoom = $"{player.Name} tries to uppercut {target.Name} but misses.";
                
                EmoteAction(textToTarget, textToRoom, target.Name, room, player);
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

                    Core.Writer.WriteLine($"Your uppercut knocks {helmet.Name.ToLower()} off {target.Name}'s head.");
                    textToTarget = $"{player.Name} knocks {helmet.Name.ToLower()} off your head.";
                    textToRoom = $"{player.Name} knocks {helmet.Name.ToLower()} off {target.Name}'s head.";
                    EmoteAction(textToTarget, textToRoom, target.Name, room, player);
                }
            }
            else
            {
                if (chance <= 15)
                {
                    
                    Core.Writer.WriteLine($"Your uppercut stuns {target.Name}.");
                    textToTarget = $"{player.Name}'s uppercut stuns you!";
                    textToRoom = $"{player.Name}'s uppercut stuns {target.Name}.";
                    EmoteAction(textToTarget, textToRoom, target.Name, room, player);

                    target.Lag += 2;
                }
            }

            DamagePlayer(DefineSkill.UpperCut().Name, damage, player, target, room);

            player.Lag += 1;

            updateCombat(player, target, room);
            
        }
    }

}