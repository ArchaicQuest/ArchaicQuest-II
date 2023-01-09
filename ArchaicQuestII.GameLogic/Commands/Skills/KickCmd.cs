using System;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill;
using ArchaicQuestII.GameLogic.Skill.Core;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Skills
{
    public class KickCmd :  SkillCore, ICommand
    {
        public KickCmd(ICore core): base (core)
        {
            Aliases = new[] { "kick", "kic" };
            Description = "";
            Usages = new[] { "Type: kick cow - to see the stats of that item" };
            DeniedStatus = new [] { CharacterStatus.Status.Sleeping, CharacterStatus.Status.Resting, CharacterStatus.Status.Dead, CharacterStatus.Status.Mounted, CharacterStatus.Status.Stunned };
            Title = DefineOffensiveSkills.Kick().Name;
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
            
            
            var canDoSkill = CanPerformSkill(DefineOffensiveSkills.Kick(), player);
            if (!canDoSkill)
            { 
                return;
            }

            var obj = input.ElementAtOrDefault(1)?.ToLower();
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


            var damage = DiceBag.Roll(1, 1, 8) + player.Attributes.Attribute[EffectLocation.Strength] / 4;
            player.Lag += 1;

          DamagePlayer("Kick", damage, player, target, room);
          updateCombat(player, target, room);
        }
    }

}