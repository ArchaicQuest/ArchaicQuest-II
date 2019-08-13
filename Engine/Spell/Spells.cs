using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Item;
using System.Data;
using ArchaicQuestII.Engine.Effect;
using ArchaicQuestII.Engine.Skills;
using ArchaicQuestII.Engine.World.Room.Model;

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
        public LevelBasedMessages LevelBasedMessages { get; set; }
        public SpellType Type { get; set; }
        public int Rounds { get; set; }
        public Cost Cost { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Spell"></param>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="room"></param>
        public void DoSpell(Spells Spell, Player origin, Player target, Room room)
        {
            // you're asleep
            // your deead
            // all those checks to go here
            // mana check
            // target check (shrugs)

     

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
            
                //can be used for spells like magic missile that fires multiple bolts at higher levels
                if (Spell.LevelBasedMessages.HasLevelBasedMessages)
                {
                    switch (origin.Level)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                            //send level spec message
                            //Spell.LevelBasedMessages.Ten.ToPlayer;
                            //Spell.LevelBasedMessages.Ten.ToRoom
                            //Spell.LevelBasedMessages.Ten.ToTarget
                            break;
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                        case 20:
                            //send level spec message
                            //Spell.LevelBasedMessages.Ten.ToPlayer;
                            //Spell.LevelBasedMessages.Ten.ToRoom
                            //Spell.LevelBasedMessages.Ten.ToTarget
                            break;
                        case 21:
                        case 22:
                        case 23:
                        case 24:
                        case 25:
                        case 26:
                        case 27:
                        case 28:
                        case 29:
                        case 30:
                            //send level spec message
                            //Spell.LevelBasedMessages.Ten.ToPlayer;
                            //Spell.LevelBasedMessages.Ten.ToRoom
                            //Spell.LevelBasedMessages.Ten.ToTarget
                            break;
                        case 31:
                        case 32:
                        case 33:
                        case 34:
                        case 35:
                        case 36:
                        case 37:
                        case 38:
                        case 39:
                        case 40:
                            //send level spec message
                            //Spell.LevelBasedMessages.Ten.ToPlayer;
                            //Spell.LevelBasedMessages.Ten.ToRoom
                            //Spell.LevelBasedMessages.Ten.ToTarget
                            break;
                        case 41:
                        case 42:
                        case 43:
                        case 44:
                        case 45:
                        case 46:
                        case 47:
                        case 48:
                        case 49:
                        case 50:
                        case 51:
                            //send level spec message
                            //Spell.LevelBasedMessages.Ten.ToPlayer;
                            //Spell.LevelBasedMessages.Ten.ToRoom
                            //Spell.LevelBasedMessages.Ten.ToTarget
                            break;
                        default:

                            //Fire defualt skill Action message to player, room, target
                            break;
                    }
                }
                else
                {
                    //Fire skill Action message to player, room, target
                }


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

    }

 
}

