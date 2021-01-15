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
        public Help FindHelpFile(string keyword)
        {
          var helpFile =  _cache.FindHelp(keyword);

          return helpFile;
        }

        public void DisplayHelpFile(string keyword, Player player)
        {
            var helpFile = FindHelpFile(keyword);

            if (helpFile != null)
            {
                _writer.WriteLine(helpFile.Description, player.ConnectionId);
            }
 
        }
    }
}
