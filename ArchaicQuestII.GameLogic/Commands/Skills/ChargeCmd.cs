
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Skills
{
    public class ChargeCmd :  SkillCore, ICommand
    {
        public ChargeCmd(ICore core): base (core)
        {
            Aliases = new[] { "charge", "char" };
            Description = "Charge into combat and smash a devastating blow to your opponent! Can only be used to start a fight and max damage is linked to the max damage of your equipped weapon.\r\n" +
                          "Charging with a shield may also stun the target for 2 rounds.";
            Usages = new[] { "Type: charge bob, char bob" };
            DeniedStatus = new [] { CharacterStatus.Status.Sleeping, CharacterStatus.Status.Resting, CharacterStatus.Status.Dead, CharacterStatus.Status.Mounted, CharacterStatus.Status.Stunned };
            Title = DefineSkill.Charge().Name;
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
   
            var canDoSkill = CanPerformSkill(DefineSkill.Charge(), player);
            if (!canDoSkill)
            { 
                return;
            }
            
            if (player.Equipped.Wielded == null)
            {
                Core.Writer.WriteLine("You need to have a weapon equipped to do this.", player.ConnectionId);
                return;
            }
            
            if (player.Status == CharacterStatus.Status.Fighting)
            {
                Core.Writer.WriteLine("You are already in combat, Charge can only be used to start a combat.");
                return;
            }


            var obj = input.ElementAtOrDefault(1)?.ToLower() ?? player.Target;
            if (string.IsNullOrEmpty(obj))
            {
                Core.Writer.WriteLine("Charge What!?.", player.ConnectionId);
                return;
            }
          
            var target = FindTargetInRoom(obj, room, player);
            if (target == null)
            {
                return;
            }
            
            var textToTarget = string.Empty;
            var textToRoom = string.Empty;

            var skillSuccess = SkillSuccessWithMessage(player, DefineSkill.Charge(), $"You attempt to charge at to {target.Name} but miss.");
            if (!skillSuccess)
            { 
                textToTarget = $"{player.Name} tries to charge at you but misses."; 
                textToRoom = $"{player.Name} tries to charge at {target.Name} but misses.";
                
                EmoteAction(textToTarget, textToRoom, target.Name, room, player);
                player.FailedSkill(DefineSkill.Charge().Name, out var message);
                Core.Writer.WriteLine(message, player.ConnectionId);
                player.Lag += 1;
                return;
            }

            var weaponDam = player.Equipped.Wielded.Damage.Maximum + player.Equipped.Wielded.Damage.Maximum / 3;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = DiceBag.Roll(1, 1, weaponDam) + str / 5;
            
            DamagePlayer(DefineSkill.Charge().Name, damage, player, target, room);

            player.Lag += 1;

            if (DiceBag.Roll("1d10") <= 3 && player.Equipped.Shield != null)
            {
                target.Lag += 2;
                
                Core.Writer.WriteLine($"You smash {target.Name} to the ground with your charge.");
                textToTarget = $"{player.Name} charge smashes you off your feet."; 
                textToRoom = $"{player.Name} charge smashes {target.Name} off their feet.";
                
                EmoteAction(textToTarget, textToRoom, target.Name, room, player);
            }
            
            updateCombat(player, target, room);
        }
    }

}