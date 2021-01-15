using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Character.Help
{
   public class HelpFile: IHelp
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
          var helpFile =  _cache.FindHelp(keyword);

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

        public void DisplayHelpFile(string keyword, Player player)
        {
            var helpFile = FindHelpFile(keyword);

            if (helpFile != null)
            {

                if (helpFile.Count > 1)
                {
                    _writer.WriteLine(DisplayHelpOptions(helpFile, keyword), player.ConnectionId);
                    return;
                }

                var sb = new StringBuilder();


                    sb.Append("<div class='help-section'><table><tr>");
                    sb.Append($"<td>Help Title</td><td>{helpFile[0].Title}</td></tr>");

                sb.Append($"<td>Help Keywords</td><td>{helpFile[0].Keywords}</td></tr>");

                if (!string.IsNullOrEmpty(helpFile[0].RelatedHelpFiles))
                {    
                    sb.Append($"<tr><td>Related Helps</td><td>{helpFile[0].RelatedHelpFiles}</td></tr>");
                }

                sb.Append($"<tr><td>Last Updated</td><td>{helpFile[0].DateUpdated.Value:MMMM dd, yyyy}</td></tr></table>");

                sb.Append($"<pre>{helpFile[0].Description}</pre>");

                _writer.WriteLine(sb.ToString(), player.ConnectionId);
            }
 
        }
    }
}
