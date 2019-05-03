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
                    character.Equipment.Arms = item.Name;
                    break;
                default:
                    character.Equipment.Held = item.Name;
                    break;
            }



        }

 
    }
}
