using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Character.Help
{
    public class HelpFile : IHelp
    {
        private readonly IWriteToClient _writer;
        private readonly ICache _cache;
        public HelpFile(IWriteToClient writer, ICache cache)
        {
            _writer = writer;
            _cache = cache;
        }
        public List<Help> FindHelpFile(string keyword)
        {
            var helpFile = _cache.FindHelp(keyword);

            return helpFile;
        }

        public string DisplayHelpOptions(List<Help> helpList, string keyword)
        {
            var sb = new StringBuilder();
            sb.Append($"<p>Multiple help files found containing the keyword: {keyword}</p>");
            sb.Append("<table class='help-list'>");
            foreach (var help in helpList)
            {
                sb.Append($"<tr><td>Help {help.Title}</td><td>-</td><td>{help.BriefDescription}</td></tr>");
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        public void SendHelpFileToUser(Help help, Player player)
        {
            var sb = new StringBuilder();


            sb.Append("<div class='help-section'><table><tr>");
            sb.Append($"<td>Help Title</td><td>{help.Title}</td></tr>");

            sb.Append($"<td>Help Keywords</td><td>{help.Keywords}</td></tr>");

            if (!string.IsNullOrEmpty(help.RelatedHelpFiles))
            {
                sb.Append($"<tr><td>Related Helps</td><td>{help.RelatedHelpFiles}</td></tr>");
            }

            sb.Append($"<tr><td>Last Updated</td><td>{help.DateUpdated.Value:MMMM dd, yyyy}</td></tr></table>");

            sb.Append($"<pre>{help.Description}</pre>");

            _writer.WriteLine(sb.ToString(), player.ConnectionId);
        }

        public void DisplayHelpFile(string keyword, Player player)
        {
            var helpFile = FindHelpFile(keyword);

            if (helpFile != null)
            {

                if (helpFile.Count > 1)
                {
                    var searchByTitle = helpFile.FirstOrDefault(x =>
                        x.Title.Equals(keyword, StringComparison.CurrentCultureIgnoreCase));

                    if (searchByTitle == null)
                    {

                        _writer.WriteLine(DisplayHelpOptions(helpFile, keyword), player.ConnectionId);
                        return;
                    }

                    SendHelpFileToUser(searchByTitle, player);
                    return;

                }

                if (helpFile.Count == 1)
                {
                    SendHelpFileToUser(helpFile[0], player);
                }
                else
                {
                    _writer.WriteLine("No help found for that keyword", player.ConnectionId);
                }
            }


        }
    }
}
