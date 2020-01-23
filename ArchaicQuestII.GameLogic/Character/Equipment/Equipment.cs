using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.GameLogic.Character.Equipment
{
    public class Equipment
    { 
        public Item.Item Light { get; set; }
        public Item.Item Finger { get; set; }
        public Item.Item  Finger2 { get; set; } 
        public Item.Item  Neck { get; set; } 
        public Item.Item  Neck2 { get; set; } 
        public Item.Item  Eyes { get; set; } 
        public Item.Item  Face { get; set; } 
        public Item.Item  Head { get; set; } 
        public Item.Item  Ear { get; set; } 
        public Item.Item  Torso { get; set; } 
        public Item.Item  Legs { get; set; } 
        public Item.Item  Feet { get; set; } 
        public Item.Item  Hands { get; set; } 
        public Item.Item  Arms { get; set; } 
        public Item.Item  AboutBody { get; set; } 
        public Item.Item  OnBack { get; set; } 
        public Item.Item  Waist { get; set; } 
        public Item.Item  Wrist { get; set; } 
        public Item.Item  Wrist2 { get; set; } 
        public Item.Item  Wielded { get; set; } 
        public Item.Item  Shield { get; set; } 
        public Item.Item  Held { get; set; } 
        public Item.Item  Floating { get; set; } 
        public Item.Item  Quiver { get; set; }


        public void Wear(Character character, Item.Item item)
        {
            switch (item.Slot)
            {
                //case 0:
                //    character.Equipped.Arms = item;
                //    break;
                //default:
                //    character.Equipped.Held = item;
                //    break;
            }



        }
    }
}
