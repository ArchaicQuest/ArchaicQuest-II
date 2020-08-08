using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication
{
    public class Communication: Icommunication
    {

        private readonly IWriteToClient _writer;
        public Communication(IWriteToClient writer)
        {
            _writer = writer;
        }
        public void Say(string text, Room room, Player player)
        {
            _writer.WriteLine($"<p>You say {text}</p>", player.ConnectionId);
            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                _writer.WriteLine($"<p>{pc.Name} says {text}</p>", pc.ConnectionId);
            }

        }

        public void SayTo(string text, string target, Room room, Player player)
        {
            throw new NotImplementedException();
        }

        public void Whisper(string text, string target, Room room, Player player)
        {
            throw new NotImplementedException();
        }

        public void Yell(string text, Room room, Player player)
        {
            throw new NotImplementedException();
        }
    }
}
