using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects
{
   public class Object: IObject
    {

        public Object()
        {
            
        }
        public void Get(string target, Room room, Player player)
        {
            //Check room first
            var item = room.Items.Find(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                // you dont see taht here
            }

            player.Inventory.Add(item);

        }

        public void Drop(string target, Room room, Player player)
        {
            throw new NotImplementedException();
        }

        public void Open(string target, Room room, Player player)
        {
            throw new NotImplementedException();
        }

        public void Close(string target, Room room, Player player)
        {
            throw new NotImplementedException();
        }

        public void Delete(string target, Room room, Player player)
        {
            throw new NotImplementedException();
        }

    
    }
}
