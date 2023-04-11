using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class EquipmentCmd : ICommand
    {
        public EquipmentCmd()
        {
            Aliases = new[] { "hold", "wear", "remove", "wield", "eq", "equipment" };
            Description =
                @"'{yellow}Wear{/}' is used to wear a piece of armour from your inventory, if you are already wearing a piece of armour 
in the same slot it will automatically remove it.

Examples:
wear vest
wear all - wear everything in your inventory until all EQ slots are filled

'{yellow}remove{/}' is used to remove a piece of armour or stop using a weapon. Removed items return to your inventory.

Examples:
remove vest
remove all - remove everything you are wearing

{yellow}wield{/}' is used to equip a weapon

Examples:
wield staff

'{yellow}eq{/}' or {yellow}equipment{/}' is used to display all your equipment and the equipment slots available

Examples:
eq
equipment

{yellow}hold{/}' is used to hold an item, this can be useful such as if blind, and you're holding a potion you will be able to quaff it, other items that can only be held may give benefits 

Examples:
hold potion
";
            Usages = new[] { "Type: wear vest, remove vest, wield dagger, eq, hold doll" };
            Title = "Equipment";
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Busy,
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Ghost,
                CharacterStatus.Status.Fleeing,
                CharacterStatus.Status.Incapacitated,
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Stunned
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
            var command = input.ElementAtOrDefault(0);
            var target = input.ElementAtOrDefault(1);

            switch (command)
            {
                case "wear":
                case "hold":

                    if (
                        command == "wear"
                            && !CommandTargetCheck(target, player, "<p>Wear what?</p>")
                        || command == "hold"
                            && !CommandTargetCheck(target, player, "<p>Hold what?</p>")
                    )
                    {
                        return;
                    }

                    if (target == "all")
                    {
                        player.WearAll(room);
                        return;
                    }

                    player.Wear(target, room, command == "hold" ? "hold" : String.Empty);

                    break;
                case "remove":

                    if (!CommandTargetCheck(target, player, "<p>Remove what?</p>"))
                    {
                        return;
                    }

                    if (target == "all")
                    {
                        player.RemoveAll(room);
                        return;
                    }
                    player.Remove(target, room);
                    break;
                case "eq":
                case "equipment":
                    player.DisplayEquipmentUI();
                    break;
                case "wield":
                    if (!CommandTargetCheck(target, player, "<p>Wield what?</p>"))
                    {
                        return;
                    }

                    player.Wear(target, room, "wield");
                    break;
            }
        }

        private bool CommandTargetCheck(string target, Player player, string errorMessage = "What?")
        {
            if (!string.IsNullOrEmpty(target))
                return true;
            Services.Instance.Writer.WriteLine(errorMessage, player.ConnectionId);
            return false;
        }
    }
}
