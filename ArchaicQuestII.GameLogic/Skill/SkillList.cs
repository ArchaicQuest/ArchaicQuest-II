using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Skill.Skills;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill
{

    public interface ISkillList
    {
        void DoSkill(string key, string obj, Player target, string fullCommand, Player player, Room room, bool wearOff);
    }

    public class SkillList:ISkillList
    {
        private readonly IDamageSkills _damage;
        private readonly IUtilSkills _util;
        public SkillList(IDamageSkills damage, IUtilSkills utilSkills)
        {
            _damage = damage;
            _util = utilSkills;
        }
        public void DoSkill(string key, string obj, Player target, string fullCommand, Player player, Room room, bool wearOff)
        {

            switch (key.ToLower())
            {
                case "kick":
                    _damage.Kick(player, target, room);
                    break;
                case "elbow":
                    _damage.Elbow(player, target, room);
                    break;
                case "trip":
                    _damage.Trip(player, target, room);
                    break;
                case "headbutt":
                    _damage.HeadButt(player, target, room);
                    break;
                case "charge":
                    _damage.Charge(player, target, room, obj);
                    break;
                case "stab":
                    _damage.Stab(player, target, room, obj);
                    break;
                case "uppercut":
                    _damage.UpperCut(player, target, room, obj);
                    break;
                case "dirt kick":
                case "dirtkick":
                    _damage.DirtKick(player, target, room, obj);
                    break;
                case "disarm":
                    _util.Disarm(player, target, room, obj);
                    break;
                case "lunge":
                    _damage.Lunge(player, target, room, obj);
                    break;
                case "berserk":
                    _util.Berserk(player, target, room);
                    break;
                case "rescue":
                    _util.Rescue(player, target, room, obj);
                    break;
                case "mount":
                    _util.Mount(player, target, room);
                    break;
                case "shield bash":
                    _damage.ShieldBash(player, target, room, obj);
                    break;
                case "war cry":
                    _util.WarCry(player, target, room);
                    break;
                case "hamstring":
                    _damage.HamString(player, target, room, obj);
                    break;
                case "impale":
                    _damage.Impale(player, target, room, obj);
                    break;
                case "slash":
                    _damage.Slash(player, target, room, obj);
                    break;
                case "overhead crush":
                    _damage.OverheadCrush(player, target, room, obj);
                    break;
                case "cleave":
                    _damage.Cleave(player, target, room, obj);
                    break;
                    //case "cure light wounds":
                    //    _damageSpells.CureLightWounds(player, target, room);
                    //    break;
                    //case "armour":
                    //case "armor":
                    //    _damageSpells.Armor(player, target, room, wearOff);
                    //    break;
                    //case "bless":
                    //    _damageSpells.Bless(player, target, room, wearOff);
                    //    break;
            }


        }

    }
}
