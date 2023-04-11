using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class LookCmd : ICommand
    {
        public LookCmd()
        {
            Aliases = new[] { "look", "l" };
            Description =
                "Shows you the current room title, description and what items, mobs, and players are there. Look with an argument will show more information on that object.";
            Usages = new[] { "Type: look" };
            Title = "";
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Incapacitated,
                CharacterStatus.Status.Sleeping
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
            var verb = input.ElementAtOrDefault(1)?.ToLower();
            var target = input.ElementAtOrDefault(2)?.ToLower();

            if (string.IsNullOrEmpty(verb))
            {
                Look(player, room);
                return;
            }

            if (string.IsNullOrEmpty(target))
            {
                switch (verb)
                {
                    case "at" when string.IsNullOrEmpty(target):
                        Services.Instance.Writer.WriteLine("<p>Look at what?</p>", player);
                        return;
                    case "in" when string.IsNullOrEmpty(target):
                        Services.Instance.Writer.WriteLine("<p>Look in what?</p>", player);
                        return;
                }
            }

            if (!string.IsNullOrEmpty(verb) && string.IsNullOrEmpty(target))
            {
                LookAtObject(player, room, verb);
                return;
            }

            if (!string.IsNullOrEmpty(verb) && !string.IsNullOrEmpty(target))
            {
                LookInContainer(player, room, target);
            }
        }

        private void Look(Player player, Room room)
        {
            var showVerboseExits = player.Config.VerboseExits;
            var exits = Services.Instance.RoomActions.FindValidExits(room, showVerboseExits);

            var items = DisplayItems(room, player);
            var mobs = DisplayMobs(room, player);
            var players = DisplayPlayers(room, player);
            var roomDesc = new StringBuilder();

            roomDesc.Append(
                $"<p class=\"room-title {(!player.CanSee(room) ? "room-dark" : "")}\">{room.Title} ({room.Coords.X},{room.Coords.Y},{room.Coords.Z})<br /></p>"
            );

            // With brief toggled we don't show the room description
            if (!player.Config.Brief)
            {
                roomDesc.Append(
                    $"<p class=\"room-description  {(!player.CanSee(room) ? "room-dark" : "")}\">{room.Description}</p>"
                );
            }

            if (!showVerboseExits)
            {
                roomDesc.Append(
                    $"<p class=\"room-exit  {(!player.CanSee(room) ? "room-dark" : "")}\"> <span class=\"room-exits\">[</span>Exits: <span class=\"room-exits\">{exits}</span><span class=\"room-exits\">]</span></p>"
                );
            }
            else
            {
                roomDesc.Append(
                    $"<div class=\" {(!player.CanSee(room) ? "room-dark" : "")}\">Obvious exits: <table class=\"room-exits\"><tbody>{exits}</tbody></table></div>"
                );
            }

            roomDesc
                .Append($"<p  class=\" {(!player.CanSee(room) ? "room-dark" : "")}\">{items}</p>")
                .Append($"<p  class=\"{(!player.CanSee(room) ? "room-dark" : "")}\">{mobs}</p>")
                .Append(
                    $"<p  class=\"  {(!player.CanSee(room) ? "room-dark" : "")}\">{players}</p>"
                );

            Services.Instance.Writer.WriteLine(roomDesc.ToString(), player);
        }

        private void LookInContainer(Player player, Room room, string target)
        {
            var nthTarget = Helpers.findNth(target);
            var container =
                Helpers.findRoomObject(nthTarget, room) ?? player.FindObjectInInventory(nthTarget);

            if (
                container != null
                && container.ItemType != Item.Item.ItemTypes.Container
                && container.ItemType != Item.Item.ItemTypes.Cooking
            )
            {
                if (container.ItemType == Item.Item.ItemTypes.Portal)
                {
                    LookInPortal(player, room, container);
                    return;
                }

                Services.Instance.Writer.WriteLine(
                    $"<p>{container.Name} is not a container",
                    player
                );
                return;
            }

            if (container == null)
            {
                Services.Instance.Writer.WriteLine("<p>You don't see that here.", player);
                return;
            }

            if (container.Container.IsOpen == false)
            {
                Services.Instance.Writer.WriteLine("<p>You need to open it first.", player);
                return;
            }

            Services.Instance.Writer.WriteLine($"<p>{container.Name} contains:</p>", player);

            if (container.Container.Items.Count == 0)
            {
                Services.Instance.Writer.WriteLine("<p>Nothing.</p>", player);
            }

            foreach (var obj in container.Container.Items.List(false))
            {
                Services.Instance.Writer.WriteLine(
                    $"<span class='item {(!player.CanSee(room) ? "room-dark" : "")}'>{obj.Name}</span>",
                    player
                );
            }

            foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>{player.Name} looks inside {container.Name.ToLower()}.</p>",
                    pc
                );
            }
        }

        private void LookInPortal(Player player, Room room, Item.Item target)
        {
            var getPortalLocation = Services.Instance.Cache.GetRoom(target.Portal.Destination);

            if (getPortalLocation == null)
            {
                Services.Instance.ErrorLog.Write(
                    "LookCmd.cs",
                    $"Portal ({target.Portal.Name}) location empty.",
                    ErrorLog.Priority.Medium
                );
                Services.Instance.Writer.WriteLine(
                    "<p>The dark abyss, I wouldn't enter if I were you.</p>",
                    player
                );
                return;
            }

            Look(player, getPortalLocation);
        }

        private void LookAtObject(Player player, Room room, string target)
        {
            var nthTarget = Helpers.findNth(target);

            var item =
                Helpers.findRoomObject(nthTarget, room) ?? player.FindObjectInInventory(nthTarget);
            var character = Helpers.FindMob(nthTarget, room) ?? Helpers.FindPlayer(nthTarget, room);

            RoomObject roomObjects = null;
            if (room.RoomObjects.Count >= 1 && room.RoomObjects[0].Name != null)
            {
                roomObjects = room.RoomObjects.FirstOrDefault(
                    x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)
                );
            }

            if (target.Equals("self", StringComparison.CurrentCultureIgnoreCase))
            {
                character = player;
            }

            if (item == null && character == null && roomObjects == null)
            {
                Services.Instance.Writer.WriteLine("<p>You don't see that here.", player);
                return;
            }

            if (item != null)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p  class='{(!player.CanSee(room) ? "room-dark" : "")}'>{item.Description.Look}",
                    player
                );

                if (player.HasSkill(SkillName.Lore))
                {
                    if (player.RollSkill(SkillName.Lore, false))
                    {
                        Services.Instance.PassiveSkills.Lore(player, room, item.Name);
                    }
                    else
                    {
                        player.FailedSkill(SkillName.Lore, false);
                    }
                }

                if (item.Container is { CanOpen: false } && item.Container.Items.Any())
                {
                    Services.Instance.Writer.WriteLine(
                        $"<p  class='{(!player.CanSee(room) ? "room-dark" : "")}'>{item.Name} contains:",
                        player
                    );

                    var listOfContainerItems = new StringBuilder();
                    foreach (var containerItem in item.Container.Items.List())
                    {
                        listOfContainerItems.Append(
                            $"<p class='{(!player.CanSee(room) ? "room-dark" : "")} container-item'>{containerItem.Name.Replace(" lies here.", "")}</p>"
                        );
                    }

                    Services.Instance.Writer.WriteLine(listOfContainerItems.ToString(), player);
                }

                foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
                {
                    Services.Instance.Writer.WriteLine(
                        $"<p>{player.Name} looks at {item.Name.ToLower()}.</p>",
                        pc
                    );
                }

                return;
            }

            //for player?
            if (roomObjects != null)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p class='{(!player.CanSee(room) ? "room-dark" : "")}'>{roomObjects.Look}",
                    player
                );

                foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
                {
                    Services.Instance.Writer.WriteLine(
                        $"<p>{player.Name} looks at {roomObjects.Name.ToLower()}.</p>",
                        pc
                    );
                }

                return;
            }

            if (character == null)
            {
                Services.Instance.Writer.WriteLine("<p>You don't see them here.", player);
                return;
            }

            var sb = new StringBuilder();

            if (character.ConnectionId != "mob")
            {
                sb.Append(
                    $"<table class='char-look'><tr><td><span class='cell-title'>Eyes:</span> {character.Eyes}</td><td><span class='cell-title'>Hair:</span> {character.HairColour}</td></tr>"
                );
                sb.Append(
                    $"<tr><td><span class='cell-title'>Skin:</span> {character.Skin}</td><td><span class='cell-title'>Hair Length:</span> {character.HairLength}</td></tr>"
                );
                sb.Append(
                    $"<tr><td><span class='cell-title'>Build:</span> {character.Build}</td><td><span class='cell-title'>Hair Texture:</span> {character.HairTexture}</td></tr>"
                );
                sb.Append(
                    $"<tr><td><span class='cell-title'>Face:</span> {character.Face}</td><td><span class='cell-title'>Hair Facial:</span> {character.FacialHair}</td></tr><table>"
                );
            }

            var status = Enum.GetName(typeof(CharacterStatus.Status), player.Status);

            var statusText = string.Empty;

            if (status.Equals("fighting", StringComparison.CurrentCultureIgnoreCase))
            {
                statusText =
                    $"fighting {(character.Target.Equals(player.Name) ? "YOU!" : character.Target)}.";
            }
            else
            {
                statusText = $"{Enum.GetName(typeof(CharacterStatus.Status), player.Status)}.";
                statusText = statusText.ToLower();
            }

            var displayEquipment = new StringBuilder();
            displayEquipment.Append("<p>They are using:</p>").Append("<table>");

            if (character.Equipped.Light != null)
            {
                displayEquipment
                    .Append(
                        "<tr><td style='width:175px;' class=\"cell-title\" title='Worn as light'>"
                    )
                    .Append("&lt;used as light&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Light?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Finger != null)
            {
                displayEquipment
                    .Append("<tr><td class=\"cell-title\" title='Worn on finger'>")
                    .Append(" &lt;worn on finger&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Finger?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Finger2 != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Worn on finger'>")
                    .Append(" &lt;worn on finger&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Finger2?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Neck != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Worn around neck'>")
                    .Append(" &lt;worn around neck&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Neck?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Neck2 != null)
            {
                displayEquipment
                    .Append("<tr><td class=\"cell-title\" title='Worn around neck'>")
                    .Append(" &lt;worn around neck&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Neck2?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Face != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Worn on face'>")
                    .Append(" &lt;worn on face&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Face?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Head != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Worn on head'>")
                    .Append(" &lt;worn on head&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Head?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Torso != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Worn on torso'>")
                    .Append(" &lt;worn on torso&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Torso?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Legs != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Worn on legs'>")
                    .Append(" &lt;worn on legs&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Legs?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Feet != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Worn on feet'>")
                    .Append(" &lt;worn on feet&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Feet?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Hands != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Worn on hands'>")
                    .Append(" &lt;worn on hands&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Hands?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Arms != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Worn on arms'>")
                    .Append(" &lt;worn on arms&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Arms?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.AboutBody != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Worn about body'>")
                    .Append(" &lt;worn about body&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.AboutBody?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Waist != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Worn on waist'>")
                    .Append(" &lt;worn about waist&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Waist?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Wrist != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Worn on wrist'>")
                    .Append(" &lt;worn around wrist&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Wrist?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Wrist2 != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Worn on wrist'>")
                    .Append(" &lt;worn around wrist&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Wrist2?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Wielded != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='worn as weapon'>")
                    .Append(" &lt;wielded&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Wielded?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Secondary != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='worn as weapon'>")
                    .Append(" &lt;secondary&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Secondary?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Shield != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Worn as shield'>")
                    .Append(" &lt;worn as shield&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Shield?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Held != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Held'>")
                    .Append(" &lt;Held&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Held?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Floating != null)
            {
                displayEquipment
                    .Append("<tr><td  class=\"cell-title\" title='Floating Nearby'>")
                    .Append(" &lt;Floating nearby&gt;")
                    .Append("</td>")
                    .Append("<td>")
                    .Append(character.Equipped.Floating?.ReturnWithFlags() ?? "(nothing)")
                    .Append("</td></tr>")
                    .Append("</table");
            }

            if (
                character.Equipped.Light == null
                && character.Equipped.Finger == null
                && character.Equipped.Finger2 == null
                && character.Equipped.Neck == null
                && character.Equipped.Neck2 == null
                && character.Equipped.Face == null
                && character.Equipped.Head == null
                && character.Equipped.Torso == null
                && character.Equipped.Legs == null
                && character.Equipped.Feet == null
                && character.Equipped.Hands == null
                && character.Equipped.Arms == null
                && character.Equipped.AboutBody == null
                && character.Equipped.Waist == null
                && character.Equipped.Wrist == null
                && character.Equipped.Wrist2 == null
                && character.Equipped.Wielded == null
                && character.Equipped.Secondary == null
                && character.Equipped.Shield == null
                && character.Equipped.Held == null
                && character.Equipped.Held == null
                && character.Equipped.Floating == null
            )
            {
                displayEquipment.Append("</table").Append("<p>Nothing.</p>");
            }

            Services.Instance.Writer.WriteLine(
                $"{sb}<p class='{(!player.CanSee(room) ? "room-dark" : "")}'>{character.Description} <br/>{character.Name} {Services.Instance.Formulas.TargetHealth(player, character)} and is {statusText}<br/> {displayEquipment}",
                player
            );

            foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>{player.Name} looks at {character.Name.ToLower()}.</p>",
                    pc
                );
            }
        }

        private string DisplayItems(Room room, Player player)
        {
            var items = room.Items.List();
            var x = string.Empty;

            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.Name))
                {
                    var i = room.Items.FirstOrDefault(x => x.Id.Equals(item.Id));
                    var keyword = i.Name.Split(" ");

                    var data =
                        $"{{detail: {{name: \" {HtmlEncoder.Default.Encode(i.Name)}\", desc: \"{HtmlEncoder.Default.Encode(i.Description.Look)}\", type: \"{i.ItemType}\", canOpen: \"{i.Container.CanOpen}\", isOpen: \"{i.Container.IsOpen}\", keyword: \"{HtmlEncoder.Default.Encode(keyword[keyword.Length - 1])}\"}}}}";

                    var clickEvent =
                        $"window.dispatchEvent(new CustomEvent(\"open-detail\", {data}))";
                    x +=
                        $"<p onClick='{clickEvent}' class='item {(!player.CanSee(room) ? "dark-room" : "")}' >{item.Name}</p>";
                }
            }

            return x;
        }

        private string DisplayMobs(Room room, Player player)
        {
            var mobs = string.Empty;
            var mobName = string.Empty;
            var isFightingPC = false;

            foreach (var mob in room.Mobs.Where(x => x.IsHiddenScriptMob == false))
            {
                if (!string.IsNullOrEmpty(mob.LongName))
                {
                    mobName = mob.LongName;
                }
                else
                {
                    mobName = mob.Name;
                }

                if (!string.IsNullOrEmpty(mob.Mounted.MountedBy))
                {
                    mobName += " tosses it's mane and snorts";
                }

                if (player.Target == mob.Name)
                {
                    isFightingPC = true;
                }

                if (!string.IsNullOrEmpty(mob.LongName))
                {
                    if (mob.Status == CharacterStatus.Status.Fighting)
                    {
                        mobs +=
                            $"<p class='mob {(!player.CanSee(room) ? "dark-room" : "")}'>{mob.Name} is here{(isFightingPC ? " fighting YOU!" : "")}</p>";
                    }
                    else
                    {
                        mobs +=
                            $"<p class='mob {(!player.CanSee(room) ? "dark-room" : "")}'>{mobName}</p>";
                    }
                }
                else
                {
                    mobs +=
                        $"<p class='mob {(!player.CanSee(room) ? "dark-room" : "")}'>{mobName} is here{(isFightingPC ? " fighting YOU!" : ".")}.</p>";
                }
            }

            return mobs;
        }

        private string DisplayPlayers(Room room, Player player)
        {
            var players = string.Empty;
            var pcName = string.Empty;

            foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
            {
                if (string.IsNullOrEmpty(pc.LongName))
                {
                    pcName = $"{pc.Name}";
                }
                else
                {
                    pcName = $"{pc.Name} {pc.LongName}";
                }

                if (!string.IsNullOrEmpty(pc.Mounted.Name))
                {
                    pcName += $", is riding {pc.Mounted.Name}";
                }
                else if (string.IsNullOrEmpty(pc.LongName))
                {
                    pcName += " is here";
                }

                pcName += pc.Pose;
                players +=
                    $"<p class='player {(!player.CanSee(room) ? "dark-room" : "")}'>{pcName}.</p>";
            }

            return players;
        }
    }
}
