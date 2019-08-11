using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Item;
using System.Data;
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
            //find spell here
            Spells Spell = new Spells();
            var isCaster = false;

            var expression = "{origin.level} / 4";

            target.Level =  expression

            //if (Spell.Effect.Modifier.LevelCheck && isCaster)
            //{
            //    Spell.Effect.Duration = Spell.Effect.Duration + (origin.Level / 4);
            //}

            //if (Spell.Effect.Modifier.LevelCheck && !isCaster)
            //{
            //    Spell.Effect.Duration = Spell.Effect.Duration + (origin.Level / 2);
            //}

            //if (Spell.Effect.Modifier.LevelCheck && isCaster)
            //{
            //    Spell.Effect.Modifier.Modifier = origin.Level / 4;
            //}

            //if (Spell.Effect.Modifier.LevelCheck && !isCaster)
            //{
            //    Spell.Effect.Modifier.Modifier = origin.Level / 2;
            //}




        }
        public int ParseExpression(string exp, Player origin, Player target)
        {

            if (exp.Contains("{origin.level}"))
            {
                exp.Replace("{origin.level}", origin.Level.ToString());
            }

            DataTable dt = new DataTable();
            var v = dt.Compute("3 * (2+4)", "");

            return exp.
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
