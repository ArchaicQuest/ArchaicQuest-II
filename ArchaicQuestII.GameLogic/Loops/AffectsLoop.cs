using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Spell;

namespace ArchaicQuestII.GameLogic.Loops
{
    public class AffectsLoop : ILoop
    {
        public int TickDelay => 60000;

        public bool ConfigureAwait => true;

        private List<Player> _players = new List<Player>();

        public void PreTick()
        {
            _players = Services.Instance.Cache.GetPlayerCache().Values.ToList();
        }

        public void Tick()
        {
            //Console.WriteLine("AffectsLoop");

            foreach (var pc in _players)
            {
                pc.Hunger--;
                foreach (var aff in pc.Affects.Custom.ToList())
                {
                    aff.Duration--;

                    if (aff.Duration <= 0)
                    {
                        if (aff.Modifier.Strength != 0)
                        {
                            pc.Attributes.Attribute[EffectLocation.Strength] -=
                                aff.Modifier.Strength;
                        }

                        if (aff.Modifier.Dexterity != 0)
                        {
                            pc.Attributes.Attribute[EffectLocation.Dexterity] -=
                                aff.Modifier.Dexterity;
                        }

                        if (aff.Modifier.Constitution != 0)
                        {
                            pc.Attributes.Attribute[EffectLocation.Constitution] -=
                                aff.Modifier.Constitution;
                        }

                        if (aff.Modifier.Intelligence != 0)
                        {
                            pc.Attributes.Attribute[EffectLocation.Intelligence] -=
                                aff.Modifier.Intelligence;
                        }

                        if (aff.Modifier.Wisdom != 0)
                        {
                            pc.Attributes.Attribute[EffectLocation.Wisdom] -= aff.Modifier.Wisdom;
                        }

                        if (aff.Modifier.Charisma != 0)
                        {
                            pc.Attributes.Attribute[EffectLocation.Charisma] -=
                                aff.Modifier.Charisma;
                        }

                        if (aff.Modifier.HitRoll != 0)
                        {
                            pc.Attributes.Attribute[EffectLocation.HitRoll] -= aff.Modifier.HitRoll;
                        }

                        if (aff.Modifier.DamRoll != 0)
                        {
                            pc.Attributes.Attribute[EffectLocation.DamageRoll] -=
                                aff.Modifier.DamRoll;
                        }
                        if (aff.Modifier.Armour != 0)
                        {
                            pc.ArmorRating.Armour -= aff.Modifier.Armour;
                            pc.ArmorRating.Magic -= aff.Modifier.Armour;
                        }

                        pc.Affects.Custom.Remove(aff);

                        Services.Instance.SpellList.CastSpell(
                            aff.Name,
                            "",
                            pc,
                            "",
                            pc,
                            Services.Instance.Cache.GetRoom(pc.RoomId),
                            true
                        );

                        if (aff.Affects == DefineSpell.SpellAffect.Blind)
                        {
                            pc.Affects.Blind = false;
                            Services.Instance.Writer.WriteLine("You are no longer blinded.", pc);
                        }
                        if (aff.Affects == DefineSpell.SpellAffect.Berserk)
                        {
                            pc.Affects.Berserk = false;
                        }
                        if (aff.Affects == DefineSpell.SpellAffect.NonDetect)
                        {
                            pc.Affects.NonDectect = false;
                        }
                        if (aff.Affects == DefineSpell.SpellAffect.Invis)
                        {
                            pc.Affects.Invis = false;
                        }
                        if (aff.Affects == DefineSpell.SpellAffect.DetectInvis)
                        {
                            pc.Affects.DetectInvis = false;
                        }
                        if (aff.Affects == DefineSpell.SpellAffect.DetectHidden)
                        {
                            pc.Affects.DetectHidden = false;
                        }
                        if (aff.Affects == DefineSpell.SpellAffect.Poison)
                        {
                            pc.Affects.Poisoned = false;
                        }
                        if (aff.Affects == DefineSpell.SpellAffect.Haste)
                        {
                            pc.Affects.Haste = false;
                        }
                    }
                    Services.Instance.UpdateClient.UpdateAffects(pc);
                }

                var idleTime5Mins = pc.LastCommandTime.AddMinutes(6) <= DateTime.Now;

                if (!pc.Idle && idleTime5Mins)
                {
                    Services.Instance.Writer.WriteLine("You enter the void.", pc);
                    pc.Idle = true;
                    return;
                }

                var idleTime10Mins = pc.LastCommandTime.AddMinutes(11) <= DateTime.Now;
                var idleTime15Mins = pc.LastCommandTime.AddMinutes(16) <= DateTime.Now;
                if (idleTime10Mins && !idleTime15Mins)
                {
                    Services.Instance.Writer.WriteLine("You go deeper into the void.", pc);
                }

                if (idleTime15Mins)
                {
                    pc.Buffer.Enqueue("quit");
                }
            }
        }

        public void PostTick()
        {
            _players.Clear();
        }
    }
}
