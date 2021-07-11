using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Core;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill.Skills
{

    public interface IDamageSkills
    {
        int Kick(Player player, Player target, Room room);
        int Elbow(Player player, Player target, Room room);
        int Trip(Player player, Player target, Room room);
        int HeadButt(Player player, Player target, Room room);
        int Charge(Player player, Player target, Room room, string obj);

    }

    public class DamageSkills : IDamageSkills
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _updateClientUi;
        private readonly IDice _dice;
        private readonly IDamage _damage;
        private readonly ICombat _fight;
        private readonly ISkillManager _skillManager;



        public DamageSkills(IWriteToClient writer, IUpdateClientUI updateClientUi, IDice dice, IDamage damage, ICombat fight, ISkillManager skillManager)
        {
            _writer = writer;
            _updateClientUi = updateClientUi;
            _dice = dice;
            _damage = damage;
            _fight = fight;
            _skillManager = skillManager;

        }

        public int Kick(Player player, Player target, Room room)
        {
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = _dice.Roll(1, 1, 8) + str / 4;
 
            _skillManager.DamagePlayer("Kick", damage, player, target, room);

            player.Lag += 1;


            _skillManager.updateCombat(player, target);

            return damage;
        }

        public int Elbow(Player player, Player target, Room room)
        {
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = _dice.Roll(1, 1, 6) + str / 5;

            _skillManager.DamagePlayer("elbow", damage, player, target, room);

            player.Lag += 1;


            _skillManager.updateCombat(player, target);

            return damage;
        }

        // TODO skill success check
        public int HeadButt(Player player, Player target, Room room)
        {
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = _dice.Roll(1, 1, 12) + str / 5;

            if (player.Equipped.Head == null)
            {
                damage /= 2;
            }

            _skillManager.DamagePlayer("headbutt", damage, player, target, room);

            player.Lag += 1;


            _skillManager.updateCombat(player, target);

            return damage;
        }

        public int Charge(Player player, Player target, Room room, string obj)
        {
            if (player.Status == CharacterStatus.Status.Fighting)
            {
                _writer.WriteLine("You are already in combat, Charge can only be used to start a combat.");
                return 0;
            }

            var nthTarget = Helpers.findNth(obj);
 
            var character = Helpers.FindMob(nthTarget, room) ?? Helpers.FindPlayer(nthTarget, room);

 
            var weaponDam = player.Equipped.Wielded != null ? player.Equipped.Wielded.Damage.Maximum : 1 * 2;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = _dice.Roll(1, 1, weaponDam) + str / 5;


            _skillManager.DamagePlayer("charge", damage, player, target, room);

            player.Lag += 2;

            _skillManager.updateCombat(player, target);

            return damage;
        }

        public int Trip(Player player, Player target, Room room)
        {
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = _dice.Roll(1, 1, 4) + str / 5;

            var skillMessage = new SkillMessage()
            {
                Hit =
                {
                    ToPlayer = $"You trip {target.Name} and {target.Name} goes down!",
                    ToRoom = $"{player.Name} trips {target.Name} and {target.Name} goes down!",
                    ToTarget = $"{player.Name} trips you and you go down!"
                },
                Miss =
                {
                    ToPlayer = $"You trip {target.Name} and {target.Name} goes down!",
                    ToRoom = $"{player.Name} trips {target.Name} and {target.Name} goes down!",
                    ToTarget = $"{player.Name} trips you and you go down!"
                }
            };

            if (target.Lag == 0)
            {

                _skillManager.EmoteAction(player, target, room, skillMessage);

                _skillManager.DamagePlayer("trip", damage, player, target, room);

                player.Lag += 1;
                target.Lag += 2;

                target.Status = CharacterStatus.Status.Stunned;

                _skillManager.updateCombat(player, target);
            }
            else
            {
                player.Lag += 1;

                var skillMessageMiss = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You try to trip {target.Name} and miss.",
                        ToRoom = $"{player.Name} tries to trip {target.Name} but {target.Name} easily avoids it.",
                        ToTarget = $"{player.Name} tries to trip you but fails."
                    },
                    Miss =
                    {
                        ToPlayer = $"You try to trip {target.Name} and miss.",
                        ToRoom = $"{player.Name} tries to trip {target.Name} but {target.Name} easily avoids it.",
                        ToTarget = $"{player.Name} tries to trip you but fails."
                    }
                };

                _skillManager.EmoteAction(player, target, room, skillMessageMiss);
            }

            return damage;
        }
    }
}
