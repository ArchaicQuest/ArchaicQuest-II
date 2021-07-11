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
        public SkillList(IDamageSkills damage)
        {
            _damage = damage;
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
                    //case "cause light wounds":
                    //    _damageSpells.CauseLightWounds(player, target, room);
                    //    break;
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
