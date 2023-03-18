
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
    public class TripCmd :  SkillCore, ICommand
    {
        public TripCmd(ICore core): base (core)
        {
            Aliases = new[] { "trip", "tri" };
            Description = "A cheap move but effective, Trip your opponent to stun them and strike them while they're down.";
            Usages = new[] { "Type: trip gary" };
            DeniedStatus = new [] { CharacterStatus.Status.Sleeping, CharacterStatus.Status.Resting, CharacterStatus.Status.Dead, CharacterStatus.Status.Mounted, CharacterStatus.Status.Stunned };
            Title = DefineSkill.Trip().Name;
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
   
            var canDoSkill = CanPerformSkill(DefineSkill.Trip(), player);
            if (!canDoSkill)
            { 
                return;
            }

            var obj = input.ElementAtOrDefault(1)?.ToLower() ?? player.Target;
            if (string.IsNullOrEmpty(obj))
            {
                Core.Writer.WriteLine("Trip What!?.", player.ConnectionId);
                return;
            }
          
            var target = FindTargetInRoom(obj, room, player);
            if (target == null)
            {
                return;
            }
            
            var textToTarget = string.Empty;
            var textToRoom = string.Empty;

            var skillSuccess = SkillSuccessWithMessage(player, DefineSkill.Trip(), $"You try to trip {target.Name} and miss.");
            if (!skillSuccess)
            { 
                textToTarget = $"{player.Name} tries to trip you."; 
                textToRoom = $"{player.Name} tries to trip {target.Name}.";
                
                EmoteAction(textToTarget, textToRoom, target.Name, room, player);
                player.Lag += 1;
                return;
            }
            
            if (target.Lag <= 1)
            {

                Core.Writer.WriteLine($"You trip {target.Name} and {target.Name} goes down!");
                textToRoom = $"{player.Name} trips {target.Name} and {target.Name} goes down!";
                textToTarget = $"{player.Name} trips you and you go down!";

                EmoteAction(textToTarget, textToRoom, target.Name, room, player);   

                DamagePlayer(DefineSkill.Trip().Name, DiceBag.Roll("1d6"), player, target, room);
                
                target.Lag += 2;
                target.Status = CharacterStatus.Status.Stunned;
            }
            else
            {
                //Player already stunned
                player.Lag += 1;

                Core.Writer.WriteLine($"You try to trip {target.Name} and miss.");
                textToRoom = $"{player.Name} tries to trip {target.Name} but {target.Name} easily avoids it.";
                textToTarget = $"{player.Name} tries to trip you but fails.";

                EmoteAction(textToTarget, textToRoom, target.Name, room, player);
                Core.Writer.WriteLine(Helpers.SkillLearnMistakes(player, DefineSkill.Trip().Name, Core.Gain), player.ConnectionId);
            }
            updateCombat(player, target, room);
        }
    }

}