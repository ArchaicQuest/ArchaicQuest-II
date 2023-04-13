using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Combat
{
    public class Fight
    {
        private List<Player> _aggressors = new List<Player>();
        private List<Player> _victims = new List<Player>();
        private Queue<Player> _combatQueue = new Queue<Player>();
        private Room _room;
        private bool _isMurder;
        public bool Ended;

        public Fight(Player aggressor, Player victim, Room room, bool isMurder)
        {
            _room = room;
            _isMurder = isMurder;

            _aggressors.Add(aggressor);
            aggressor.Target = victim.Name;
            aggressor.Status = CharacterStatus.Status.Fighting;

            if (aggressor.Grouped)
            {
                foreach (var follower in aggressor.Followers)
                {
                    if (
                        follower.Grouped
                        && follower.Following == aggressor.Name
                        && follower.Status != CharacterStatus.Status.Fighting
                    )
                    {
                        if (follower.Config.AutoAssist && string.IsNullOrEmpty(follower.Target))
                        {
                            follower.Buffer.Clear();
                            follower.Target = aggressor.Target;
                            follower.Status = CharacterStatus.Status.Fighting;
                            _aggressors.Add(follower);
                        }
                    }
                }
            }

            _victims.Add(victim);
            victim.Target = aggressor.Name;
            victim.Status =
                (victim.Status & CharacterStatus.Status.Stunned) != 0
                    ? CharacterStatus.Status.Stunned
                    : CharacterStatus.Status.Fighting;

            if (victim.Grouped)
            {
                foreach (var follower in victim.Followers)
                {
                    if (
                        follower.Grouped
                        && follower.Following == victim.Name
                        && follower.Status != CharacterStatus.Status.Fighting
                    )
                    {
                        if (follower.Config.AutoAssist && string.IsNullOrEmpty(follower.Target))
                        {
                            follower.Buffer.Clear();
                            follower.Target = victim.Target;
                            follower.Status = CharacterStatus.Status.Fighting;
                            _victims.Add(follower);
                        }
                    }
                }
            }
        }

        private void End()
        {
            Ended = true;

            foreach (var player in _aggressors)
            {
                player.Status = CharacterStatus.Status.Standing;
            }

            foreach (var player in _victims)
            {
                player.Status = CharacterStatus.Status.Standing;
            }
        }

        public void Do()
        {
            foreach (var player in _aggressors)
            {
                SetupInitiative(player);
            }

            foreach (var player in _victims)
            {
                SetupInitiative(player);
            }

            while (_combatQueue.Count > 0 && !Ended)
            {
                DoRound(_combatQueue.Dequeue());
            }
        }

        private void DoRound(Player player)
        {
            try
            {
                if (player.Affects.Stunned)
                {
                    Services.Instance.Writer.WriteLine(
                        "<p>You are too stunned to attack this round.<p>",
                        player
                    );
                    return;
                }

                Player target = null;

                if (_aggressors.Contains(player))
                    target = _victims[DiceBag.Roll(1, 0, _victims.Count - 1)];
                else
                    target = _aggressors[DiceBag.Roll(1, 0, _aggressors.Count - 1)];

                if (target == null)
                {
                    End();
                    return;
                }

                if (target.Name == player.Name)
                {
                    Services.Instance.Writer.WriteLine(
                        "<p>You can't start a fight with yourself!</p>",
                        player
                    );
                    return;
                }

                if (player.Attributes.Attribute[EffectLocation.Hitpoints] <= 0)
                {
                    Services.Instance.Writer.WriteLine(
                        "<p>You cannot do that while dead.</p>",
                        player
                    );
                    return;
                }

                if (target.Attributes.Attribute[EffectLocation.Hitpoints] <= 0)
                {
                    Services.Instance.Writer.WriteLine("<p>They are already dead.</p>", player);

                    player.Target = String.Empty;
                    return;
                }

                // For the UI to create a nice gap between rounds of auto attacks
                Services.Instance.Writer.WriteLine($"<p class='combat-start'></p>", player);

                player.Target = target.Name;
                target.Target = string.IsNullOrEmpty(target.Target) ? player.Name : target.Target; //for group combat, if target is ganged, there target should not be changed when combat is initiated.

                /*
                 *  This section crying out for a refactor
                 */


                var weapon = CombatHandler.GetWeapon(player);
                var chanceToHit = Services.Instance.Formulas.ToHitChance(player, target, false);

                if (chanceToHit < 5)
                {
                    chanceToHit = 5;
                }

                //if player bind and don't have blind fighting
                // reduce chance to hit by 40%
                if (player.Affects.Blind && !CombatHandler.BlindFighting(player))
                {
                    chanceToHit = (int)(chanceToHit - (chanceToHit * .70));
                }

                var doesHit = Services.Instance.Formulas.DoesHit(chanceToHit);

                if (doesHit)
                {
                    // avoidance percentage can be improved by core skills
                    // such as improved parry, acrobatic etc
                    // instead of rolling a D10, roll a D6 for a close to 15% increase in chance

                    // Move to formula, needs to use _dice instead of making a new instance
                    var avoidanceRoll = DiceBag.Roll(1, 1, 10);

                    var avoidanceChance = DiceBag.Roll(1, 1, 100);
                    //10% chance to attempt a dodge
                    if (avoidanceRoll == 1)
                    {
                        var dodge = target.GetSkill(SkillName.Dodge);

                        if (dodge != null && avoidanceChance <= dodge.Proficiency)
                        {
                            Services.Instance.Writer.WriteLine(
                                $"<p>You dodge {player.Name}'s attack.</p>",
                                target
                            );
                            Services.Instance.Writer.WriteLine(
                                $"<p>{target.Name} dodges your attack.</p>",
                                player
                            );
                        }
                        else
                        {
                            player.FailedSkill(SkillName.Dodge, true);
                            Services.Instance.Writer.WriteLine(
                                $"<p>You fail to dodge {player.Name}'s attack.</p>",
                                target
                            );
                        }
                    }

                    //10% chance to parry
                    if (avoidanceRoll == 2)
                    {
                        var skill = target.GetSkill(SkillName.Parry);

                        if (skill != null && avoidanceChance <= skill.Proficiency)
                        {
                            Services.Instance.Writer.WriteLine(
                                $"<p>You parry {player.Name}'s attack.</p>",
                                target
                            );
                            Services.Instance.Writer.WriteLine(
                                $"<p>{target.Name} parries your attack.</p>",
                                player
                            );

                            var riposte = target.GetSkill(SkillName.Riposte);

                            if (riposte != null && avoidanceChance <= riposte.Proficiency)
                            {
                                Services.Instance.Writer.WriteLine(
                                    $"<p>You riposte {player.Name}'s attack.</p>",
                                    target
                                );
                                Services.Instance.Writer.WriteLine(
                                    $"<p>{target.Name} riposte's your attack.</p>",
                                    player
                                );

                                var ripDamage = Services.Instance.Formulas.CalculateDamage(
                                    target,
                                    player,
                                    weapon
                                );

                                ripDamage /= 3;

                                target.HarmTarget(ripDamage);

                                CombatHandler.DisplayDamage(
                                    target,
                                    player,
                                    _room,
                                    weapon,
                                    ripDamage
                                );

                                Services.Instance.UpdateClient.UpdateHP(player);

                                if (!target.IsAlive())
                                {
                                    CombatHandler.TargetKilled(target, player, _room);
                                }
                            }
                            else
                            {
                                player.FailedSkill(SkillName.Riposte, true);
                                Services.Instance.Writer.WriteLine(
                                    $"<p>You fail to riposte {player.Name}'s attack.</p>",
                                    target
                                );
                            }
                        }
                        else
                        {
                            player.FailedSkill(SkillName.Parry, true);
                            Services.Instance.Writer.WriteLine(
                                $"<p>You fail to parry {player.Name}'s attack.</p>",
                                target
                            );
                        }
                    }

                    // Block
                    if (avoidanceRoll == 3 && player.Equipped.Shield != null)
                    {
                        var chanceToBlock = Services.Instance.Formulas.ToBlockChance(
                            target,
                            player
                        );
                        var doesBlock = Services.Instance.Formulas.DoesHit(chanceToBlock);

                        if (doesBlock)
                        {
                            var skill = target.GetSkill(SkillName.ShieldBlock);
                            ;

                            if (skill != null)
                            {
                                Services.Instance.UpdateClient.PlaySound("block", player);
                                Services.Instance.UpdateClient.PlaySound("block", target);
                                Services.Instance.Writer.WriteLine(
                                    $"You block {player.Name}'s attack with your shield.",
                                    target
                                );
                                Services.Instance.Writer.WriteLine(
                                    $"{target.Name} blocks your attack with their shield.",
                                    player
                                );
                                return;
                            }
                        }
                        else
                        {
                            player.FailedSkill(SkillName.ShieldBlock, true);
                            Services.Instance.Writer.WriteLine(
                                $"<p>You fail to block {player.Name}'s attack.</p>",
                                target
                            );
                        }
                    }

                    var damage = Services.Instance.Formulas.CalculateDamage(player, target, weapon);

                    var enhancedDamageChance = DiceBag.Roll(1, 1, 100);
                    var hasEnhancedDamage = player.GetSkill(SkillName.EnhancedDamage);

                    if (Services.Instance.Formulas.IsCriticalHit())
                    {
                        // double damage
                        damage *= 2;
                    }

                    if (hasEnhancedDamage != null)
                    {
                        if (
                            hasEnhancedDamage.Proficiency >= enhancedDamageChance
                            && player.Level >= hasEnhancedDamage.Level
                        )
                        {
                            var bonusDam = Helpers.GetPercentage(15, damage);
                            damage += bonusDam;
                        }
                    }

                    Services.Instance.UpdateClient.PlaySound("hit", target);
                    Services.Instance.UpdateClient.PlaySound("hit", player);
                    target.HarmTarget(damage);

                    CombatHandler.DisplayDamage(player, target, _room, weapon, damage);

                    Services.Instance.UpdateClient.UpdateHP(target);

                    if (!target.IsAlive())
                    {
                        CombatHandler.TargetKilled(player, target, _room);
                    }
                }
                else
                {
                    Services.Instance.UpdateClient.PlaySound("miss", target);
                    Services.Instance.UpdateClient.PlaySound("miss", player);
                    CombatHandler.DisplayMiss(player, target, _room, weapon);
                    // miss message
                    // gain improvements on weapon skill


                    SkillList getWeaponSkill = null;
                    if (
                        weapon != null
                        && !player.ConnectionId.Equals(
                            "mob",
                            StringComparison.CurrentCultureIgnoreCase
                        )
                    )
                    {
                        // urgh this is ugly
                        getWeaponSkill = player.GetSkill(weapon.WeaponType);
                    }

                    if (
                        weapon == null
                        && !player.ConnectionId.Equals(
                            "mob",
                            StringComparison.CurrentCultureIgnoreCase
                        )
                    )
                    {
                        getWeaponSkill = player.GetSkill(SkillName.Unarmed);
                    }

                    if (getWeaponSkill != null && getWeaponSkill.Proficiency < 100)
                    {
                        getWeaponSkill.Proficiency += 1;
                        Services.Instance.Writer.WriteLine(
                            $"<p class='improve'>Your proficiency in {getWeaponSkill.Name.ToString()} has increased.</p>",
                            player
                        );

                        player.GainExperiencePoints(getWeaponSkill.Level * 50, true);
                    }
                }

                if (player.Equipped.Secondary != null)
                {
                    weapon = CombatHandler.GetWeapon(player, true);
                    chanceToHit = Services.Instance.Formulas.ToHitChance(player, target, true);

                    if (player.ConnectionId == "mob" && chanceToHit < 45)
                    {
                        chanceToHit = 45;
                    }

                    //if player bind and don't have blind fighting
                    // reduce chance to hit by 40%
                    if (player.Affects.Blind && !CombatHandler.BlindFighting(player))
                    {
                        chanceToHit = (int)(chanceToHit - (chanceToHit * .40));
                    }

                    doesHit = Services.Instance.Formulas.DoesHit(chanceToHit);

                    if (doesHit)
                    {
                        // avoidance percentage can be improved by core skills
                        // such as improved parry, acrobatic etc
                        // instead of rolling a D10, roll a D6 for a close to 15% increase in chance

                        // Move to formula, needs to use _dice instead of making a new instance
                        var avoidanceRoll = DiceBag.Roll(1, 1, 10);

                        //10% chance to attempt a dodge
                        if (avoidanceRoll == 1)
                        {
                            var dodge = target.GetSkill(SkillName.Dodge);

                            if (dodge != null)
                            {
                                Services.Instance.Writer.WriteLine(
                                    $"<p>You dodge {player.Name}'s attack.</p>",
                                    target
                                );
                                Services.Instance.Writer.WriteLine(
                                    $"<p>{target.Name} dodges your attack.</p>",
                                    player
                                );
                                return;
                            }
                        }

                        //10% chance to parry
                        if (avoidanceRoll == 2)
                        {
                            var skill = target.GetSkill(SkillName.Parry);

                            if (skill != null)
                            {
                                Services.Instance.UpdateClient.PlaySound("parry", player);
                                Services.Instance.UpdateClient.PlaySound("parry", target);
                                Services.Instance.Writer.WriteLine(
                                    $"<p>You parry {player.Name}'s attack.</p>",
                                    target
                                );
                                Services.Instance.Writer.WriteLine(
                                    $"<p>{target.Name} parries your attack.</p>",
                                    player
                                );

                                var riposte = target.GetSkill(SkillName.Riposte);

                                if (riposte != null)
                                {
                                    Services.Instance.Writer.WriteLine(
                                        $"<p>You riposte {player.Name}'s attack.</p>",
                                        target
                                    );
                                    Services.Instance.Writer.WriteLine(
                                        $"<p>{target.Name} riposte's your attack.</p>",
                                        player
                                    );

                                    var ripDamage = Services.Instance.Formulas.CalculateDamage(
                                        target,
                                        player,
                                        weapon
                                    );

                                    ripDamage /= 3;

                                    player.HarmTarget(ripDamage);

                                    CombatHandler.DisplayDamage(
                                        target,
                                        player,
                                        _room,
                                        weapon,
                                        ripDamage
                                    );

                                    Services.Instance.UpdateClient.UpdateHP(player);

                                    if (!player.IsAlive())
                                    {
                                        CombatHandler.TargetKilled(target, player, _room);
                                    }
                                }

                                return;
                            }
                        }

                        // Block
                        if (avoidanceRoll == 3 && player.Equipped.Shield != null)
                        {
                            var chanceToBlock = Services.Instance.Formulas.ToBlockChance(
                                target,
                                player
                            );
                            var doesBlock = Services.Instance.Formulas.DoesHit(chanceToBlock);

                            if (doesBlock)
                            {
                                var skill = target.GetSkill(SkillName.ShieldBlock);

                                if (skill != null)
                                {
                                    Services.Instance.Writer.WriteLine(
                                        $"You block {player.Name}'s attack with your shield.",
                                        target
                                    );
                                    Services.Instance.Writer.WriteLine(
                                        $"{target.Name} blocks your attack with their shield.",
                                        player
                                    );
                                    return;
                                }
                            }
                            else
                            {
                                // block fail
                            }
                        }

                        var damage = Services.Instance.Formulas.CalculateDamage(
                            player,
                            target,
                            weapon
                        );

                        if (Services.Instance.Formulas.IsCriticalHit())
                        {
                            // double damage
                            damage *= 2;
                        }

                        var enhancedDamageChance = DiceBag.Roll(1, 1, 100);
                        var hasEnhancedDamage = player.GetSkill(SkillName.EnhancedDamage);

                        if (hasEnhancedDamage != null)
                        {
                            if (hasEnhancedDamage.Proficiency >= enhancedDamageChance)
                            {
                                var bonusDam = Helpers.GetPercentage(15, damage);
                                damage += bonusDam;
                            }
                        }

                        target.HarmTarget(damage);

                        CombatHandler.DisplayDamage(player, target, _room, weapon, damage);

                        Services.Instance.UpdateClient.UpdateHP(target);

                        if (!target.IsAlive())
                        {
                            CombatHandler.TargetKilled(player, target, _room);
                        }
                    }
                    else
                    {
                        CombatHandler.DisplayMiss(player, target, _room, weapon);
                        // miss message
                        // gain improvements on weapon skill


                        SkillList getWeaponSkill = null;
                        if (
                            weapon != null
                            && !player.ConnectionId.Equals(
                                "mob",
                                StringComparison.CurrentCultureIgnoreCase
                            )
                        )
                        {
                            getWeaponSkill = player.GetSkill(weapon.WeaponType);
                        }

                        if (
                            weapon == null
                            && !player.ConnectionId.Equals(
                                "mob",
                                StringComparison.CurrentCultureIgnoreCase
                            )
                        )
                        {
                            getWeaponSkill = player.GetSkill(SkillName.Unarmed);
                        }

                        if (getWeaponSkill != null)
                        {
                            getWeaponSkill.Proficiency += 1;
                            Services.Instance.Writer.WriteLine(
                                $"<p class='improve'>Your proficiency in {getWeaponSkill.Name.ToString()} has increased.</p>",
                                player
                            );

                            player.GainExperiencePoints(getWeaponSkill.Level * 50, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SetupInitiative(Player player)
        {
            if (player.Lag > 0)
                return;

            var attackCount = 1;

            var hasSecondAttack = player.HasSkill(SkillName.SecondAttack);

            var hasThirdAttack = player.HasSkill(SkillName.ThirdAttack);

            var hasForthAttack = player.HasSkill(SkillName.FourthAttack);

            var hasFithAttack = player.HasSkill(SkillName.FifthAttack);

            if (hasSecondAttack)
            {
                attackCount += 1;
            }

            if (hasThirdAttack)
            {
                attackCount += 1;
            }

            if (hasForthAttack)
            {
                attackCount += 1;
            }

            if (hasFithAttack)
            {
                attackCount += 1;
            }

            if (player.Affects.Haste)
            {
                attackCount += 1;
            }

            for (var i = 0; i < attackCount; i++)
            {
                _combatQueue.Enqueue(player);
            }
        }
    }
}
