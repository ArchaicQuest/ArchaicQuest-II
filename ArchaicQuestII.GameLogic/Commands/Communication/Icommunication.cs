using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication
{
   public  interface Icommunication
   {
       void Say(string text, Room room, Player player);
       void SayTo(string text, string target, Room room, Player player);
       void Whisper(string text, string target, Room room, Player player);
       void Yell(string text, Room room, Player player);
    }
}
