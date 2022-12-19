using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class LookCmd : ICommand
    {
        public LookCmd(ICore core)
        {
            Aliases = new[] {"look"};
            Description = "Shows info about room or object.";
            Usages = new[] {"Type: look"};
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Incapacitated,
                CharacterStatus.Status.Sleeping
            };
            UserRole = UserRole.Player;
            Core = core;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }
        
        public void Execute(Player player, Room room, string[] input)
        {
            if (player.Affects.Blind)
            {
                Core.Writer.WriteLine("<p>You are blind and can't see a thing!</p>", player.ConnectionId);
                return;
            }
            
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
                        Core.Writer.WriteLine("<p>Look at what?</p>", player.ConnectionId);
                        return;
                    case "in" when string.IsNullOrEmpty(target):
                        Core.Writer.WriteLine("<p>Look in what?</p>", player.ConnectionId);
                        return;
                }
            }

            switch (verb)
            {
                case "at":
                    LookAtObject(player, room, target);
                    return;
                case "in":
                    LookInContainer(player, room, target);
                    return;
            }
        }
        
        private void Look(Player player, Room room)
        {
            var showVerboseExits = player.Config.VerboseExits;
            var exits = Core.RoomActions.FindValidExits(room, showVerboseExits);

            var items = DisplayItems(room, player);
            var mobs = DisplayMobs(room, player);
            var players = DisplayPlayers(room, player);
            var isDark = Core.RoomActions.RoomIsDark(room, player);
            var roomDesc = new StringBuilder();

            roomDesc.Append(
                $"<p class=\"room-title {(isDark ? "room-dark" : "")}\">{room.Title} ({room.Coords.X},{room.Coords.Y},{room.Coords.Z})<br /></p>");

            // With brief toggled we don't show the room description
            if (!player.Config.Brief)
            {
                roomDesc.Append($"<p class=\"room-description  {(isDark ? "room-dark" : "")}\">{room.Description}</p>");
            }

            roomDesc.Append(
                !showVerboseExits
                    ? roomDesc.Append(
                        $"<p class=\"room-exit  {(isDark ? "room-dark" : "")}\"> <span class=\"room-exits\">[</span>Exits: <span class=\"room-exits\">{exits}</span><span class=\"room-exits\">]</span></p>")
                    : roomDesc.Append(
                        $"<div class=\" {(isDark ? "room-dark" : "")}\">Obvious exits: <table class=\"room-exits\"><tbody>{exits}</tbody></table></div>"));

            roomDesc.Append($"<p  class=\" {(isDark ? "room-dark" : "")}\">{items}</p>")
                .Append($"<p  class=\"{(isDark ? "room-dark" : "")}\">{mobs}</p>")
                .Append($"<p  class=\"  {(isDark ? "room-dark" : "")}\">{players}</p>");


            Core.Writer.WriteLine(roomDesc.ToString(), player.ConnectionId);
        }

        private void LookInContainer(Player player, Room room, string target)
        {
            var nthTarget = Helpers.findNth(target);
            var container = Helpers.findRoomObject(nthTarget, room) ?? Helpers.findObjectInInventory(nthTarget, player);

            if (container != null && container.ItemType != Item.Item.ItemTypes.Container &&
                container.ItemType != Item.Item.ItemTypes.Cooking)
            {
                if (container.ItemType == Item.Item.ItemTypes.Portal)
                {
                    LookInPortal(player, room, container);
                    return;
                }

                Core.Writer.WriteLine($"<p>{container.Name} is not a container", player.ConnectionId);
                return;
            }

            if (container == null)
            {
                Core.Writer.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            if (container.Container.IsOpen == false)
            {
                Core.Writer.WriteLine("<p>You need to open it first.", player.ConnectionId);
                return;
            }

            Core.Writer.WriteLine($"<p>{container.Name} contains:</p>", player.ConnectionId);
            
            if (container.Container.Items.Count == 0)
            {
                Core.Writer.WriteLine("<p>Nothing.</p>", player.ConnectionId);
            }

            var isDark = Core.RoomActions.RoomIsDark(room, player);
            
            foreach (var obj in container.Container.Items.List(false))
            {
                Core.Writer.WriteLine($"<span class='item {(isDark ? "room-dark" : "")}'>{obj.Name}</span>",
                    player.ConnectionId);
            }
            
            foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
            {
                Core.Writer.WriteLine($"<p>{player.Name} looks inside {container.Name.ToLower()}.</p>",
                    pc.ConnectionId);
            }
        }

        private void LookInPortal(Player player, Room room, Item.Item target)
        {
            var getPortalLocation = Core.Cache.GetRoom(target.Portal.Destination);

            if (getPortalLocation == null)
            {
                Core.ErrorLog.Write("LookCmd.cs", $"Portal ({target.Portal.Name}) location empty.", ErrorLog.Priority.Medium);
                Core.Writer.WriteLine("<p>The dark abyss, I wouldn't enter if I were you.</p>", player.ConnectionId);
                return;
            }

            Look(player, getPortalLocation);
        }

        private void LookAtObject(Player player, Room room, string target)
        {
            var isDark = Core.RoomActions.RoomIsDark(room, player);
            var nthTarget = Helpers.findNth(target);

            var item = Helpers.findRoomObject(nthTarget, room) ?? Helpers.findObjectInInventory(nthTarget, player);
            var character = Helpers.FindMob(nthTarget, room) ?? Helpers.FindPlayer(nthTarget, room);
            
            RoomObject roomObjects = null;
            if (room.RoomObjects.Count >= 1 && room.RoomObjects[0].Name != null)
            {
                roomObjects =
                    room.RoomObjects.FirstOrDefault(x =>
                        x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));
            }

            if (target.Equals("self", StringComparison.CurrentCultureIgnoreCase))
            {
                character = player;
            }


            if (item == null && character == null && roomObjects == null)
            {
                Core.Writer.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            if (item != null)
            {
                Core.Writer.WriteLine($"<p  class='{(isDark ? "room-dark" : "")}'>{item.Description.Look}",
                    player.ConnectionId);

                // display item stats via lore
                var hasLore = Helpers.FindSkill("lore", player);

                if (hasLore != null)
                {
                    var success = Helpers.LoreSuccess(hasLore.Proficiency ?? 0);

                    if (success)
                    {
                        Core.PassiveSkills.Lore(player, room, item.Name);
                    }
                    else
                    {
                        Core.Gain.GainSkillExperience(player, hasLore.Level * 100, hasLore, Core.Dice.Roll(1, 1, 5));
                    }
                }

                if (item.Container is { CanOpen: false } && item.Container.Items.Any())
                {
                    Core.Writer.WriteLine($"<p  class='{(isDark ? "room-dark" : "")}'>{item.Name} contains:",
                        player.ConnectionId);

                    var listOfContainerItems = new StringBuilder();
                    foreach (var containerItem in item.Container.Items.List())
                    {
                        listOfContainerItems.Append(
                            $"<p class='{(isDark ? "room-dark" : "")} container-item'>{containerItem.Name.Replace(" lies here.", "")}</p>");
                    }

                    Core.Writer.WriteLine(listOfContainerItems.ToString(), player.ConnectionId);

                }

                foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
                {
                    Core.Writer.WriteLine($"<p>{player.Name} looks at {item.Name.ToLower()}.</p>", pc.ConnectionId);
                }

                return;
            }

            //for player?
            if (roomObjects != null)
            {
                Core.Writer.WriteLine($"<p class='{(isDark ? "room-dark" : "")}'>{roomObjects.Look}",
                    player.ConnectionId);

                foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
                {
                    Core.Writer.WriteLine($"<p>{player.Name} looks at {roomObjects.Name.ToLower()}.</p>",
                        pc.ConnectionId);
                }

                return;
            }

            if (character == null)
            {
                Core.Writer.WriteLine("<p>You don't see them here.", player.ConnectionId);
                return;
            }

            var sb = new StringBuilder();
            
            if (character.ConnectionId != "mob")
            {
                sb.Append(
                    $"<table class='char-look'><tr><td><span class='cell-title'>Eyes:</span> {character.Eyes}</td><td><span class='cell-title'>Hair:</span> {character.HairColour}</td></tr>");
                sb.Append(
                    $"<tr><td><span class='cell-title'>Skin:</span> {character.Skin}</td><td><span class='cell-title'>Hair Length:</span> {character.HairLength}</td></tr>");
                sb.Append(
                    $"<tr><td><span class='cell-title'>Build:</span> {character.Build}</td><td><span class='cell-title'>Hair Texture:</span> {character.HairTexture}</td></tr>");
                sb.Append(
                    $"<tr><td><span class='cell-title'>Face:</span> {character.Face}</td><td><span class='cell-title'>Hair Facial:</span> {character.FacialHair}</td></tr><table>");
            }

            var status = Enum.GetName(typeof(CharacterStatus.Status), player.Status);

            var statusText = string.Empty;

            if (status.Equals("fighting", StringComparison.CurrentCultureIgnoreCase))
            {
                statusText = $"fighting {(character.Target.Equals(player.Name) ? "YOU!" : character.Target)}.";
            }
            else
            {
                statusText = $"{Enum.GetName(typeof(CharacterStatus.Status), player.Status)}.";
                statusText = statusText.ToLower();
            }

            var displayEquipment = new StringBuilder();
            displayEquipment.Append("<p>They are using:</p>")
                .Append("<table>");

            if (character.Equipped.Light != null)
            {
                displayEquipment.Append("<tr><td style='width:175px;' class=\"cell-title\" title='Worn as light'>")
                    .Append("&lt;used as light&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Light?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Finger != null)
            {
                displayEquipment.Append("<tr><td class=\"cell-title\" title='Worn on finger'>")
                    .Append(" &lt;worn on finger&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Finger?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Finger2 != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Worn on finger'>")
                    .Append(" &lt;worn on finger&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Finger2?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Neck != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Worn around neck'>")
                    .Append(" &lt;worn around neck&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Neck?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Neck2 != null)
            {
                displayEquipment.Append("<tr><td class=\"cell-title\" title='Worn around neck'>")
                    .Append(" &lt;worn around neck&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Neck2?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Face != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Worn on face'>")
                    .Append(" &lt;worn on face&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Face?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Head != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Worn on head'>")
                    .Append(" &lt;worn on head&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Head?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Torso != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Worn on torso'>")
                    .Append(" &lt;worn on torso&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Torso?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Legs != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Worn on legs'>")
                    .Append(" &lt;worn on legs&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Legs?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Feet != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Worn on feet'>")
                    .Append(" &lt;worn on feet&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Feet?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Hands != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Worn on hands'>")
                    .Append(" &lt;worn on hands&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Hands?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Arms != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Worn on arms'>")
                    .Append(" &lt;worn on arms&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Arms?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.AboutBody != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Worn about body'>")
                    .Append(" &lt;worn about body&gt;").Append("</td>").Append("<td>")
                    .Append(Helpers.DisplayEQNameWithFlags(character.Equipped.AboutBody) ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Waist != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Worn on waist'>")
                    .Append(" &lt;worn about waist&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Waist?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Wrist != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Worn on wrist'>")
                    .Append(" &lt;worn around wrist&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Wrist?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Wrist2 != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Worn on wrist'>")
                    .Append(" &lt;worn around wrist&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Wrist2?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Wielded != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='worn as weapon'>")
                    .Append(" &lt;wielded&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Wielded?.Name ?? "(nothing)").Append("</td></tr>");
            }
            
            if (character.Equipped.Secondary != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='worn as weapon'>")
                    .Append(" &lt;secondary&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Secondary?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Shield != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Worn as shield'>")
                    .Append(" &lt;worn as shield&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Shield?.Name ?? "(nothing)").Append("</td></tr>");
            }

            if (character.Equipped.Held != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Held'>").Append(" &lt;Held&gt;")
                    .Append("</td>").Append("<td>").Append(character.Equipped.Held?.Name ?? "(nothing)")
                    .Append("</td></tr>");
            }

            if (character.Equipped.Floating != null)
            {
                displayEquipment.Append("<tr><td  class=\"cell-title\" title='Floating Nearby'>")
                    .Append(" &lt;Floating nearby&gt;").Append("</td>").Append("<td>")
                    .Append(character.Equipped.Floating?.Name ?? "(nothing)").Append("</td></tr>").Append("</table");
            }

            if (character.Equipped.Light == null && character.Equipped.Finger == null &&
                character.Equipped.Finger2 == null && character.Equipped.Neck == null &&
                character.Equipped.Neck2 == null &&
                character.Equipped.Face == null && character.Equipped.Head == null &&
                character.Equipped.Torso == null && character.Equipped.Legs == null &&
                character.Equipped.Feet == null &&
                character.Equipped.Hands == null && character.Equipped.Arms == null &&
                character.Equipped.AboutBody == null && character.Equipped.Waist == null &&
                character.Equipped.Wrist == null &&
                character.Equipped.Wrist2 == null && character.Equipped.Wielded == null &&
                character.Equipped.Secondary == null && character.Equipped.Shield == null &&
                character.Equipped.Held == null &&
                character.Equipped.Held == null && character.Equipped.Floating == null)
            {

                displayEquipment.Append("</table").Append("<p>Nothing.</p>");
            }

            Core.Writer.WriteLine(
                $"{sb}<p class='{(isDark ? "room-dark" : "")}'>{character.Description} <br/>{character.Name} {Core.Formulas.TargetHealth(player, character)} and is {statusText}<br/> {displayEquipment}",
                player.ConnectionId);
            
            foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
            {
                Core.Writer.WriteLine($"<p>{player.Name} looks at {character.Name.ToLower()}.</p>", pc.ConnectionId);
            }
        }

        private string DisplayItems(Room room, Player player)
        {
            var isDark = Core.RoomActions.RoomIsDark(room, player);
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

                    var clickEvent = $"window.dispatchEvent(new CustomEvent(\"open-detail\", {data}))";
                    x += $"<p onClick='{clickEvent}' class='item {(isDark ? "dark-room" : "")}' >{item.Name}</p>";
                }
            }

            return x;
        }

        private string DisplayMobs(Room room, Player player)
        {
            var mobs = string.Empty;
            var mobName = string.Empty;
            var isDark = Core.RoomActions.RoomIsDark(room, player);
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
                            $"<p class='mob {(isDark ? "dark-room" : "")}'>{mob.Name} is here{(isFightingPC ? " fighting YOU!" : "")}</p>";
                    }
                    else
                    {
                        mobs +=
                            $"<p class='mob {(isDark ? "dark-room" : "")}'>{mobName}</p>";
                    }
                }
                else
                {
                    mobs +=
                        $"<p class='mob {(isDark ? "dark-room" : "")}'>{mobName} is here{(isFightingPC ? " fighting YOU!" : ".")}.</p>";
                }
            }

            return mobs;
        }

        private string DisplayPlayers(Room room, Player player)
        {
            var players = string.Empty;
            var isNightTime = Core.RoomActions.RoomIsDark(room, player);
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
                players += $"<p class='player {(isNightTime ? "dark-room" : "")}'>{pcName}.</p>";
            }

            return players;
        }
    }
}
