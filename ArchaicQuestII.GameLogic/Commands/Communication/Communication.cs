using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication
{
    public class Communication: Icommunication
    {

        private readonly IWriteToClient _writer;
        private readonly ICache _cache;
        private readonly IUpdateClientUI _updateClient;
        public Communication(IWriteToClient writer, ICache cache, IUpdateClientUI updateClient)
        {
            _writer = writer;
            _cache = cache;
            _updateClient = updateClient;
        }

        public void Gossip(string text, Room room, Player player)
        {
            _writer.WriteLine($"<p class='gossip'>[<span>Gossip</span>]: {text}</p>", player.ConnectionId);
            _updateClient.UpdateCommunication(player, $"<p class='gossip'>[<span>Gossip</span>]: {text}</p>", "gossip");
            foreach (var pc in room.Players)
            {
                if (pc.Name.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
                _writer.WriteLine($"<p class='gossip'>[<span>Gossip</span>] {player.Name}: {text}</p>", pc.ConnectionId);
                _updateClient.UpdateCommunication(pc, $"<p class='gossip'>[<span>Gossip</span>] {player.Name}: {text}</p>", "gossip");
            }
        }

        public void Newbie(string text, Room room, Player player)
        {

            _writer.WriteLine($"<p class='newbie'>[<span>Newbie</span>]: {text}</p>", player.ConnectionId);
            _updateClient.UpdateCommunication(player, $"<p class='newbie'>[<span>Newbie</span>]: {text}</p>", "newbie");
            foreach (var pc in room.Players)
            {
                if (pc.Name.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
                _writer.WriteLine($"<p class='newbie'>[<span>Newbie</span>] {player.Name}: {text}</p>", pc.ConnectionId);
                _updateClient.UpdateCommunication(pc, $"<p class='newbie'>[<span>Newbie</span>] {player.Name}: {text}</p>", "newbie");
            }
             
        }

        public void OOC(string text, Room room, Player player)
        {
            _writer.WriteLine($"<p class='ooc'>[<span>OOC</span>]: {text}</p>", player.ConnectionId);
            _updateClient.UpdateCommunication(player, $"<p class='ooc'>[<span>OOC</span>]: {text}</p>", "ooc");
            foreach (var pc in room.Players)
            {
                if (pc.Name.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
                _writer.WriteLine($"<p class='ooc'>[<span>OOC</span>] {player.Name}: {text}</p>", pc.ConnectionId);
                _updateClient.UpdateCommunication(pc, $"<p class='ooc'>[<span>OOC</span>] {player.Name}: {text}</p>", "ooc");
            }

        }

        // TODO: newbie, OOC, Gossip,

        public void Say(string text, Room room, Player player)
        {
            _writer.WriteLine($"<p class='say'>You say {text}</p>", player.ConnectionId);
            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                _writer.WriteLine($"<p class='say'>{player.Name} says {text}</p>", pc.ConnectionId);
                _updateClient.UpdateCommunication(pc, $"<p class='say'>{player.Name} says {text}</p>", "room");
            }
            _updateClient.UpdateCommunication(player, $"<p class='say'>You say {text}</p>", "room");

        }

        public void SayTo(string text, string target, Room room, Player player)
        {
            //find target
            var sayTo = room.Players.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

            if (sayTo == null)
            {
                _writer.WriteLine("<p>They are not here.</p>", player.ConnectionId);
                return;
            }

            _writer.WriteLine($"<p class='say'>You say to {sayTo.Name}, {text}</p>", player.ConnectionId);
            _updateClient.UpdateCommunication(player, $"<p class='say'>You say to {sayTo.Name}, {text}</p>", "room");
            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                if (pc.Name == sayTo.Name)
                {
                    _writer.WriteLine($"<p class='say'>{player.Name} says to you, {text}</p>", pc.ConnectionId);
                    _updateClient.UpdateCommunication(pc, $"<p class='say'>{player.Name} says to you, {text}</p>", "room");
                }
                else
                {
                    _writer.WriteLine($"<p class='say'>{player.Name} says to {sayTo.Name}, {text}</p>", pc.ConnectionId);
                    _updateClient.UpdateCommunication(pc, $"<p class='say'>{player.Name} says to {sayTo.Name}, {text}</p>", "room");
                }
            }
          
        }

        public void Tells(string text, Room room, Player player)
        {
            throw new NotImplementedException();
        }

        public void Whisper(string text, string target, Room room, Player player)
        {
            throw new NotImplementedException();
        }

        public void Yell(string text, Room room, Player player)
        {
         
            var rooms = _cache.GetAllRoomsInArea(room.AreaId);
            _writer.WriteLine($"<p class='yell'>You yell, {text.ToUpper()}</p>", player.ConnectionId);

            foreach (var rm in rooms)
            {
                foreach (var pc in rm.Players)
                {
                    if (pc.Name == player.Name)
                    {
                        continue;
                    }

                    _writer.WriteLine($"<p class='yell'>{player.Name} yells, {text.ToUpper()}</p>", pc.ConnectionId);
                    _updateClient.UpdateCommunication(pc, $"<p class='yell'>{player.Name} yells, {text.ToUpper()}</p>", "room");
                }
              
            }

            _updateClient.UpdateCommunication(player, $"<p class='yell'>You yell, {text.ToUpper()}</p>", "room");
        }
    }
}
