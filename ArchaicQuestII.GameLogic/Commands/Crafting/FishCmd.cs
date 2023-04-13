using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class FishCmd : ICommand
{
    public FishCmd()
    {
        Aliases = new[] { "fish", };
        Description =
            "Fish at fishing spots for a chance to catch fish. If you're lucky you may catch the fish of the day. Requires a fishing rod in inventory to fish.";
        Usages = new[] { "Type: fish" };
        Title = "";
        DeniedStatus = new[]
        {
            CharacterStatus.Status.Busy,
            CharacterStatus.Status.Dead,
            CharacterStatus.Status.Fighting,
            CharacterStatus.Status.Ghost,
            CharacterStatus.Status.Fleeing,
            CharacterStatus.Status.Incapacitated,
            CharacterStatus.Status.Sleeping,
            CharacterStatus.Status.Stunned,
            CharacterStatus.Status.Resting,
            CharacterStatus.Status.Sitting,
        };
        UserRole = UserRole.Player;
    }

    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        if (player.Status == CharacterStatus.Status.Busy)
        {
            Services.Instance.Writer.WriteLine("You are already doing it.", player);
            return;
        }

        var fishingSpot = room.Items.FirstOrDefault(
            x => x.ItemType == Item.Item.ItemTypes.FishingSpot
        );

        if (fishingSpot == null)
        {
            Services.Instance.Writer.WriteLine($"You can't fish here.", player);
            return;
        }

        var hasRod = player.Inventory.FirstOrDefault(
            x => x.ItemType == Item.Item.ItemTypes.FishingRod
        );

        if (hasRod == null)
        {
            Services.Instance.Writer.WriteLine(
                $"You attempt to catch fish with your bare hands. Probably better to try with a fishing rod.",
                player
            );
            return;
        }

        Harvest(player, room);
    }

    private async void Harvest(Player player, Room room)
    {
        player.Status = CharacterStatus.Status.Busy;

        // Core.UpdateClient.PlaySound("chopping", player);

        Services.Instance.Writer.WriteLine($"<p>You cast your line and begin fishing.</p>", player);
        Services.Instance.Writer.WriteLine($"<p>*PLOP*.</p>", player);

        foreach (var character in room.Players.Where(character => character != player))
        {
            Services.Instance.Writer.WriteLine(
                $"<p>{player.Name} casts their line and begin fishing.</p>",
                character
            );
            Services.Instance.Writer.WriteLine($"<p>*PLOP*.</p>", character);
        }

        await Task.Delay(4000);

        if (!room.Players.Contains(player) || player.Status != CharacterStatus.Status.Busy)
        {
            return;
        }

        //  Core.UpdateClient.PlaySound("chopping", player);

        Services.Instance.Writer.WriteLine(
            $"<p>You throw some bait and gently reel in slightly.</p>",
            player
        );

        foreach (var character in room.Players.Where(character => character != player))
        {
            Services.Instance.Writer.WriteLine(
                $"<p>{player.Name} throws some bait and gently reels in slightly.</p>",
                character
            );
        }

        await Task.Delay(4000);

        if (!room.Players.Contains(player) || player.Status != CharacterStatus.Status.Busy)
        {
            return;
        }

        //Core.UpdateClient.PlaySound("chopping", player);

        Services.Instance.Writer.WriteLine(
            "<p>You continue fishing, enjoying the serene tranquility of your fishing spot.</p>",
            player
        );

        foreach (var character in room.Players.Where(character => character != player))
        {
            Services.Instance.Writer.WriteLine(
                $"<p>{player.Name} continues fishing.</p>",
                character
            );
        }

        await Task.Delay(4000);

        if (!room.Players.Contains(player) || player.Status != CharacterStatus.Status.Busy)
        {
            return;
        }

        var roll = DiceBag.Roll(1, 1, 10);

        var randomMobObj = roll switch
        {
            1
                => new
                {
                    Name = "A water snake",
                    Description = "A black and red water snake",
                    LongName = "A black snake slithers along here.",
                    DefaultAttack = "bite"
                },
            _
                => new
                {
                    Name = "An Alligator",
                    Description = "An angry large alligator with thick rubbery skin and a large mouth full of sharp teeth.",
                    LongName = "An angry alligator is here with it's mouth open.",
                    DefaultAttack = "bite"
                },
        };

        var randomMob = new Player
        {
            Name = randomMobObj.Name,
            ClassName = ClassName.Fighter.ToString(),
            Target = string.Empty,
            Status = CharacterStatus.Status.Standing,
            Race = "Other",
            Level = roll + 2,
            RoomId = player.RoomId,
            Attributes = new Attributes
            {
                Attribute = new Dictionary<EffectLocation, int>
                {
                    { EffectLocation.Mana, 30 },
                    { EffectLocation.Hitpoints, 300 },
                    { EffectLocation.Moves, 30 },
                    { EffectLocation.Strength, 30 },
                    { EffectLocation.Dexterity, 30 },
                    { EffectLocation.Constitution, 30 },
                    { EffectLocation.Intelligence, 30 },
                    { EffectLocation.Wisdom, 30 },
                    { EffectLocation.Charisma, 30 },
                    { EffectLocation.HitRoll, 2 },
                    { EffectLocation.DamageRoll, 1 },
                    { EffectLocation.SavingSpell, 1 },
                }
            },
            MaxAttributes = new Attributes
            {
                Attribute = new Dictionary<EffectLocation, int>
                {
                    { EffectLocation.Mana, 30 },
                    { EffectLocation.Hitpoints, 300 },
                    { EffectLocation.Moves, 30 },
                    { EffectLocation.Strength, 30 },
                    { EffectLocation.Dexterity, 30 },
                    { EffectLocation.Constitution, 30 },
                    { EffectLocation.Intelligence, 30 },
                    { EffectLocation.Wisdom, 30 },
                    { EffectLocation.Charisma, 30 },
                    { EffectLocation.HitRoll, 2 },
                    { EffectLocation.DamageRoll, 1 },
                    { EffectLocation.SavingSpell, 1 },
                }
            },
            Description = randomMobObj.Description,
            LongName = randomMobObj.LongName,
            ArmorRating = new ArmourRating { Armour = 5, Magic = 5 },
            DefaultAttack = randomMobObj.DefaultAttack,
            UniqueId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };

        if (roll <= 1)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>{{yellow}}You reel in {randomMob.Name} that attacks you!{{/}}</p>",
                player
            );
            room.Mobs.Add(randomMob);
            player.Status = CharacterStatus.Status.Standing;
            var combat = new Fight(randomMob, player, room, false);
            Services.Instance.Cache.AddCombat(combat);
            return;
        }

        if (roll <= 3)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>{{yellow}}You cut yourself on the fishing hook, OUCH!{{/}}</p>",
                player
            );
            player.Status = CharacterStatus.Status.Standing;

            foreach (var character in room.Players.Where(character => character != player))
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>{player.Name} cuts themself on a fishing hook.</p>",
                    character
                );
            }
            return;
        }

        if (!player.RollSkill(SkillName.Fishing, false))
        {
            player.FailedSkill(SkillName.Fishing, true);
            Services.Instance.Writer.WriteLine("<p>You fail to catch any fish.</p>", player);
            player.Status = CharacterStatus.Status.Standing;
            return;
        }

        var caught = PossibleFish();
        var weight = FishWeight();
        var sizeText = FishSizeGrade(weight);

        caught.Weight = weight;

        // TODO mention if biggest catch today, biggest This week, This Month
        caught.Name = $"A {sizeText} {caught.Name}";
        Services.Instance.Writer.WriteLine(
            $"<p>You caught {caught.Name.ToLower()}. It weighs {caught.Weight.ToString("0.00")} pounds!</p>",
            player
        );
        player.Inventory.Add(caught);

        player.Status = CharacterStatus.Status.Standing;
        Services.Instance.UpdateClient.UpdateInventory(player);
    }

    private Item.Item PossibleFish()
    {
        var fish = new Item.Item
        {
            Uuid = Guid.NewGuid(),
            Id = 10000 + DiceBag.Roll(1, 100, 2000),
            Name = "Fish",
            Level = 1,
            Value = 0,
            KnownByName = false,
            Description = new Description() { Look = "", },
            Keywords = null,
            IsHiddenInRoom = false,
            Hidden = false,
            Stuck = false,
            DecayTimer = 0,
            Condition = 0,
            QuestItem = false,
            DateUpdated = default,
            Equipped = false,
            DamageType = Item.Item.DamageTypes.None,
            ArmourType = Item.Item.ArmourTypes.Cloth,
            ItemFlag = Item.Item.ItemFlags.None,
            ItemType = Item.Item.ItemTypes.Food,
            AttackType = Item.Item.AttackTypes.Charge,
            Slot = EquipmentSlot.Arms,
            Forage = null,
            WeaponType = SkillName.None,
            WeaponSpeed = 0,
            Damage = null,
            KeyId = default,
            Container = null,
            Book = null,
            Modifier = null,
            ArmourRating = null,
            Weight = DiceBag.Roll(1, 1, 100),
            Gold = 0,
            Silver = 0,
            ForageRank = 0,
            Uses = 0,
            Infinite = false,
            Deleted = false,
            TwoHanded = false,
            Portal = null,
            Decay = 0,
            SpellName = null,
            SpellLevel = null,
            DecayMessages = null
        };

        var typeOfFish = DiceBag.Roll(1, 1, 16);

        fish.Name = "Perch";
        fish.Description = new Description()
        {
            Look =
                "Perch fish are a freshwater fish that are typically found in lakes and rivers. "
                + "They have a distinctive spiny dorsal fin and are usually yellowish-green in color with vertical black stripes along their sides."
        };

        switch (typeOfFish)
        {
            case 1:
                fish.Name = "Silvery Green Bream";
                fish.Description = new Description()
                {
                    Look =
                        "A bream fish is a freshwater fish that is typically found in rivers, lakes, and ponds. "
                        + "They have a deep, flattened body and are generally silver, green, or brown in color with a slightly forked tail."
                };
                break;
            case 2:
            case 3:
                fish.Name = "Brown Trout";
                fish.Description = new Description()
                {
                    Look =
                        "Trout fish are a type of freshwater fish found in rivers, streams, and lakes."
                        + " They have a streamlined body shape and are typically brown or rainbow-colored with small black spots along their sides."
                };
                break;
            case 4:
            case 5:
                fish.Name = "A Golden Carp";
                fish.Description = new Description()
                {
                    Look =
                        "Carp fish are a freshwater fish that are typically found in lakes, rivers, and ponds. "
                        + "They have a large, scaled body that can range in color from gold to gray, and are often used for food or as a sport fish."
                };
                break;
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
                fish.Name = "Grey Chub";
                fish.Description = new Description()
                {
                    Look =
                        "Chub fish are a freshwater fish that are commonly found in rivers and streams."
                        + " They have a deep, rounded body shape with large scales, and are typically brown or gray in color with a white underbelly."
                };
                break;
            case 11:
                fish.Name = "Yellowish-green Perch";
                fish.Description = new Description()
                {
                    Look =
                        "Perch fish are a freshwater fish that are typically found in lakes and rivers. "
                        + "They have a distinctive spiny dorsal fin and are usually yellowish-green in color with vertical black stripes along their sides."
                };
                break;
            case 12:
                fish.Name = "Snapping turtle";
                fish.Description = new Description()
                {
                    Look =
                        "Snapping turtles are freshwater turtles known for their aggressive behavior and powerful bite."
                        + " They have a large, muscular head, a long tail, and rough, scaly skin, and are typically found in ponds, lakes, and slow-moving streams."
                };
                break;
            case 13:
                fish.Name = "Brown Eel";
                fish.Description = new Description()
                {
                    Look =
                        "Eels are a type of fish that can be found in both freshwater and saltwater environments. "
                        + "They have a long, snake-like body with smooth, slimy skin, and are typically brown or greenish-brown in color."
                };
                break;
            case 14:
                fish.Name = "Green Frog";
                fish.Description = new Description()
                {
                    Look =
                        "Frogs are amphibians that are known for their long, sticky tongues and powerful hind legs, "
                        + "which they use to jump and catch prey. They have smooth, moist skin and are typically green or brown in color, "
                        + "making them excellent at blending in with their surroundings."
                };
                break;
        }
        return fish;
    }

    private string FishSizeGrade(float weight)
    {
        double[] weightRanges = new double[] { 10, 30, 60, 90, 120, 150, 180, 230 };
        string[] sizeCategories = new string[]
        {
            "small",
            "medium",
            "large",
            "trophy",
            "monster",
            "behemoth",
            "goliath",
            "titan"
        };

        // Initialize the fish size to "unknown"
        string size = "unknown";

        // Iterate through the fish size categories to find the size of the fish
        for (int i = 0; i < weightRanges.Length; i++)
        {
            if (weight <= weightRanges[i])
            {
                size = sizeCategories[i];
                break;
            }
        }

        return size;
    }

    private float FishWeight()
    {
        /*
            small 0.5 - 10
            medium 10 - 30
            large: 30 - 60
            trophy: 60 - 90
            Monster: 90 - 120
            Behemoth: 120 - 150
            Goliath: 150 - 180
            Titan: 180 - 230
        */

        // Define the weight ranges for each fish category
        double[] weightRanges = new double[] { 10, 30, 60, 90, 120, 150, 180, 230 };

        // Define the chances of catching each fish category
        double[] chances = new double[] { 0.5, 0.3, 0.1, 0.05, 0.03, 0.015, 0.007, 0.005 };

        Random rng = new Random(Guid.NewGuid().GetHashCode());
        // Get a random number between 0 and 1
        double random = rng.NextDouble();

        // Initialize the weight of the fish to zero
        double weight = 0;

        // Iterate through the fish categories to find which one the player caught
        for (int i = 0; i < weightRanges.Length; i++)
        {
            if (random < chances[i])
            {
                // Get a random weight within the weight range for this fish category
                weight =
                    new Random().NextDouble()
                        * (weightRanges[i] - (i == 0 ? 0 : weightRanges[i - 1]))
                    + (i == 0 ? 0 : weightRanges[i - 1]);
                break;
            }
            random -= chances[i];
        }

        return (float)weight;
    }
}
