using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Item
{
    public class Portal
    {
        public string Name { get; set; }
        /// <summary>
        /// AreaIdRoomID
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// What player sees when entering the portal
        /// </summary>
        public string EnterDescription { get; set; }

        /// <summary>
        /// What room sees when player enters portal
        /// </summary>
        public string EnterDescriptionRoom { get; set; }

        public string ExitDescription { get; set; }
        /// <summary>
        /// What room sees when player exits portal
        /// </summary>
        public string ExitDescriptionRoom { get; set; }

    }
}
