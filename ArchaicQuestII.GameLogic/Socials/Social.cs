using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Emote;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Socials
{
    public class Social: ISocials
    {
 
        private readonly IWriteToClient _writeToClient;
        public Social(IWriteToClient writeToClient)
        {
            _writeToClient = writeToClient;
        }
        public void EmoteSocial(Player player, Room room, Emote social, string target)
        {

            if (string.IsNullOrEmpty(target))
            {
                _writeToClient.WriteLine($"<p>{social.ToSender}</p>", player.ConnectionId);
              
                foreach (var pc in room.Players)
                {
                    if (pc.Id == player.Id)
                    {
                        continue;
                    }
                    _writeToClient.WriteLine($"<p>{social.ToRoom.Replace("#", player.Name)}</p>", pc.ConnectionId);
                }

                return;
            }

            var getTarget = (room.Players.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase)));
            if (getTarget != null)
            {
                _writeToClient.WriteLine($"<p>{social.ToSenderAtTarget.Replace("@", getTarget.Name)}<p>", player.ConnectionId);
                _writeToClient.WriteLine($"<p>{social.ToTarget.Replace("#", player.Name)}</p>", getTarget.ConnectionId);
                foreach (var pc in room.Players)
                {
                    if (pc.Id == player.Id || pc.Id == getTarget.Id)
                    {
                        continue;
                    }
                    _writeToClient.WriteLine($"<p>{social.ToRoomTarget.Replace("@", getTarget.Name).Replace("#", player.Name)}</p>", pc.ConnectionId);
                }
            }
            else
            {
                _writeToClient.WriteLine("<p>They are not here.</p>", player.ConnectionId);
            }
        }
    }
}
