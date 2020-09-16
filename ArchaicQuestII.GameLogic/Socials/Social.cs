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
                _writeToClient.WriteLine($"<p>{social.CharNoTarget}</p>", player.ConnectionId);
              
                foreach (var pc in room.Players)
                {
                    if (pc.Id == player.Id)
                    {
                        continue;
                    }
                    _writeToClient.WriteLine($"<p>{social.RoomNoTarget.Replace("#player#", player.Name).Replace("#pgender#", Helpers.GetPronoun(player.Gender)).Replace("#pgender2#", Helpers.GetSubjectPronoun(player.Gender)).Replace("#pgender3#", Helpers.GetObjectPronoun(player.Gender))}</p>", pc.ConnectionId);
                }

                return;
            }

            var getTarget = (room.Players.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase)));
            if (getTarget != null)
            {
                if (getTarget.Id == player.Id)
                {
                    foreach (var pc in room.Players)
                    {
                        if (pc.Id == player.Id)
                        {
                            _writeToClient.WriteLine($"<p>{social.TargetSelf.Replace("#player#", getTarget.Name).Replace("#player#", player.Name).Replace("#pgender#", Helpers.GetPronoun(player.Gender)).Replace("#pgender#", Helpers.GetPronoun(player.Gender)).Replace("#pgender3#", Helpers.GetSubjectPronoun(player.Gender))}</p>", pc.ConnectionId);
                        }
                        _writeToClient.WriteLine($"<p>{social.RoomSelf.Replace("#player#", getTarget.Name).Replace("#player#", player.Name).Replace("#pgender#", Helpers.GetPronoun(player.Gender)).Replace("#pgender#", Helpers.GetPronoun(player.Gender)).Replace("#pgender3#", Helpers.GetSubjectPronoun(player.Gender))}</p>", pc.ConnectionId);
                    }
                    return;
                }
                _writeToClient.WriteLine($"<p>{social.TargetFound.Replace("#target#", getTarget.Name).Replace("#tgender#", Helpers.GetPronoun(getTarget.Gender))}<p>", player.ConnectionId);
                _writeToClient.WriteLine($"<p>{social.ToTarget.Replace("#player#", player.Name).Replace("#pgender#", Helpers.GetPronoun(player.Gender)).Replace("#pgender2#", Helpers.GetObjectPronoun(player.Gender))}</p>", getTarget.ConnectionId);
                foreach (var pc in room.Players)
                {
                    if (pc.Id == player.Id || pc.Id == getTarget.Id)
                    {
                        continue;
                    }
                    _writeToClient.WriteLine($"<p>{social.RoomTarget.Replace("#target#", getTarget.Name).Replace("#player#", player.Name).Replace("#pgender#", Helpers.GetPronoun(player.Gender)).Replace("#pgender#", Helpers.GetPronoun(player.Gender)).Replace("#pgender3#", Helpers.GetSubjectPronoun(player.Gender))}</p>", pc.ConnectionId);
                }
            }
            else
            {
                _writeToClient.WriteLine("<p>They are not here.</p>", player.ConnectionId);
            }
        }
    }
}
