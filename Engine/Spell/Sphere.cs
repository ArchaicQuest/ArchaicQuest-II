using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Engine.Spell
{
    public class Sphere
    {
        /// <summary>
        /// Notes taking from here https://en.wikipedia.org/wiki/Magic_of_Dungeons_%26_Dragons
        /// </summary>
        public enum SpellGroup
        {
            [Description("This school is focused on protective spells, as well as spells which cancel or interfere with other spells, magical effects or supernatural abilities, such as Break Enchantment, Dimensional Anchor, Dispel Magic or Remove Curse. Wizards who specialize in this school are known as Abjurers.")]
            Abjuration,
            [Description("This school is focused on instantaneous transportation, conjuring manifestations of creatures, energy or objects, and object creation. Mages who specialize in this school are known as Conjurers.")]
            Conjuration,
            [Description("This school is focused on acquiring information, spells such as Detect Magic, Identify and Read Magic, Wizards who specialize in this school are known as Diviners.")]
            Divination,
            [Description("Holy spells for Cleric, druid types")]
            Divine,
            [Description("Enchanment, buffs and charm spells, Wizards who specialize in this school are known as Enchanters ")]
            Enchantment,
            [Description("Invocation is focused on damaging energy-based spells such as Fireball, Lightning Bolt, and Cone of Cold. It also includes conjurations of magical energy, such as Wall of Force, Darkness, Light Wizards who specialize in this school are known as Invokers.")]
            Invocation,
            [Description("This school is divided into five subschools: figment, glamer, pattern, phantasm, and shadow. Figment spells create artificial sensations with no physical substance.  Glamer spells alter the target's sensory properties, and can cause invisibility. Pattern spells create insubstantial images which affect the minds of the viewers, and can inflict harm. Phantasm spells create hallucinations which can be harmful. Shadow spells use magical shadows to create things with physical substance. Wizards who specialize in this school are known as Illusionists.")]
            Illusion,
            [Description("Necromancy spells involve death, undeath, and the manipulation of life energy. Necromancy can usually be divided into three or four categories: spells that help or create the Undead, like Animate Dead; spells that hurt the Undead, like Disrupt Undead; spells that hurt other people, like Enervation or Vampiric Touch; and spells that manipulate life in order to heal Wizards who specialize in this school are known as Necromancers.")]
            Necromancy,
            [Description(" Spells in this school alter the properties of their targets. Examples include Bull's Strength, Fabricate, Polymorph, Plant Growth, Move Earth, and Water Breathing Wizards who specialize in this school are known as Transmuters.")]
            Transmutation,
            [Description("Universal spells have effects too broad to place into one class, or too useful for any specialist to consider forsaking. They often can perform multiple effects, or perform a very specific effect that does not fit into another category.")]
            Universal,
            None
        };

    }
}
