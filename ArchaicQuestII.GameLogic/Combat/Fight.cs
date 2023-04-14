using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Combat
{
    public class Fight
    {
        public Guid Id = new Guid();
        private ConcurrentDictionary<string, Combatant> _combatants =
            new ConcurrentDictionary<string, Combatant>();
        private Queue<Combatant> _combatQueue = new Queue<Combatant>();
        private Room _room;
        private bool _isMurder;

        public Fight(Player aggressor, Player victim, Room room, bool isMurder)
        {
            _room = room;
            _isMurder = isMurder;

            _combatants.TryAdd(aggressor.Name, new Combatant(aggressor, true));
            aggressor.Status = CharacterStatus.Status.Fighting;
            aggressor.Combat = this;

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
                            follower.Status = CharacterStatus.Status.Fighting;
                            _combatants.TryAdd(follower.Name, new Combatant(follower, true));
                            follower.Combat = this;
                        }
                    }
                }
            }

            _combatants.TryAdd(victim.Name, new Combatant(victim, false));
            victim.Status =
                (victim.Status & CharacterStatus.Status.Stunned) != 0
                    ? CharacterStatus.Status.Stunned
                    : CharacterStatus.Status.Fighting;
            victim.Combat = this;

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
                            follower.Status = CharacterStatus.Status.Fighting;
                            _combatants.TryAdd(follower.Name, new Combatant(follower, false));
                            follower.Combat = this;
                        }
                    }
                }
            }

            foreach (var c in _combatants.Values.Where(x => x.aggressor))
            {
                Services.Instance.Writer.WriteLine("<p>You initiated combat!</p>", c.player);
            }

            foreach (var c in _combatants.Values.Where(x => x.aggressor == false))
            {
                Services.Instance.Writer.WriteLine("<p>You have been attacked!</p>", c.player);
            }

            Services.Instance.Cache.AddCombat(this);
        }

        private void End()
        {
            foreach (var combatant in _combatants.Values)
            {
                combatant.player.Status = CharacterStatus.Status.Standing;
                combatant.player.Combat = null;
                combatant.player.Buffer.Clear();
                combatant.player.Target = null;
                Services.Instance.Writer.WriteLine("<p>Combat has ended.</p>", combatant.player);
            }

            _combatants.Clear();
            _combatQueue.Clear();

            Services.Instance.Cache.RemoveCombat(this);
        }

        public void AddCommand(Player player, string command)
        {
            _combatants.TryGetValue(player.Name, out var combatant);
            combatant.Command = command;
        }

        public void Do()
        {
            foreach (var c in _combatants.Values)
            {
                SetupInitiative(c);
            }

            while (_combatQueue.Count > 0)
            {
                var combatant = _combatQueue.Dequeue();

                if (
                    combatant.player.IsAlive()
                    && combatant.player.RoomId == Helpers.ReturnRoomId(_room)
                )
                {
                    SetTarget(combatant);
                    if (combatant.target != null)
                        DoRound(combatant);
                }

                CheckIfMoreCombatants();
            }
        }

        private void DoRound(Combatant combatant)
        {
            // For the UI to create a nice gap between rounds of auto attacks
            Services.Instance.Writer.WriteLine($"<p class='combat-start'></p>", combatant.player);

            if (!string.IsNullOrEmpty(combatant.Command))
            {
                Services.Instance.CommandHandler.HandleCommand(
                    combatant.player,
                    _room,
                    combatant.Command
                );

                combatant.Command = null;

                return;
            }

            if (combatant.player.Affects.Stunned)
            {
                Services.Instance.Writer.WriteLine(
                    "<p>You are too stunned to attack this round.<p>",
                    combatant.player
                );
                return;
            }

            var weapon = combatant.player.GetWeapon();

            var chanceToHit = Services.Instance.Formulas.ToHitChance(
                combatant.player,
                combatant.target,
                false
            );

            if (chanceToHit < 5)
            {
                chanceToHit = 5;
            }

            //if player bind and don't have blind fighting
            // reduce chance to hit by 40%
            if (combatant.player.Affects.Blind && !CombatHandler.BlindFighting(combatant.player))
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
                    var dodge = combatant.target.GetSkill(SkillName.Dodge);

                    if (dodge != null && avoidanceChance <= dodge.Proficiency)
                    {
                        Services.Instance.Writer.WriteLine(
                            $"<p>You dodge {combatant.player.Name}'s attack.</p>",
                            combatant.target
                        );
                        Services.Instance.Writer.WriteLine(
                            $"<p>{combatant.target.Name} dodges your attack.</p>",
                            combatant.player
                        );
                    }
                    else
                    {
                        combatant.player.FailedSkill(SkillName.Dodge, true);
                        Services.Instance.Writer.WriteLine(
                            $"<p>You fail to dodge {combatant.player.Name}'s attack.</p>",
                            combatant.target
                        );
                    }
                }

                //10% chance to parry
                if (avoidanceRoll == 2)
                {
                    var skill = combatant.target.GetSkill(SkillName.Parry);

                    if (skill != null && avoidanceChance <= skill.Proficiency)
                    {
                        Services.Instance.Writer.WriteLine(
                            $"<p>You parry {combatant.player.Name}'s attack.</p>",
                            combatant.target
                        );
                        Services.Instance.Writer.WriteLine(
                            $"<p>{combatant.target.Name} parries your attack.</p>",
                            combatant.player
                        );

                        var riposte = combatant.target.GetSkill(SkillName.Riposte);

                        if (riposte != null && avoidanceChance <= riposte.Proficiency)
                        {
                            Services.Instance.Writer.WriteLine(
                                $"<p>You riposte {combatant.player.Name}'s attack.</p>",
                                combatant.target
                            );
                            Services.Instance.Writer.WriteLine(
                                $"<p>{combatant.target.Name} riposte's your attack.</p>",
                                combatant.player
                            );

                            var ripDamage = Services.Instance.Formulas.CalculateDamage(
                                combatant.target,
                                combatant.player,
                                weapon
                            );

                            ripDamage /= 3;

                            combatant.target.Harm(ripDamage);

                            DisplayDamage(combatant.target, combatant.player, weapon, ripDamage);

                            Services.Instance.UpdateClient.UpdateHP(combatant.player);

                            if (!combatant.target.IsAlive())
                            {
                                CombatHandler.TargetKilled(combatant, _room);
                                return;
                            }
                        }
                        else
                        {
                            combatant.player.FailedSkill(SkillName.Riposte, true);
                            Services.Instance.Writer.WriteLine(
                                $"<p>You fail to riposte {combatant.player.Name}'s attack.</p>",
                                combatant.target
                            );
                        }
                    }
                    else
                    {
                        combatant.player.FailedSkill(SkillName.Parry, true);
                        Services.Instance.Writer.WriteLine(
                            $"<p>You fail to parry {combatant.player.Name}'s attack.</p>",
                            combatant.target
                        );
                    }
                }

                // Block
                if (avoidanceRoll == 3 && combatant.player.Equipped.Shield != null)
                {
                    var chanceToBlock = Services.Instance.Formulas.ToBlockChance(
                        combatant.target,
                        combatant.player
                    );
                    var doesBlock = Services.Instance.Formulas.DoesHit(chanceToBlock);

                    if (doesBlock)
                    {
                        var skill = combatant.target.GetSkill(SkillName.ShieldBlock);

                        if (skill != null)
                        {
                            Services.Instance.UpdateClient.PlaySound("block", combatant.player);
                            Services.Instance.UpdateClient.PlaySound("block", combatant.target);
                            Services.Instance.Writer.WriteLine(
                                $"You block {combatant.player.Name}'s attack with your shield.",
                                combatant.target
                            );
                            Services.Instance.Writer.WriteLine(
                                $"{combatant.target.Name} blocks your attack with their shield.",
                                combatant.player
                            );
                            return;
                        }
                    }
                    else
                    {
                        combatant.player.FailedSkill(SkillName.ShieldBlock, true);
                        Services.Instance.Writer.WriteLine(
                            $"<p>You fail to block {combatant.player.Name}'s attack.</p>",
                            combatant.target
                        );
                    }
                }

                var damage = Services.Instance.Formulas.CalculateDamage(
                    combatant.player,
                    combatant.target,
                    weapon
                );

                var enhancedDamageChance = DiceBag.Roll(1, 1, 100);
                var hasEnhancedDamage = combatant.player.GetSkill(SkillName.EnhancedDamage);

                if (Services.Instance.Formulas.IsCriticalHit())
                {
                    // double damage
                    damage *= 2;
                }

                if (hasEnhancedDamage != null)
                {
                    if (
                        hasEnhancedDamage.Proficiency >= enhancedDamageChance
                        && combatant.player.Level >= hasEnhancedDamage.Level
                    )
                    {
                        var bonusDam = Helpers.GetPercentage(15, damage);
                        damage += bonusDam;
                    }
                }

                Services.Instance.UpdateClient.PlaySound("hit", combatant.target);
                Services.Instance.UpdateClient.PlaySound("hit", combatant.player);

                combatant.target.Harm(damage);

                DisplayDamage(combatant.player, combatant.target, weapon, damage);

                if (!combatant.target.IsAlive())
                {
                    CombatHandler.TargetKilled(combatant, _room);
                    return;
                }
            }
            else
            {
                Services.Instance.UpdateClient.PlaySound("miss", combatant.target);
                Services.Instance.UpdateClient.PlaySound("miss", combatant.player);
                DisplayMiss(combatant.player, combatant.target, weapon);
                // miss message
                // gain improvements on weapon skill


                SkillList getWeaponSkill = null;
                if (
                    weapon != null
                    && !combatant.player.ConnectionId.Equals(
                        "mob",
                        StringComparison.CurrentCultureIgnoreCase
                    )
                )
                {
                    // urgh this is ugly
                    getWeaponSkill = combatant.player.GetSkill(weapon.WeaponType);
                }

                if (
                    weapon == null
                    && !combatant.player.ConnectionId.Equals(
                        "mob",
                        StringComparison.CurrentCultureIgnoreCase
                    )
                )
                {
                    getWeaponSkill = combatant.player.GetSkill(SkillName.Unarmed);
                }

                if (getWeaponSkill != null && getWeaponSkill.Proficiency < 100)
                {
                    getWeaponSkill.Proficiency += 1;
                    Services.Instance.Writer.WriteLine(
                        $"<p class='improve'>Your proficiency in {getWeaponSkill.Name.ToString()} has increased.</p>",
                        combatant.player
                    );

                    combatant.player.GainExperiencePoints(getWeaponSkill.Level * 50, true);
                }
            }

            if (combatant.player.Equipped.Secondary != null)
            {
                weapon = combatant.player.GetWeapon(true);
                chanceToHit = Services.Instance.Formulas.ToHitChance(
                    combatant.player,
                    combatant.target,
                    true
                );

                if (combatant.player.ConnectionId == "mob" && chanceToHit < 45)
                {
                    chanceToHit = 45;
                }

                //if player bind and don't have blind fighting
                // reduce chance to hit by 40%
                if (
                    combatant.player.Affects.Blind && !CombatHandler.BlindFighting(combatant.player)
                )
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
                        var dodge = combatant.target.GetSkill(SkillName.Dodge);

                        if (dodge != null)
                        {
                            Services.Instance.Writer.WriteLine(
                                $"<p>You dodge {combatant.player.Name}'s attack.</p>",
                                combatant.target
                            );
                            Services.Instance.Writer.WriteLine(
                                $"<p>{combatant.target.Name} dodges your attack.</p>",
                                combatant.player
                            );
                            return;
                        }
                    }

                    //10% chance to parry
                    if (avoidanceRoll == 2)
                    {
                        var skill = combatant.target.GetSkill(SkillName.Parry);

                        if (skill != null)
                        {
                            Services.Instance.UpdateClient.PlaySound("parry", combatant.player);
                            Services.Instance.UpdateClient.PlaySound("parry", combatant.target);
                            Services.Instance.Writer.WriteLine(
                                $"<p>You parry {combatant.player.Name}'s attack.</p>",
                                combatant.target
                            );
                            Services.Instance.Writer.WriteLine(
                                $"<p>{combatant.target.Name} parries your attack.</p>",
                                combatant.player
                            );

                            var riposte = combatant.target.GetSkill(SkillName.Riposte);

                            if (riposte != null)
                            {
                                Services.Instance.Writer.WriteLine(
                                    $"<p>You riposte {combatant.player.Name}'s attack.</p>",
                                    combatant.target
                                );
                                Services.Instance.Writer.WriteLine(
                                    $"<p>{combatant.target.Name} riposte's your attack.</p>",
                                    combatant.player
                                );

                                var ripDamage = Services.Instance.Formulas.CalculateDamage(
                                    combatant.target,
                                    combatant.player,
                                    weapon
                                );

                                ripDamage /= 3;

                                combatant.player.Harm(ripDamage);

                                DisplayDamage(
                                    combatant.target,
                                    combatant.player,
                                    weapon,
                                    ripDamage
                                );

                                if (!combatant.player.IsAlive())
                                {
                                    if (
                                        _combatants.TryGetValue(
                                            combatant.target.Name,
                                            out var target
                                        )
                                    )
                                        CombatHandler.TargetKilled(target, _room);
                                    return;
                                }
                            }

                            return;
                        }
                    }

                    // Block
                    if (avoidanceRoll == 3 && combatant.player.Equipped.Shield != null)
                    {
                        var chanceToBlock = Services.Instance.Formulas.ToBlockChance(
                            combatant.target,
                            combatant.player
                        );
                        var doesBlock = Services.Instance.Formulas.DoesHit(chanceToBlock);

                        if (doesBlock)
                        {
                            var skill = combatant.target.GetSkill(SkillName.ShieldBlock);

                            if (skill != null)
                            {
                                Services.Instance.Writer.WriteLine(
                                    $"You block {combatant.player.Name}'s attack with your shield.",
                                    combatant.target
                                );
                                Services.Instance.Writer.WriteLine(
                                    $"{combatant.target.Name} blocks your attack with their shield.",
                                    combatant.player
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
                        combatant.player,
                        combatant.target,
                        weapon
                    );

                    if (Services.Instance.Formulas.IsCriticalHit())
                    {
                        // double damage
                        damage *= 2;
                    }

                    var enhancedDamageChance = DiceBag.Roll(1, 1, 100);
                    var hasEnhancedDamage = combatant.player.GetSkill(SkillName.EnhancedDamage);

                    if (hasEnhancedDamage != null)
                    {
                        if (hasEnhancedDamage.Proficiency >= enhancedDamageChance)
                        {
                            var bonusDam = Helpers.GetPercentage(15, damage);
                            damage += bonusDam;
                        }
                    }

                    combatant.target.Harm(damage);

                    DisplayDamage(combatant.player, combatant.target, weapon, damage);

                    if (!combatant.target.IsAlive())
                    {
                        CombatHandler.TargetKilled(combatant, _room);
                        return;
                    }
                }
                else
                {
                    DisplayMiss(combatant.player, combatant.target, weapon);
                    // miss message
                    // gain improvements on weapon skill


                    SkillList getWeaponSkill = null;
                    if (
                        weapon != null
                        && !combatant.player.ConnectionId.Equals(
                            "mob",
                            StringComparison.CurrentCultureIgnoreCase
                        )
                    )
                    {
                        getWeaponSkill = combatant.player.GetSkill(weapon.WeaponType);
                    }

                    if (
                        weapon == null
                        && !combatant.player.ConnectionId.Equals(
                            "mob",
                            StringComparison.CurrentCultureIgnoreCase
                        )
                    )
                    {
                        getWeaponSkill = combatant.player.GetSkill(SkillName.Unarmed);
                    }

                    if (getWeaponSkill != null)
                    {
                        getWeaponSkill.Proficiency += 1;
                        Services.Instance.Writer.WriteLine(
                            $"<p class='improve'>Your proficiency in {getWeaponSkill.Name.ToString()} has increased.</p>",
                            combatant.player
                        );

                        combatant.player.GainExperiencePoints(getWeaponSkill.Level * 50, true);
                    }
                }
            }
        }

        public void RemoveFromCombat(Player combatant)
        {
            combatant.Combat = null;
            _combatants.Remove(combatant.Name, out _);
        }

        private void CheckIfMoreCombatants()
        {
            bool aggressors = false;
            bool victims = false;

            foreach (var c in _combatants.Values)
            {
                if (c.aggressor)
                    aggressors = true;
                if (!c.aggressor)
                    victims = true;
            }

            if (!aggressors || !victims)
                End();
        }

        private void SetupInitiative(Combatant combatant)
        {
            if (combatant.player.Lag > 0)
                return;

            var attackCount = 1;

            var hasSecondAttack = combatant.player.HasSkill(SkillName.SecondAttack);

            var hasThirdAttack = combatant.player.HasSkill(SkillName.ThirdAttack);

            var hasForthAttack = combatant.player.HasSkill(SkillName.FourthAttack);

            var hasFithAttack = combatant.player.HasSkill(SkillName.FifthAttack);

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

            if (combatant.player.Affects.Haste)
            {
                attackCount += 1;
            }

            for (var i = 0; i < attackCount; i++)
            {
                _combatQueue.Enqueue(combatant);
            }
        }

        private void SetTarget(Combatant combatant)
        {
            combatant.target = _combatants.Values
                .FirstOrDefault(x => x.aggressor != combatant.aggressor)
                .player;

            combatant.player.Target = combatant.target.Name;
        }

        public void DisplayMiss(Player player, Player target, Item.Item weapon)
        {
            CultureInfo cc = CultureInfo.CurrentCulture;
            var attackType = "";
            if (weapon == null)
            {
                attackType = player.ConnectionId.Equals(
                    "mob",
                    StringComparison.CurrentCultureIgnoreCase
                )
                    ? player.DefaultAttack?.ToLower(cc)
                    : "punch";
            }
            else
            {
                attackType = Enum.GetName(typeof(Item.Item.AttackTypes), weapon.AttackType)
                    ?.ToLower(cc);
            }

            Services.Instance.Writer.WriteLine(
                $"<p class='combat'>Your {attackType} misses {target.Name.ToLower(cc)}.</p>",
                player
            );
            Services.Instance.Writer.WriteLine(
                $"<p class='combat'>{player.Name}'s {attackType} misses you.</p>",
                target
            );

            foreach (var pc in _room.Players)
            {
                if (pc.Name == player.Name || pc.Name == target.Name)
                {
                    continue;
                }

                Services.Instance.Writer.WriteLine(
                    $"<p>{player.Name}'s {attackType} misses {target.Name.ToLower(cc)}.</p>",
                    pc
                );
            }
        }

        public void DisplayDamage(Player player, Player target, Item.Item weapon, int damage)
        {
            CultureInfo cc = CultureInfo.CurrentCulture;
            var damText = Services.Instance.Damage.DamageText(damage);
            var attackType = "";
            var damageType = "";
            if (weapon == null)
            {
                attackType = player.ConnectionId.Equals(
                    "mob",
                    StringComparison.CurrentCultureIgnoreCase
                )
                    ? player.DefaultAttack?.ToLower(cc)
                    : "punch";
            }
            else
            {
                attackType = Enum.GetName(typeof(Item.Item.AttackTypes), weapon.AttackType)
                    ?.ToLower(cc);
                damageType = Enum.GetName(typeof(Item.Item.DamageTypes), weapon.DamageType)
                    ?.ToLower(cc);
            }

            Services.Instance.Writer.WriteLine(
                $"<p class='combat'>Your {(damageType != "none" ? damageType : "")} {attackType} {damText.Value} {target.Name.ToLower(cc)}. <span class='damage'>[{damage}]</span></p>",
                player
            );
            Services.Instance.Writer.WriteLine(
                $"<p class='combat'>{target.Name} {Services.Instance.Formulas.TargetHealth(player, target)}.</p>",
                player
            );

            Services.Instance.Writer.WriteLine(
                $"<p>{player.Name}'s {(damageType != "none" ? damageType : "")} {attackType} {damText.Value} you. <span class='damage'>[{damage}]</span></p></p>",
                target
            );

            foreach (var pc in _room.Players)
            {
                if (pc.Name == player.Name || pc.Name == target.Name)
                {
                    continue;
                }

                Services.Instance.Writer.WriteLine(
                    $"<p>{player.Name}'s {attackType} {damText.Value} {target.Name.ToLower(cc)}.</p>",
                    pc
                );
            }
        }
    }
}
