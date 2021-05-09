using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells
{
    public interface IDamageSpells
    {
        public void updateCombat(Player player, Player target);
        public string ReplacePlaceholders(string str, Player player, bool isTarget);

        public void DamagePlayer(string spellName, int damage, Player player, Player target, Room room);

        public bool AffectPlayerAttributes(string spellName, EffectLocation attribute, int value, Player player,
            Player target, Room room, string noAffect);

        public void UpdateClientUI(Player player);

        public void EmoteAction(Player player, Player target, Room room, SkillMessage emote);

        public void EmoteEffectWearOffAction(Player player, Room room, SkillMessage emote);

        public void CastSpell(string key, string obj, Player target, string fullCommand, Player player, Room room, bool wearOff);
    }
}
