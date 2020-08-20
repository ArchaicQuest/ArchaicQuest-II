﻿using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.Combat
{
   public interface IFormulas
    {
        public int OffensivePoints(Player player);

        public int DefensivePoints(Player player);

        public int BlockPoints(Player player);

        public int ToBlockChance(Player player, Player target);

        public int ToHitChance(Player player, Player target);

        public int DamageReduction(Player defender, int damage);

        public int CalculateDamage(Player player, Player target, Item.Item weapon);

        public bool IsCriticalHit();
        public bool DoesHit(int chance);
    }
}
