using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Utilities;

namespace ArchaicQuestII.GameLogic.Character;

public static class CharacterEvents
{
    public static void LoopPlayers(ICoreHandler coreHandler)
    {
        var players = coreHandler.Character.GetPlayerCache().Values.ToList();
        
        foreach (var player in players)
        {

            //  IdleCheck(player);

            var hP = DiceBag.Roll(1, 2, 5);
            var mana = DiceBag.Roll(1, 2, 5);
            var moves = DiceBag.Roll(1, 2, 5);

            // if player has fast healing add the bonus here
            var hasFastHealing = player.Skills.FirstOrDefault(x =>
                x.SkillName.Equals("Fast Healing", StringComparison.CurrentCultureIgnoreCase) &&
                player.Level >= x.Level);



            if ((player.Status & CharacterStatus.Status.Sleeping) != 0)
            {
                hP *= 2;
                mana *= 2;
                moves *= 2;
            }

            if ((player.Status & CharacterStatus.Status.Resting) != 0)
            {
                hP *= (int)1.5;
                mana *= (int)1.5;
                moves *= (int)1.5;
            }

            if (player.Attributes.Attribute[EffectLocation.Hitpoints] <
                player.MaxAttributes.Attribute[EffectLocation.Hitpoints])
            {

                if (hasFastHealing != null)
                {
                    if (Helpers.SkillSuccessCheck(hasFastHealing))
                    {
                        hP *= 2;
                    }
                    else
                    {
                        coreHandler.Character.GainSkillProficiency(hasFastHealing, player);
                    }
                }

                player.Attributes.Attribute[EffectLocation.Hitpoints] += Formulas.GainAmount(hP, player);
                if (player.Attributes.Attribute[EffectLocation.Hitpoints] >
                    player.MaxAttributes.Attribute[EffectLocation.Hitpoints])
                {
                    player.Attributes.Attribute[EffectLocation.Hitpoints] =
                        player.MaxAttributes.Attribute[EffectLocation.Hitpoints];
                }
            }

            if (player.Attributes.Attribute[EffectLocation.Mana] <
                player.MaxAttributes.Attribute[EffectLocation.Mana])
            {
                player.Attributes.Attribute[EffectLocation.Mana] += Formulas.GainAmount(mana, player);

                if (player.Attributes.Attribute[EffectLocation.Mana] >
                    player.MaxAttributes.Attribute[EffectLocation.Mana])
                {
                    player.Attributes.Attribute[EffectLocation.Mana] =
                        player.MaxAttributes.Attribute[EffectLocation.Mana];
                }
            }

            if (player.Attributes.Attribute[EffectLocation.Moves] <
                player.MaxAttributes.Attribute[EffectLocation.Moves])
            {
                player.Attributes.Attribute[EffectLocation.Moves] += Formulas.GainAmount(moves, player);
                if (player.Attributes.Attribute[EffectLocation.Moves] >
                    player.MaxAttributes.Attribute[EffectLocation.Moves])
                {
                    player.Attributes.Attribute[EffectLocation.Moves] =
                        player.MaxAttributes.Attribute[EffectLocation.Moves];
                }
            }

            coreHandler.Client.UpdateHP(player);
            coreHandler.Client.UpdateMana(player);
            coreHandler.Client.UpdateMoves(player);
            coreHandler.Client.UpdateScore(player);

        }
    }

    public static void UpdatePlayers(ICoreHandler coreHandler)
    {
        var players = coreHandler.Character.GetPlayerCache().Values;

        foreach (var pc in players)
        {
            pc.Hunger--;
            foreach (var aff in pc.Affects.Custom.ToList())
            {
                aff.Duration--;

                if (aff.Duration <= 0)
                {
                    if (aff.Modifier.Strength != 0)
                    {
                        pc.Attributes.Attribute[EffectLocation.Strength] -= aff.Modifier.Strength;
                    }

                    if (aff.Modifier.Dexterity != 0)
                    {
                        pc.Attributes.Attribute[EffectLocation.Dexterity] -= aff.Modifier.Dexterity;
                    }

                    if (aff.Modifier.Constitution != 0)
                    {
                        pc.Attributes.Attribute[EffectLocation.Constitution] -= aff.Modifier.Constitution;
                    }

                    if (aff.Modifier.Intelligence != 0)
                    {
                        pc.Attributes.Attribute[EffectLocation.Intelligence] -= aff.Modifier.Intelligence;
                    }

                    if (aff.Modifier.Wisdom != 0)
                    {
                        pc.Attributes.Attribute[EffectLocation.Wisdom] -= aff.Modifier.Wisdom;
                    }

                    if (aff.Modifier.Charisma != 0)
                    {
                        pc.Attributes.Attribute[EffectLocation.Charisma] -= aff.Modifier.Charisma;
                    }

                    if (aff.Modifier.HitRoll != 0)
                    {
                        pc.Attributes.Attribute[EffectLocation.HitRoll] -= aff.Modifier.HitRoll;
                    }

                    if (aff.Modifier.DamRoll != 0)
                    {
                        pc.Attributes.Attribute[EffectLocation.DamageRoll] -= aff.Modifier.DamRoll;
                    }

                    if (aff.Modifier.Armour != 0)
                    {
                        pc.ArmorRating.Armour -= aff.Modifier.Armour;
                        pc.ArmorRating.Magic -= aff.Modifier.Armour;
                    }


                    pc.Affects.Custom.Remove(aff);

                    coreHandler.Character.CastSpell(aff.Name, "", pc, "", pc, coreHandler.World.GetRoom(pc.RoomId), true);

                    if (aff.Affects == DefineSpell.SpellAffect.Blind)
                    {
                        pc.Affects.Blind = false;
                        coreHandler.Client.WriteLine("You are no longer blinded.", pc.ConnectionId);
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

                    if (aff.Affects == DefineSpell.SpellAffect.Haste
                       )
                    {
                        pc.Affects.Haste = false;
                    }
                }

                coreHandler.Client.UpdateAffects(pc);
            }

            var idleTime5Mins = pc.LastCommandTime.AddMinutes(6) <= DateTime.Now;

            if (!pc.Idle && idleTime5Mins)
            {
                coreHandler.Client.WriteLine("You enter the void.", pc.ConnectionId);
                pc.Idle = true;
                return;
            }

            var idleTime10Mins = pc.LastCommandTime.AddMinutes(11) <= DateTime.Now;
            var idleTime15Mins = pc.LastCommandTime.AddMinutes(16) <= DateTime.Now;
            
            if (idleTime10Mins && !idleTime15Mins)
            {
                coreHandler.Client.WriteLine("You go deeper into the void.", pc.ConnectionId);
            }

            if (idleTime15Mins)
            {
                pc.Buffer.Enqueue("quit");
            }
        }
    }
}