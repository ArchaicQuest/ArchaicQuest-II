using System;
using System.Linq;
using System.Web;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;

namespace ArchaicQuestII.GameLogic.World;

public static class WorldEvents
{
    public static void LoopRooms(ICoreHandler coreHandler)
    {
        Console.Write("Room Loop Started");
        
        var rooms = coreHandler.World.GetAllRoomsToRepop();
        
        foreach (var room in rooms)
        {
            var originalRoom = coreHandler.World.GetOriginalRoom(room.StringId);

            foreach (var mob in originalRoom.Mobs)
            {
                var mobExist = rooms.Find(x => x.Mobs.Any(y => y.UniqueId.Equals(mob.UniqueId)))
                    ?.Mobs.FirstOrDefault(z => z.UniqueId.Equals(mob.UniqueId));

                if (mobExist == null)
                {
                    room.Mobs.Add(mob);
                }
                else
                {
                    if (mob.Status != CharacterStatus.Status.Fighting)
                    {

                        mobExist.Attributes.Attribute[EffectLocation.Hitpoints] +=
                            DiceBag.Roll(1, 2, 5) + mobExist.Level;
                        mobExist.Attributes.Attribute[EffectLocation.Mana] +=
                            DiceBag.Roll(1, 2, 5) + mobExist.Level;
                        mobExist.Attributes.Attribute[EffectLocation.Moves] +=
                            DiceBag.Roll(1, 2, 5) + mobExist.Level;
                    }
                    else
                    {
                        mobExist.Attributes.Attribute[EffectLocation.Hitpoints] +=
                            (DiceBag.Roll(1, 1, 5) + mobExist.Level) / 2;
                        mobExist.Attributes.Attribute[EffectLocation.Mana] +=
                            (DiceBag.Roll(1, 1, 5) + mobExist.Level) / 2;
                        mobExist.Attributes.Attribute[EffectLocation.Moves] +=
                            (DiceBag.Roll(1, 1, 5) + mobExist.Level) / 2;
                    }

                    if (mobExist.Attributes.Attribute[EffectLocation.Hitpoints] >
                        mobExist.MaxAttributes.Attribute[EffectLocation.Hitpoints])
                    {
                        mobExist.Attributes.Attribute[EffectLocation.Hitpoints] =
                            mobExist.MaxAttributes.Attribute[EffectLocation.Hitpoints];
                    }

                    if (mobExist.Attributes.Attribute[EffectLocation.Mana] >
                        mobExist.MaxAttributes.Attribute[EffectLocation.Mana])
                    {
                        mobExist.Attributes.Attribute[EffectLocation.Mana] =
                            mobExist.MaxAttributes.Attribute[EffectLocation.Mana];
                    }

                    if (mobExist.Attributes.Attribute[EffectLocation.Moves] >
                        mobExist.MaxAttributes.Attribute[EffectLocation.Moves])
                    {
                        mobExist.Attributes.Attribute[EffectLocation.Moves] =
                            mobExist.MaxAttributes.Attribute[EffectLocation.Moves];
                    }
                }
            }

            //get corpse and remove
            var corpses = room.Items.FindAll(x =>
                x.Description.Room.Contains("corpse", StringComparison.CurrentCultureIgnoreCase));

            foreach (var corpse in corpses.Where(DecayCorpse))
            {
                coreHandler.Client.WriteLineRoom($"<p>A quivering horde of maggots consumes {corpse.Name.ToLower()}.</p>", room);
                room.Items.Remove(corpse);
            }
            

            foreach (var item in originalRoom.Items)
            {
                // need to check if item exists before adding
                var itemExist = room.Items.FirstOrDefault(x => x.Id.Equals(item.Id));

                if (itemExist == null)
                {
                    room.Items.Add(item);
                }

                itemExist = room.Items.FirstOrDefault(x => x.Id.Equals(item.Id));

                if (itemExist?.Container.Items.Count < item.Container.Items.Count)
                {
                    itemExist.Container.Items = item.Container.Items;
                    itemExist.Container.IsOpen = item.Container.IsOpen;
                    itemExist.Container.IsLocked = item.Container.IsLocked;
                }

            }

            // reset doors
            room.Exits = originalRoom.Exits;

            //set room clean
            // if (room.Players.Count == 0)
            //   {
            room.Clean = true;
            coreHandler.Client.WriteLineRoom("<p>The hairs on your neck stand up.</p>", room);
            
            //  }
        }
    }

    private static bool DecayCorpse(Item.Item corpse)
    {
        corpse.Decay--;
        
        switch (corpse.Decay)
        {
            case 10:
            case 9:
            case 8:
            case 7:
            case 6:
                corpse.Description.Room = $"{corpse.Name.ToLower()} lies here.";
                return false;
            case 5:
            case 4:
                corpse.Description.Room = $"{corpse.Name.ToLower()} is buzzing with flies.";
                return false;
            case 3:
                corpse.Description.Room = $"{corpse.Name.ToLower()} fills the air with a foul stench.";
                return false;
            case 2:
                corpse.Description.Room = $"{corpse.Name.ToLower()} is crawling with vermin.";
                return false;
            case 1:
                corpse.Description.Room = $"{corpse.Name.ToLower()} is in the last stages of decay.";
                return false;
            case 0:
                return true;
            default:
                corpse.Description.Room = $"{corpse.Name.ToLower()} lies here.";
                return false;
        }
    }
}