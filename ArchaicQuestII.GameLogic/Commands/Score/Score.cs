using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Score
{
    public class Score: IScore
    {

        private readonly IWriteToClient _writer;
       
        public Score(IWriteToClient writer)
        {
            _writer = writer;
        }

        public float CalculateWeight(Player player)
        {

            float weight = 0;
            foreach (var item in player.Inventory)
            {
                weight += item.Weight == 0 ? 1 : item.Weight;
            }

            player.Weight = weight;

            return weight;
        }

        public void DisplayScore(Player player)
        {
           var sb = new StringBuilder();

           sb.Append($"<table class=\"score-table\"><tr><td class=\"cell-title\">Level:</td><td>{player.Level}</td><td class=\"cell-title\">Race:</td><td>{player.Race}</td><td class=\"cell-title\">Born on:</td><td>{player.DateCreated}</td></tr>");
           sb.Append($"<tr><td class=\"cell-title\">Years:</td><td>n/a</td><td class=\"cell-title\">Class:</td><td>{player.ClassName}</td><td class=\"cell-title\">Played:</td><td>{player.PlayTime} hours</td></tr>");
           sb.Append($"<tr><td class=\"cell-title\">STR:</td><td>{player.Attributes.Attribute[EffectLocation.Strength]}({player.MaxAttributes.Attribute[EffectLocation.Strength]})</td><td class=\"cell-title\"></td> <!-- hit roll --><td></td><td class=\"cell-title\">Sex:</td><td>{player.Gender}</td></tr>");
           sb.Append($"<tr><td class=\"cell-title\">DEX:</td><td>{player.Attributes.Attribute[EffectLocation.Dexterity]}({player.MaxAttributes.Attribute[EffectLocation.Dexterity]})<td class=\"cell-title\"></td><!-- dam roll --><td></td><td class=\"cell-title\">Wimpy:</td><td>XX</td></tr>");
           sb.Append($"<tr><td class=\"cell-title\">Con:</td><td>{player.Attributes.Attribute[EffectLocation.Constitution]}({player.MaxAttributes.Attribute[EffectLocation.Constitution]})<td class=\"cell-title\">Align:</td><td>{player.AlignmentScore}</td><td></td><td></td></tr>");
           sb.Append($"<tr><td class=\"cell-title\">INT:</td><td>{player.Attributes.Attribute[EffectLocation.Intelligence]}({player.MaxAttributes.Attribute[EffectLocation.Intelligence]})<td class=\"cell-title\">Armour:</td><td>xxx</td><td></td><td></td></tr>");
           sb.Append($"<tr><td class=\"cell-title\">WIS:</td><td>{player.Attributes.Attribute[EffectLocation.Wisdom]}({player.MaxAttributes.Attribute[EffectLocation.Wisdom]})<td class=\"cell-title\">Pos'n:</td><td>Standing</td><td></td><td></td></tr>");
           sb.Append($"<tr><td class=\"cell-title\">CHA:</td><td>{player.Attributes.Attribute[EffectLocation.Charisma]}({player.MaxAttributes.Attribute[EffectLocation.Charisma]})<td class=\"cell-title\">Style:</td><td>Standard</td><td></td><td></td></tr>");
           sb.Append("<tr><td></td><td></td><td></td><td></td><td></td><td></td></tr><tr><td></td><td></td><td class=\"cell-title\"></td><td></td><td></td><td></td></tr>");
           sb.Append($"<tr><td class=\"cell-title\">Qpoints:</td><td>0</td><td class=\"cell-title\">HP</td><td> {player.Attributes.Attribute[EffectLocation.Hitpoints]}/{player.MaxAttributes.Attribute[EffectLocation.Hitpoints]}</td><td class=\"cell-title\">weight:</td><td>{CalculateWeight(player)} lb. (max:{player.Attributes.Attribute[EffectLocation.Strength] * 3} lb.)</td></tr>");
           sb.Append($"<tr><td class=\"cell-title\">Pract:</td><td>0</td><td class=\"cell-title\">Mana</td><td> {player.Attributes.Attribute[EffectLocation.Mana]}/{player.MaxAttributes.Attribute[EffectLocation.Mana]}</td><td class=\"cell-title\">Mkills:</td><td>0</td></tr>");
           sb.Append($"<tr><td class=\"cell-title\">Train:</td><td>0</td><td class=\"cell-title\">Moves</td><td> {player.Attributes.Attribute[EffectLocation.Moves]}/{player.MaxAttributes.Attribute[EffectLocation.Moves]}</td><td class=\"cell-title\">MDeaths:</td><td>0</td></tr>");
           sb.Append($"<tr><td class=\"cell-title\">Gold:</td><td>{player.Money.Gold}</td><td class=\"cell-title\">XP</td><td>{player.Experience}</td><td class=\"cell-title\"></td><td></td></tr>");
           sb.Append($"<tr><td class=\"cell-title\">Bank:</td><td>0</td><td class=\"cell-title\">TNL</td><td>{player.ExperienceToNextLevel}</td><td class=\"cell-title\"></td><td></td></tr></table>");

           _writer.WriteLine(sb.ToString(), player.ConnectionId);
        }
    }
}
