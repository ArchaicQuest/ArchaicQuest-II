using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Spell
{
   public class SpellList: ISpellList
    {
        private readonly IDamageSpells _damageSpells;
        public SpellList(IDamageSpells damageSpells)
        {
            _damageSpells = damageSpells;
        }
        public void CastSpell(string key, string obj, Player target, string fullCommand, Player player, Room room, bool wearOff)
        {

            switch (key.ToLower())
            {
                case "magic missile":
                    _damageSpells.MagicMissile(player, target, room);
                    break;
                case "cause light wounds":
                    _damageSpells.CauseLightWounds(player, target, room);
                    break;
                case "cure light wounds":
                    _damageSpells.CureLightWounds(player, target, room);
                    break;
                case "armour":
                case "armor":
                    _damageSpells.Armor(player, target, room, wearOff);
                    break;
                case "bless":
                    _damageSpells.Bless(player, target, room, wearOff);
                    break;
                case "identify":
                    _damageSpells.Identify(player, obj, room);
                    break;
            }
        }
    }
}
