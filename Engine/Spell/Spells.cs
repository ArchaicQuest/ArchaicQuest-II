using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Item;
using System.Data;
using ArchaicQuestII.Engine.Effect;

namespace ArchaicQuestII.Engine.Spell
{
    public class Spells
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dice Damage { get; set; }
        public Effect.Effect Effect { get; set; }


        public void DoSpell(string spellName, Player origin, Player target)
        {
            //find spell here, or pass to func
            Spells Spell = new Spells();
            //check class
            var isCaster = false;
            // does the spell have a positive or negative affect
            var affectPositive = false;
            var die = new Dice();
            var modifier = 0;
            var duration = 0;


            if (isCaster)
            {
                //die roll simulating 2d20 roll
                modifier = die.Roll(2, 1, 20) + origin.Level / 2;
                duration = die.Roll(1, 1, 6) + origin.Level / 2;
            }
            else
            {
                modifier = die.Roll(2, 1, 20) + origin.Level / 4;
                //die roll simulating 1d6 roll
                duration = die.Roll(1, 1, 6) +  origin.Level / 4;
            }

           
            // Here we find what the spell affects
            // -----------------------------
            // list of locations:
            // None = 0,
            // Strength = 1 << 0,
            // Dexterity = 1 << 1,  
            // Constitution = 1 << 2,  
            // Intelligence = 1 << 3, 
            // Wisdom = 1 << 4,  
            // Charisma = 1 << 5,
            // Luck = 1 << 6,  
            // Hitpoints = 1 << 7,  
            // Mana = 1 << 8,  
            // Moves = 1 << 9,
            // Armour = 1 << 10,  
            // HitRoll = 1 << 11,  
            // SavingSpell = 1 << 12,  
            // DamageRoll = 1 << 13,   
            // Gender = 1 << 14,  
            // Age = 1 << 15,
            // Weight = 1 << 16,  
            // Height = 1 << 16, 
            // Level = 1 << 16,  
        
 

            if (Spell.Effect.Location == EffectLocation.Strength)
            {
                if (affectPositive)
                {
                    target.Attributes.Strength += modifier;
                }
                else
                {
                    target.Attributes.Strength -= modifier;
                }
                
            }

            if (Spell.Effect.Location == EffectLocation.HitRoll)
            {
                if (affectPositive)
                {
                    target.Stats.HitPoints += modifier;
                }
                else
                {
                    target.Stats.HitPoints -= modifier;
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
