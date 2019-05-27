using ArchaicQuestII.Engine.Item;


namespace ArchaicQuestII.Engine.Character.Equipment.Commands
{
    public class WearEquipmentCommand
    {
        public void Wear(Character.Model.Character character, Item.Item item)
        {
            switch (item.Slot)
            {
                case 0:
                    character.Equipped.Arms = item;
                    break;
                default:
                    character.Equipped.Held = item;
                    break;
            }



        }

 
    }
}
