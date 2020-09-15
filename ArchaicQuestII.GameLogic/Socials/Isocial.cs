using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Emote;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Socials
{
    public interface ISocials
    {
        public void EmoteSocial(Player player, Room room, Emote social, string target);
    }
}
