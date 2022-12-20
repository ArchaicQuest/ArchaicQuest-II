using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.Core
{
    public class Damage : IDamage
    {
        public KeyValuePair<string, string> DamageText(int damage)
        {
            switch (damage)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    return new KeyValuePair<string, string>("<span style='color:#2ecc71'>scratch</span>", "<span style='color:#2ecc71'>scratches</span>");
                case 5:
                case 6:
                case 7:
                case 8:
                    return new KeyValuePair<string, string>("<span style='color:#2ecc71'>graze</span>", "<span style='color:#2ecc71'>grazes</span>");
                case 9:
                case 10:
                case 11:
                case 12:
                    return new KeyValuePair<string, string>("<span style='color:#2ecc71'>hit</span>", "<span style='color:#2ecc71'>hits</span>");
                case 13:
                case 14:
                case 15:
                case 16:
                    return new KeyValuePair<string, string>("<span style='color:#2ecc71'>injure</span>", "<span style='color:#2ecc71'>injures</span>");
                case 17:
                case 18:
                case 19:
                case 20:
                    return new KeyValuePair<string, string>("<span style='color:yellow'>wound</span>", "<span style='color:yellow'>wounds</span>");
                case 21:
                case 22:
                case 23:
                case 24:
                    return new KeyValuePair<string, string>("<span style='color:yellow'>maul</span>", "<span style='color:yellow'>mauls</span>");
                case 25:
                case 26:
                case 27:
                case 28:
                    return new KeyValuePair<string, string>("<span style='color:yellow'>decimate</span>", "<span style='color:yellow'>decimates</span>");
                case 29:
                case 30:
                case 31:
                case 32:
                    return new KeyValuePair<string, string>("<span style='color:yellow'>devastate</span>", "<span style='color:yellow'>devastates</span>");
                case 33:
                case 34:
                case 35:
                case 36:
                    return new KeyValuePair<string, string>("<span style='color:yellow'>maim</span>", "<span style='color:yellow'>maims</span>");
                case 37:
                case 38:
                case 39:
                case 40:
                    return new KeyValuePair<string, string>("<span style='color:red'>MUTILATE</span>", "<span style='color:red'>MUTILATES</span>");
                case 41:
                case 42:
                case 43:
                case 44:
                    return new KeyValuePair<string, string>("<span style='color:red'>DISEMBOWEL</span>", "<span style='color:red'>DISEMBOWELS</span>");
                case 45:
                case 46:
                case 47:
                case 48:
                    return new KeyValuePair<string, string>("<span style='color:red'>MASSACRE</span>", "<span style='color:red'>MASSACRES</span>");
                case 49:
                case 50:
                case 51:
                case 52:
                    return new KeyValuePair<string, string>("*** <span style='color:red'>D</span>E<span style='color:red'>M</span>O<span style='color:red'>L</span>I<span style='color:red'>S</span>H ***", "*** <span style='color:red'>D</span>E<span style='color:red'>M</span>O<span style='color:red'>L</span>I<span style='color:red'>S</span>H ***");
                default:
                    return new KeyValuePair<string, string>("*** <span style='color:red'>A</span>N<span style='color:red'>N</span>I<span style='color:red'>H</span>I<span style='color:red'>L</span>A<span style='color:red'>T</span>E<span style='color:red'>S</span> ***", "*** <span style='color:red'>A</span>N<span style='color:red'>N</span>I<span style='color:red'>H</span>I<span style='color:red'>L</span>A<span style='color:red'>T</span>E<span style='color:red'>S</span> ***"); ;
            }

        }
    }
}
