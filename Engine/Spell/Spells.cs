using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Item;
using System.Data;
using ArchaicQuestII.Engine.Effect;
using ArchaicQuestII.Engine.Skills;

namespace ArchaicQuestII.Engine.Spell
{
    public class Spells
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dice Damage { get; set; }
        public Effect.Effect Effect { get; set; }
        public Requirements Requirements { get; set; }
        public Sphere SpellGroup { get; set; }
        public Messages SkillStart { get; set; }
        public Messages[] SkillAction { get; set; }
        public Messages SkillEnd { get; set; }
        public Messages SkillFailure { get; set; }
        public SpellType Type { get; set; }
        public int Rounds { get; set; }

        public void DoSpell(string spellName, Player origin, Player target)
        {
            // you're asleep
            // your deead
            // all those checks to go here
            // mana check
            // target check (shrugs)

            //find spell here, or pass to func
            Spells Spell = new Spells();

            var formula = Spell.Damage.Roll(Spell.Damage.DiceRoll, Spell.Damage.DiceMinSize,
                              Spell.Damage.DiceMaxSize) + (origin.Level + 1) / 2;

            //Fire skill start message to player, room, target

            //deduct mana

            if (Spell.Rounds != 1)
            {
                // prob needs to be on player
                Spell.Rounds -= 1;
                return;
            }
      
            if (Spell.Type.Affect)
            {
                //Fire skill Action message to player, room, target


                if (Spell.Effect.Location == EffectLocation.Strength)
                {
                    if (Spell.Effect.Modifier.PositiveEffect)
                    {
                        target.Attributes.Strength += formula;                   
                    }
                    else
                    {
                        target.Attributes.Strength -= formula;
                    }

                    //Add effect + duration 
                }
                else if (Spell.Effect.Location == EffectLocation.Dexterity)
                {
                    if (Spell.Effect.Modifier.PositiveEffect)
                    {
                        target.Attributes.Dexterity += formula;
                    }
                    else
                    {
                        target.Attributes.Dexterity -= formula;
                    }
                }
                else if (Spell.Effect.Location == EffectLocation.Constitution)
                {
                    if (Spell.Effect.Modifier.PositiveEffect)
                    {
                        target.Attributes.Constitution += formula;
                    }
                    else
                    {
                        target.Attributes.Constitution -= formula;
                    }
                }
                else if (Spell.Effect.Location == EffectLocation.Hitpoints)
                {
                    if (Spell.Effect.Modifier.PositiveEffect)
                    {
                        target.Stats.HitPoints += formula;
                    }
                    else
                    {
                       
                        target.Stats.HitPoints -= formula;
                    }
                }

                
               
            }
 

        }
        public int ParseExpression(string exp, Player origin, Player target)
        {

            if (exp.Contains("{origin.level}"))
            {
                exp.Replace("{origin.level}", origin.Level.ToString());
            }

            DataTable dt = new DataTable();
            var v = dt.Compute(exp, "");

            return Int32.Parse(v.ToString());
        }
    }

 
}

//Description
//Damage(die rolls)
//Affects(slow, haste etc affects created in admin)
//Type - Alteration, Illusion, etc the d&d schools basically
//Duration
//Delay - (how many ticks, 1 tick 120ms?)
//Messages
//Setup(delay)
//- your hands begin to glow
//- y hands begin to glow
//Action 
//- your burning hands blasts x
//- y burning hands blasts x
//- y burning hands blasts you
//Help
