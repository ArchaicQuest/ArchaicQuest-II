using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class AffectsCmd : ICommand
    {
        public AffectsCmd(ICore core)
        {
            Aliases = new[] {"affects", "aff"};
            Description = "Displays the affects upon the player.";
            Usages = new[] {"Type: affects"};
            DeniedStatus = null;
            UserRole = UserRole.Player;
            Core = core;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var sb = new StringBuilder();

            sb.Append("<p>You are affected by the following effects:</p><table class='simple'><thead><tr><td>Skill</td><td>Affect</td></tr></thead>");

            foreach (var affect in player.Affects.Custom)
            {
                sb.Append($"<tr> <td> {affect.Name}<div>{affect.Benefits}</div </td>");
                sb.Append("<td>");

                if (affect.Modifier.Armour != 0)
                {
                    sb.Append($"<p>modifies armour by {affect.Modifier.Armour}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.DamRoll != 0)
                {
                    sb.Append($"<p>modifies damage roll by {affect.Modifier.DamRoll}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.HitRoll != 0)
                {
                    sb.Append($"<p>modifies hit roll by {affect.Modifier.HitRoll}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Saves != 0)
                {
                    sb.Append($"<p>modifies saves by {affect.Modifier.Saves}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.HP != 0)
                {
                    sb.Append($"<p>modifies hit points by {affect.Modifier.HP}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Mana != 0)
                {
                    sb.Append($"<p>modifies mana by {affect.Modifier.Mana}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Moves != 0)
                {
                    sb.Append($"<p>modifies moves by {affect.Modifier.Moves}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.SpellDam != 0)
                {
                    sb.Append($"<p>modifies spell damage by {affect.Modifier.SpellDam}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Strength != 0)
                {
                    sb.Append($"<p>modifies strength by {affect.Modifier.Strength}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Dexterity != 0)
                {
                    sb.Append($"<p>modifies dexterity by {affect.Modifier.Dexterity}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Constitution != 0)
                {
                    sb.Append($"<p>modifies constitution by {affect.Modifier.Constitution}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Intelligence != 0)
                {
                    sb.Append($"<p>modifies intelligence by {affect.Modifier.Intelligence}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Wisdom != 0)
                {
                    sb.Append($"<p>modifies wisdom by {affect.Modifier.Wisdom}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Charisma != 0)
                {
                    sb.Append($"<p>modifies charisma by {affect.Modifier.Charisma}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }
                sb.Append("</td></tr>");
            }

            sb.Append("</tr></table>");

            Core.Writer.WriteLine(sb.ToString(), player.ConnectionId);
        }
    }
}