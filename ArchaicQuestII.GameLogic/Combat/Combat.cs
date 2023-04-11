using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Combat
{
    public class Combat : ICombat
    {
        private readonly IQuestLog _quest;
        private readonly IRandomItem _randomItem;

        public Combat(IQuestLog quest, IRandomItem randomItem)
        {
            _quest = quest;
            _randomItem = randomItem;
        }

        // TODO: explain that player needs to be murdered
        public Player FindTarget(Player attacker, string target, Room room, bool isMurder)
        {
            if (string.IsNullOrEmpty(target))
            {
                return null;
            }
            // If mob
            if (isMurder && attacker.ConnectionId != "mob")
            {
                return room.Players.FirstOrDefault(
                    x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)
                );
            }

            if (attacker.ConnectionId == "mob")
            {
                return room.Players.FirstOrDefault(
                    x => x.Name.Equals(target, StringComparison.CurrentCultureIgnoreCase)
                );
            }

            return room.Mobs.FirstOrDefault(
                x =>
                    x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)
                    && x.IsHiddenScriptMob == false
            );
        }

        public Item.Item GetWeapon(Player player, bool dualWield = false)
        {
            return dualWield ? player.Equipped.Secondary : player.Equipped.Wielded;
        }

        public void DisplayDamage(
            Player player,
            Player target,
            Room room,
            Item.Item weapon,
            int damage
        )
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
                player.ConnectionId
            );
            Services.Instance.Writer.WriteLine(
                $"<p class='combat'>{target.Name} {Services.Instance.Formulas.TargetHealth(player, target)}.</p>",
                player.ConnectionId
            );

            Services.Instance.Writer.WriteLine(
                $"<p>{player.Name}'s {(damageType != "none" ? damageType : "")} {attackType} {damText.Value} you. <span class='damage'>[{damage}]</span></p></p>",
                target.ConnectionId
            );

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name || pc.Name == target.Name)
                {
                    continue;
                }

                Services.Instance.Writer.WriteLine(
                    $"<p>{player.Name}'s {attackType} {damText.Value} {target.Name.ToLower(cc)}.</p>",
                    pc.ConnectionId
                );
            }
        }

        public void DisplayMiss(Player player, Player target, Room room, Item.Item weapon)
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
                player.ConnectionId
            );
            Services.Instance.Writer.WriteLine(
                $"<p class='combat'>{player.Name}'s {attackType} misses you.</p>",
                target.ConnectionId
            );

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name || pc.Name == target.Name)
                {
                    continue;
                }

                Services.Instance.Writer.WriteLine(
                    $"<p>{player.Name}'s {attackType} misses {target.Name.ToLower(cc)}.</p>",
                    pc.ConnectionId
                );
            }
        }

        public void InitFightStatus(Player player, Player target)
        {
            player.Target = string.IsNullOrEmpty(player.Target) ? target.Name : player.Target;
            player.Status = CharacterStatus.Status.Fighting;
            target.Status =
                (target.Status & CharacterStatus.Status.Stunned) != 0
                    ? CharacterStatus.Status.Stunned
                    : CharacterStatus.Status.Fighting;
            target.Target = string.IsNullOrEmpty(target.Target) ? player.Name : target.Target; //for group combat, if target is ganged, there target should not be changed when combat is initiated.

            if (player.Target == player.Name)
            {
                player.Status = CharacterStatus.Status.Standing;
                return;
            }

            if (!Services.Instance.Cache.IsCharInCombat(player.Id.ToString()))
            {
                Services.Instance.Cache.AddCharToCombat(player.Id.ToString(), player);
            }

            if (!Services.Instance.Cache.IsCharInCombat(target.Id.ToString()))
            {
                Services.Instance.Cache.AddCharToCombat(target.Id.ToString(), target);
            }
        }

        // TODO refactor FIGHT init
        // currently can spam kill because of the free init hit
        public void Fight(Player player, string victim, Room room, bool isMurder)
        {
            if (victim == "k" || victim == "kill")
            {
                Services.Instance.Writer.WriteLine("<p>Kill whom?<p>", player.ConnectionId);
                return;
            }

            try
            {
                /*if (player.Status == CharacterStatus.Status.Fighting)
                {
                    Services.Instance.Writer.WriteLine("You will try your best.", player.ConnectionId);
                    return;
                }*/

                if (player.Affects.Stunned)
                {
                    Services.Instance.Writer.WriteLine(
                        "<p>You are too stunned to attack this round.<p>",
                        player.ConnectionId
                    );
                    return;
                }
                // refactor this, makes no sense
                // murder command need its on'y check here should be generic find the target player or mob
                var target =
                    FindTarget(player, victim, room, isMurder)
                    ?? FindTarget(player, victim, room, true);

                if (target == null)
                {
                    if (player.Status == CharacterStatus.Status.Fighting)
                    {
                        player.Target = "";
                        player.Status = CharacterStatus.Status.Standing;

                        Services.Instance.Cache.RemoveCharFromCombat(player.Id.ToString());
                        return;
                    }

                    Services.Instance.Writer.WriteLine(
                        "<p>They are not here.</p>",
                        player.ConnectionId
                    );
                    return;
                }

                if (target.Name == player.Name)
                {
                    Services.Instance.Writer.WriteLine(
                        "<p>You can't start a fight with yourself!</p>",
                        player.ConnectionId
                    );
                    return;
                }

                if (player.Attributes.Attribute[EffectLocation.Hitpoints] <= 0)
                {
                    Services.Instance.Writer.WriteLine(
                        "<p>You cannot do that while dead.</p>",
                        player.ConnectionId
                    );
                    return;
                }

                if (target.Attributes.Attribute[EffectLocation.Hitpoints] <= 0)
                {
                    Services.Instance.Writer.WriteLine(
                        "<p>They are already dead.</p>",
                        player.ConnectionId
                    );

                    player.Target = String.Empty;
                    return;
                }

                // For the UI to create a nice gap between rounds of auto attacks
                Services.Instance.Writer.WriteLine(
                    $"<p class='combat-start'></p>",
                    player.ConnectionId
                );

                player.Target = target.Name;
                player.Status = CharacterStatus.Status.Fighting;
                target.Status = CharacterStatus.Status.Fighting;
                target.Target = string.IsNullOrEmpty(target.Target) ? player.Name : target.Target; //for group combat, if target is ganged, there target should not be changed when combat is initiated.

                if (!Services.Instance.Cache.IsCharInCombat(player.Id.ToString()))
                {
                    Services.Instance.Cache.AddCharToCombat(player.Id.ToString(), player);
                }

                if (!Services.Instance.Cache.IsCharInCombat(target.Id.ToString()))
                {
                    Services.Instance.Cache.AddCharToCombat(target.Id.ToString(), target);
                }

                /*
                 *  This section crying out for a refactor
                 */


                var weapon = GetWeapon(player);
                var chanceToHit = Services.Instance.Formulas.ToHitChance(player, target, false);

                if (chanceToHit < 5)
                {
                    chanceToHit = 5;
                }

                //if player bind and don't have blind fighting
                // reduce chance to hit by 40%
                if (player.Affects.Blind && !BlindFighting(player))
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
                                target.ConnectionId
                            );
                            Services.Instance.Writer.WriteLine(
                                $"<p>{target.Name} dodges your attack.</p>",
                                player.ConnectionId
                            );
                        }
                        else
                        {
                            player.FailedSkill(SkillName.Dodge, true);
                            Services.Instance.Writer.WriteLine(
                                $"<p>You fail to dodge {player.Name}'s attack.</p>",
                                target.ConnectionId
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
                                target.ConnectionId
                            );
                            Services.Instance.Writer.WriteLine(
                                $"<p>{target.Name} parries your attack.</p>",
                                player.ConnectionId
                            );

                            var riposte = target.GetSkill(SkillName.Riposte);

                            if (riposte != null && avoidanceChance <= riposte.Proficiency)
                            {
                                Services.Instance.Writer.WriteLine(
                                    $"<p>You riposte {player.Name}'s attack.</p>",
                                    target.ConnectionId
                                );
                                Services.Instance.Writer.WriteLine(
                                    $"<p>{target.Name} riposte's your attack.</p>",
                                    player.ConnectionId
                                );

                                var ripDamage = Services.Instance.Formulas.CalculateDamage(
                                    target,
                                    player,
                                    weapon
                                );

                                ripDamage /= 3;

                                target.HarmTarget(ripDamage);

                                DisplayDamage(target, player, room, weapon, ripDamage);

                                Services.Instance.UpdateClient.UpdateHP(player);

                                if (!target.IsAlive())
                                {
                                    TargetKilled(target, player, room);
                                }
                            }
                            else
                            {
                                player.FailedSkill(SkillName.Riposte, true);
                                Services.Instance.Writer.WriteLine(
                                    $"<p>You fail to riposte {player.Name}'s attack.</p>",
                                    target.ConnectionId
                                );
                            }
                        }
                        else
                        {
                            player.FailedSkill(SkillName.Parry, true);
                            Services.Instance.Writer.WriteLine(
                                $"<p>You fail to parry {player.Name}'s attack.</p>",
                                target.ConnectionId
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
                                    target.ConnectionId
                                );
                                Services.Instance.Writer.WriteLine(
                                    $"{target.Name} blocks your attack with their shield.",
                                    player.ConnectionId
                                );
                                return;
                            }
                        }
                        else
                        {
                            player.FailedSkill(SkillName.ShieldBlock, true);
                            Services.Instance.Writer.WriteLine(
                                $"<p>You fail to block {player.Name}'s attack.</p>",
                                target.ConnectionId
                            );
                        }
                    }

                    var damage = Services.Instance.Formulas.CalculateDamage(player, target, weapon);

                    var enhancedDamageChance = DiceBag.Roll(1, 1, 100);
                    var hasEnhancedDamage = player.Skills.FirstOrDefault(
                        x => x.Name.Equals("Enhanced Damage")
                    );

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
                    player.HarmTarget(damage);

                    DisplayDamage(player, target, room, weapon, damage);

                    Services.Instance.UpdateClient.UpdateHP(target);

                    if (!target.IsAlive())
                    {
                        TargetKilled(player, target, room);
                    }
                }
                else
                {
                    Services.Instance.UpdateClient.PlaySound("miss", target);
                    Services.Instance.UpdateClient.PlaySound("miss", player);
                    DisplayMiss(player, target, room, weapon);
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
                        getWeaponSkill = player.Skills.FirstOrDefault(
                            x => x.Name == weapon.WeaponType
                        );
                    }

                    if (
                        weapon == null
                        && !player.ConnectionId.Equals(
                            "mob",
                            StringComparison.CurrentCultureIgnoreCase
                        )
                    )
                    {
                        getWeaponSkill = player.Skills.FirstOrDefault(
                            x => x.Name == SkillName.Unarmed
                        );
                    }

                    if (getWeaponSkill != null && getWeaponSkill.Proficiency < 100)
                    {
                        getWeaponSkill.Proficiency += 1;
                        Services.Instance.Writer.WriteLine(
                            $"<p class='improve'>Your proficiency in {getWeaponSkill.Name.ToString()} has increased.</p>",
                            player.ConnectionId
                        );

                        player.GainExperiencePoints(getWeaponSkill.Level * 50, true);
                    }
                }

                if (player.Equipped.Secondary != null)
                {
                    weapon = GetWeapon(player, true);
                    chanceToHit = Services.Instance.Formulas.ToHitChance(player, target, true);

                    if (player.ConnectionId == "mob" && chanceToHit < 45)
                    {
                        chanceToHit = 45;
                    }

                    //if player bind and don't have blind fighting
                    // reduce chance to hit by 40%
                    if (player.Affects.Blind && !BlindFighting(player))
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
                                    target.ConnectionId
                                );
                                Services.Instance.Writer.WriteLine(
                                    $"<p>{target.Name} dodges your attack.</p>",
                                    player.ConnectionId
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
                                    target.ConnectionId
                                );
                                Services.Instance.Writer.WriteLine(
                                    $"<p>{target.Name} parries your attack.</p>",
                                    player.ConnectionId
                                );

                                var riposte = target.GetSkill(SkillName.Riposte);

                                if (riposte != null)
                                {
                                    Services.Instance.Writer.WriteLine(
                                        $"<p>You riposte {player.Name}'s attack.</p>",
                                        target.ConnectionId
                                    );
                                    Services.Instance.Writer.WriteLine(
                                        $"<p>{target.Name} riposte's your attack.</p>",
                                        player.ConnectionId
                                    );

                                    var ripDamage = Services.Instance.Formulas.CalculateDamage(
                                        target,
                                        player,
                                        weapon
                                    );

                                    ripDamage /= 3;

                                    player.HarmTarget(ripDamage);

                                    DisplayDamage(target, player, room, weapon, ripDamage);

                                    Services.Instance.UpdateClient.UpdateHP(player);

                                    if (!player.IsAlive())
                                    {
                                        TargetKilled(target, player, room);
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
                                        target.ConnectionId
                                    );
                                    Services.Instance.Writer.WriteLine(
                                        $"{target.Name} blocks your attack with their shield.",
                                        player.ConnectionId
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
                        var hasEnhancedDamage = player.Skills.FirstOrDefault(
                            x => x.Name.Equals("Enhanced Damage")
                        );

                        if (hasEnhancedDamage != null)
                        {
                            if (hasEnhancedDamage.Proficiency >= enhancedDamageChance)
                            {
                                var bonusDam = Helpers.GetPercentage(15, damage);
                                damage += bonusDam;
                            }
                        }

                        target.HarmTarget(damage);

                        DisplayDamage(player, target, room, weapon, damage);

                        Services.Instance.UpdateClient.UpdateHP(target);

                        if (!target.IsAlive())
                        {
                            TargetKilled(player, target, room);
                        }
                    }
                    else
                    {
                        DisplayMiss(player, target, room, weapon);
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
                            getWeaponSkill = player.Skills.FirstOrDefault(
                                x => x.Name == weapon.WeaponType
                            );
                        }

                        if (
                            weapon == null
                            && !player.ConnectionId.Equals(
                                "mob",
                                StringComparison.CurrentCultureIgnoreCase
                            )
                        )
                        {
                            getWeaponSkill = player.Skills.FirstOrDefault(
                                x => x.Name == SkillName.Unarmed
                            );
                        }

                        if (getWeaponSkill != null)
                        {
                            getWeaponSkill.Proficiency += 1;
                            Services.Instance.Writer.WriteLine(
                                $"<p class='improve'>Your proficiency in {getWeaponSkill.Name.ToString()} has increased.</p>",
                                player.ConnectionId
                            );

                            player.GainExperiencePoints(getWeaponSkill.Level * 50, true);
                        }
                    }
                }

                if (player.Grouped)
                {
                    foreach (var follower in player.Followers)
                    {
                        if (
                            follower.Grouped
                            && follower.Following == player.Name
                            && follower.Status != CharacterStatus.Status.Fighting
                        )
                        {
                            if (follower.Config.AutoAssist && string.IsNullOrEmpty(follower.Target))
                            {
                                follower.Buffer.Clear();
                                follower.Target = player.Target;
                                follower.Status = CharacterStatus.Status.Fighting;

                                if (!Services.Instance.Cache.IsCharInCombat(follower.Id.ToString()))
                                {
                                    Services.Instance.Cache.AddCharToCombat(
                                        follower.Id.ToString(),
                                        follower
                                    );
                                }
                            }
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

        public int DamageReduction(Player defender, int damage)
        {
            var ArRating = defender.ArmorRating.Armour + 1;

            decimal damageAfterReduction =
                damage
                * (
                    damage
                    / (
                        damage
                        + ArRating
                        + (defender.Attributes.Attribute[EffectLocation.Dexterity] / 4)
                    )
                );

            return (int)Math.Round(damageAfterReduction, MidpointRounding.ToEven);
        }

        public int CalculateSkillDamage(Player player, Player target, int damage)
        {
            var strengthMod = Math.Round(
                (decimal)(player.Attributes.Attribute[EffectLocation.Strength] - 20) / 2,
                MidpointRounding.ToEven
            );
            var levelDif = target.Level - player.Level <= 0 ? 0 : target.Level - player.Level;

            var totalDamage =
                damage
                + strengthMod
                + levelDif
                + player.Attributes.Attribute[EffectLocation.DamageRoll];

            var criticalHit = 1;

            if (
                target.Status == CharacterStatus.Status.Sleeping
                || target.Status == CharacterStatus.Status.Stunned
                || target.Status == CharacterStatus.Status.Resting
            )
            {
                criticalHit = 2;
            }

            totalDamage *= criticalHit;

            // calculate damage reduction based on target armour
            var DamageAfterArmourReduction = DamageReduction(target, (int)totalDamage);

            return DamageAfterArmourReduction;
        }

        public void TargetKilled(Player player, Player target, Room room)
        {
            player.Target = string.Empty;
            player.Status = CharacterStatus.Status.Standing;
            target.Status = CharacterStatus.Status.Ghost;
            target.Target = string.Empty;

            target.DeathCry(room);

            if (player.Grouped)
            {
                // other group members to drop from combat if they're fighting the same target
                // other group members status set to standing

                var isGroupLeader = string.IsNullOrEmpty(player.Following);

                var groupLeader = player;

                if (!isGroupLeader)
                {
                    groupLeader = Services.Instance.Cache
                        .GetPlayerCache()
                        .FirstOrDefault(x => x.Value.Name.Equals(player.Following))
                        .Value;
                }

                var exp = target.GetExpWorth() / (groupLeader.Followers.Count + 1);
                groupLeader.GainExperiencePoints(exp, true);

                foreach (
                    var follower in groupLeader.Followers.Where(
                        follower => follower.Grouped && follower.Following == groupLeader.Name
                    )
                )
                {
                    follower.GainExperiencePoints(exp, true);
                    follower.Status = CharacterStatus.Status.Standing;
                    follower.Target = string.Empty;
                    Services.Instance.Cache.RemoveCharFromCombat(follower.Id.ToString());
                }
            }
            else
            {
                player.GainExperiencePoints(target, true);
            }

            _quest.IsQuestMob(player, target.Name);

            if (target.ConnectionId != "mob")
            {
                Helpers.PostToDiscord(
                    $"{target.Name} got killed by {player.Name}!",
                    "event",
                    Services.Instance.Cache.GetConfig()
                );

                if (player.ConnectionId != "mob")
                {
                    target.PlayerDeaths += 1;
                    player.PlayerKills += 1;
                }
                else
                {
                    target.MobDeaths += 1;
                }
            }

            Services.Instance.Writer.WriteLine(
                "<p class='dead'>You are dead. R.I.P.</p>",
                target.ConnectionId
            );

            var targetName = target.Name.ToLower(CultureInfo.CurrentCulture);
            var corpse = new Item.Item
            {
                Name = $"The corpse of {targetName}",
                Description = new Description
                {
                    Room = $"The corpse of {targetName} is laying here.",
                    Exam = target.Description,
                    Look = target.Description,
                },
                Slot = EquipmentSlot.Held,
                Level = 1,
                Stuck = true,
                Container = new Container
                {
                    Items = new ItemList(),
                    CanLock = false,
                    IsOpen = true,
                    CanOpen = false,
                },
                ItemType = Item.Item.ItemTypes.Container,
                Decay = target.ConnectionId.Equals("mob", StringComparison.OrdinalIgnoreCase)
                    ? 10
                    : 20,
                DecayTimer = 300 // 5 minutes,
            };

            foreach (var item in target.Inventory)
            {
                item.Equipped = false;
                corpse.Container.Items.Add(item);
            }

            // clear list
            target.Inventory = new ItemList();
            // clear equipped
            target.Equipped = new Equipment();

            var mount = target.Pets.FirstOrDefault(x => x.Name.Equals(target.Mounted.Name));
            if (mount != null)
            {
                target.Pets.Remove(mount);
                target.Mounted.Name = string.Empty;
            }

            // add corpse to room
            room.Items.Add(corpse);
            Services.Instance.UpdateClient.UpdateInventory(target);
            Services.Instance.UpdateClient.UpdateEquipment(target);
            Services.Instance.UpdateClient.UpdateScore(target);
            Services.Instance.UpdateClient.UpdateScore(player);

            if (target.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
            {
                player.MobKills += 1;

                var randomItem = _randomItem.WeaponDrop(player);

                if (randomItem != null)
                {
                    corpse.Container.Items.Add(randomItem);
                }

                var command = Services.Instance.Cache.GetCommand("get");
                if (player.Config.AutoLoot && command != null)
                {
                    var corpseIndex = room.Items.IndexOf(corpse) + 1;
                    command.Execute(player, room, new[] { "get", "all", $"{corpseIndex}.corpse" });
                }
            }

            room.Clean = false;

            Services.Instance.Cache.RemoveCharFromCombat(target.Id.ToString());
            Services.Instance.Cache.RemoveCharFromCombat(player.Id.ToString());

            if (target.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
            {
                var command = Services.Instance.Cache.GetCommand("sacrifice");

                if (player.Config.AutoSacrifice && command != null)
                {
                    command.Execute(player, room, new[] { "sacrifice", corpse.Name });
                }

                room.Mobs.Remove(target);
                var getTodayMobStats = Services.Instance.PlayerDataBase
                    .GetList<MobStats>(PlayerDataBase.Collections.MobStats)
                    .FirstOrDefault(x => x.Date.Date.Equals(DateTime.Today));

                if (getTodayMobStats != null)
                {
                    getTodayMobStats.MobKills += 1;
                }
                else
                {
                    getTodayMobStats = new MobStats()
                    {
                        MobKills = 1,
                        PlayerDeaths = 0,
                        Date = DateTime.Now,
                    };
                }
                Services.Instance.PlayerDataBase.Save(
                    getTodayMobStats,
                    PlayerDataBase.Collections.MobStats
                );
            }
            else
            {
                room.Players.Remove(target);
                var getTodayMobStats = Services.Instance.PlayerDataBase
                    .GetList<MobStats>(PlayerDataBase.Collections.MobStats)
                    .FirstOrDefault(x => x.Date.Date.Equals(DateTime.Today));

                if (getTodayMobStats != null)
                {
                    getTodayMobStats.PlayerDeaths += 1;
                }
                else
                {
                    getTodayMobStats = new MobStats()
                    {
                        MobKills = 0,
                        PlayerDeaths = 1,
                        Date = DateTime.Now,
                    };
                }
                Services.Instance.PlayerDataBase.Save(
                    getTodayMobStats,
                    PlayerDataBase.Collections.MobStats
                );
            }
            // take player to Temple / recall area

            if (target.ConnectionId != "mob")
            {
                target.Status = CharacterStatus.Status.Resting;
                var newRoom = Services.Instance.Cache.GetRoom(target.RecallId);

                target.Buffer = new Queue<string>();

                target.RoomId = Helpers.ReturnRoomId(newRoom);

                newRoom.Players.Add(target);
                target.Buffer.Enqueue("look");
            }
        }

        /// <summary>
        /// 40% chance to hit reduce if no blind fighting
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool BlindFighting(Player player)
        {
            var foundSkill = player.Skills.FirstOrDefault(x => x.Name == SkillName.BlindFighting);

            if (foundSkill == null)
            {
                return false;
            }

            var getSkill = Services.Instance.Cache.GetSkill(foundSkill.Id);

            if (getSkill == null)
            {
                var skill = Services.Instance.Cache
                    .GetAllSkills()
                    .FirstOrDefault(
                        x =>
                            x.Name.Equals(
                                "blind fighting",
                                StringComparison.CurrentCultureIgnoreCase
                            )
                    );
                foundSkill.Id = skill.Id;
                getSkill = skill;
            }

            var proficiency = foundSkill.Proficiency;
            var success = DiceBag.Roll(1, 1, 100);

            if (success == 1 || success == 101)
            {
                return false;
            }

            //TODO Charisma Check
            if (proficiency >= success)
            {
                return true;
            }

            if (foundSkill.Proficiency == 100)
            {
                return false;
            }

            var increase = DiceBag.Roll(1, 1, 5);

            foundSkill.Proficiency += increase;

            player.GainExperiencePoints(100 * foundSkill.Level / 4, true);

            Services.Instance.UpdateClient.UpdateExp(player);

            return false;
        }
    }
}
