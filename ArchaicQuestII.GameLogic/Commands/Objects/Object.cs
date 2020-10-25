﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.Commands.Objects
{

   public class Object: IObject
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _updateUi;
        private readonly IMobScripts _mobScripts;
        public Object(IWriteToClient writer, IUpdateClientUI updateUi, IMobScripts mobScripts)
        {
            _writer = writer;
            _updateUi = updateUi;
            _mobScripts = mobScripts;
        }
        public void Get(string target, string container, Room room, Player player)
        {
            //TODO: Get all, get nth (get 2.apple)
            if (target == "all" && string.IsNullOrEmpty(container))
            {
                GetAll(room, player);
                return;
            }

            if (!string.IsNullOrEmpty(container))
            {
                GetFromContainer(target, container, room, player);
                return;
            }

            //Check room first
            var item = room.Items.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
               _writer.WriteLine("<p>You don't see that here.</p>", player.ConnectionId);
               return;
            }

            if (item.Stuck)
            {
                _writer.WriteLine("<p>You can't pick that up.</p>", player.ConnectionId);
                return;
            }

            room.Items.Remove(item);

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                _writer.WriteLine($"<p>{player.Name} picks up {item.Name.ToLower()}.</p>", pc.ConnectionId);
            }

            item.IsHiddenInRoom = false;
            player.Inventory.Add(item);
            _writer.WriteLine($"<p>You pick up {item.Name.ToLower()}.</p>", player.ConnectionId);
            _updateUi.UpdateInventory(player);
            room.Clean = false;
            // TODO: You are over encumbered 

        }

 

        public void GetAll(Room room, Player player)
        {
           
            //Check room first

            if (room.Items.Count == 0)
            {
                _writer.WriteLine("<p>You don't see anything here.</p>", player.ConnectionId);
                return;
            }

            for (var i = room.Items.Count - 1; i >= 0; i--)
            {
                if (room.Items[i].Stuck == false)
                {
                    room.Items[i].IsHiddenInRoom = false;
                    player.Inventory.Add(room.Items[i]);
                    _writer.WriteLine($"<p>You pick up {room.Items[i].Name.ToLower()}.</p>", player.ConnectionId);

                    foreach (var pc in room.Players)
                    {
                        if (pc.Name == player.Name)
                        {
                            continue;
                        }

                        _writer.WriteLine($"<p>{player.Name} picks up {room.Items[i].Name.ToLower()}.</p>",
                            pc.ConnectionId);
                    }

                    room.Items.RemoveAt(i);

                }
                else
                {
                    _writer.WriteLine("<p>You can't get that.</p>", player.ConnectionId);
                }
            }
            room.Clean = false;
            _updateUi.UpdateInventory(player);
            // TODO: You are over encumbered 

        }

        public void GetAllContainer(Item.Item container, Room room, Player player)
        {

            //Check room first

            if (container.Container.Items.Count == 0)
            {
                _writer.WriteLine("<p>You don't see anything here.</p>", player.ConnectionId);
                return;
            }

            for (var i = container.Container.Items.Count - 1; i >= 0; i--)
            {
                container.Container.Items[i].IsHiddenInRoom = false;
                player.Inventory.Add(container.Container.Items[i]);

                    _writer.WriteLine($"<p>You pick up {container.Container.Items[i].Name.ToLower()} from {container.Name.ToLower()}.</p>", player.ConnectionId);

                    foreach (var pc in room.Players)
                    {
                        if (pc.Name == player.Name)
                        {
                            continue;
                        }

                        _writer.WriteLine($"<p>{player.Name} picks up {container.Container.Items.Name.ToLower()} from {container.Name.ToLower()}.</p>",
                            pc.ConnectionId);
                    }
                    container.Container.Items.RemoveAt(i);

            }
            _updateUi.UpdateInventory(player);
            room.Clean = false;
            // TODO: You are over encumbered 

        }

        public void Drop(string target, string container, Room room, Player player)
        {

            if (target == "all" && string.IsNullOrEmpty(container))
            {
                DropAll(room, player);
                return;
            }

            if (!string.IsNullOrEmpty(container))
            {
                DropInContainer(target, container, room, player);
                return;
            }

            var item = player.Inventory.Where(x => x.Stuck == false).FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                _writer.WriteLine("<p>You don't have that item.</p>", player.ConnectionId);
                return;
            }

            player.Inventory.Remove(item);

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                _writer.WriteLine($"<p>{player.Name} drops {item.Name.ToLower()}.</p>",
                    pc.ConnectionId);
            }

            room.Items.Add(item);

            _writer.WriteLine($"<p>You drop {item.Name.ToLower()}.</p>", player.ConnectionId);
            _updateUi.UpdateInventory(player);
        }

        public void DropInContainer(string target, string container, Room room, Player player)
        {

            var containerObj = room.Items.FirstOrDefault(x => x.Name.Contains(container, StringComparison.CurrentCultureIgnoreCase))  ?? player.Inventory.Where(x => x.Stuck == false).FirstOrDefault(x => x.Name.Contains(container, StringComparison.CurrentCultureIgnoreCase));

            if (containerObj == null)
            {
                _writer.WriteLine($"<p>You don't see that here.</p>", player.ConnectionId);
                return;
            }

            if (!containerObj.Container.IsOpen)
            {
                _writer.WriteLine($"<p>You need to open it first.</p>", player.ConnectionId);
                return;
            }

            if (target == "all")
            {
                DropAllContainer(containerObj, room, player);
                return;
            }

            var item = player.Inventory.Where(x => x.Stuck == false).FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                _writer.WriteLine("<p>You don't have that item.</p>", player.ConnectionId);
                return;
            }

            player.Inventory.Remove(item);

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                _writer.WriteLine($"<p>{player.Name} puts {item.Name.ToLower()} into {containerObj.Name.ToLower()}.</p>",
                    pc.ConnectionId);
            }

            containerObj.Container.Items.Add(item);

            _writer.WriteLine($"<p>You put {item.Name.ToLower()} into {containerObj.Name.ToLower()}.</p>", player.ConnectionId);
            _updateUi.UpdateInventory(player);
        }

        public void DropAllContainer(Item.Item container, Room room, Player player)
        {


            if (player.Inventory.Count == 0)
            {
                _writer.WriteLine("<p>You don't see anything here.</p>", player.ConnectionId);
                return;
            }

            for (var i = player.Inventory.Count - 1; i >= 0; i--)
            {

                container.Container.Items.Add(player.Inventory[i]);

                _writer.WriteLine($"<p>You place {player.Inventory[i].Name.ToLower()} into {container.Name.ToLower()}.</p>", player.ConnectionId);

                foreach (var pc in room.Players)
                {
                    if (pc.Name == player.Name)
                    {
                        continue;
                    }

                    _writer.WriteLine($"<p>{player.Name} puts {player.Inventory.Name.ToLower()} into {container.Name.ToLower()}.</p>",
                        pc.ConnectionId);
                }
                player.Inventory.RemoveAt(i);

            }
            _updateUi.UpdateInventory(player);

            // TODO: You are over encumbered 

        }


        public void GetFromContainer(string target, string container, Room room, Player player)
        {

          

            var containerObj = room.Items.FirstOrDefault(x => x.Name.Contains(container, StringComparison.CurrentCultureIgnoreCase)) ?? player.Inventory.Where(x => x.Stuck == false).FirstOrDefault(x => x.Name.Contains(container, StringComparison.CurrentCultureIgnoreCase));

            if (containerObj == null)
            {
                _writer.WriteLine($"<p>You don't see that here.</p>", player.ConnectionId);
                return;
            } 


            if (containerObj.Container.CanOpen && !containerObj.Container.IsOpen)
            {
                _writer.WriteLine($"<p>You need to open it first.</p>", player.ConnectionId);
                return;
            }

            if (target == "all")
            {
                GetAllContainer(containerObj, room, player);
                return;
            }

            var item = containerObj.Container.Items.Where(x => x.Stuck == false).FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                _writer.WriteLine("<p>You don't have that item.</p>", player.ConnectionId);
                return;
            }

            containerObj.Container.Items.Remove(item);
           

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                _writer.WriteLine($"<p>{player.Name} gets {item.Name.ToLower()} from {containerObj.Name.ToLower()}.</p>",
                    pc.ConnectionId);
            }

            item.IsHiddenInRoom = false;
            player.Inventory.Add(item);

            _writer.WriteLine($"<p>You get {item.Name.ToLower()} from {containerObj.Name.ToLower()}.</p>", player.ConnectionId);
            _updateUi.UpdateInventory(player);
            room.Clean = false;
        }

        public void DropAll(Room room, Player player)
        {

            //Check room first

            if (player.Inventory.Count == 0)
            {
                _writer.WriteLine("<p>You don't have anything to drop.</p>", player.ConnectionId);
                return;
            }

         

            for (var i = player.Inventory.Count - 1; i >= 0; i--)
            {
                if (player.Inventory[i].Stuck == false)
                {

                    room.Items.Add(player.Inventory[i]);

                   
                    _writer.WriteLine($"<p>You drop {player.Inventory[i].Name.ToLower()}.</p>", player.ConnectionId);

                    foreach (var pc in room.Players)
                    {
                        if (pc.Name == player.Name)
                        {
                            continue;
                        }

                        _writer.WriteLine($"<p>{player.Name} drops {player.Inventory[i].Name.ToLower()}.</p>",
                            pc.ConnectionId);
                    }
                    player.Inventory.RemoveAt(i);

                }
            }
            _updateUi.UpdateInventory(player);
            // TODO: You are over encumbered 

        }

        public void Open(string target, Room room, Player player)
        {
            var item = room.Items.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ?? player.Inventory.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item != null && item.Container.CanOpen != true)
            {
                _writer.WriteLine($"<p>{item.Name} cannot be opened", player.ConnectionId);
                return;
            }

            if (item == null)
            {
                _writer.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            if (item.Container.IsOpen)
            {
                _writer.WriteLine("<p>It's already open.", player.ConnectionId);
                return;
            }

            _writer.WriteLine($"<p>You open {item.Name.ToLower()}.</p>", player.ConnectionId);

            item.Container.IsOpen = true;
            foreach (var obj in room.Players)
            {
                if (obj.Name == player.Name)
                {
                    continue;
                }
                _writer.WriteLine($"<p>{player.Name} opens {item.Name.ToLower()}</p>", obj.ConnectionId);
            }
            room.Clean = false;
        }

        public void Close(string target, Room room, Player player)
        {

            var item = room.Items.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ?? player.Inventory.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

                if (item != null && item.Container.CanOpen != true)
                {
                    _writer.WriteLine($"<p>{item.Name} cannot be closed", player.ConnectionId);
                    return;
                }

                if (item == null)
                {
                    _writer.WriteLine("<p>You don't see that here.", player.ConnectionId);
                    return;
                }


                if (!item.Container.IsOpen)
                {
                    _writer.WriteLine("<p>It's already closed.", player.ConnectionId);
                    return;
                }

            _writer.WriteLine($"<p>You close {item.Name.ToLower()}.</p>", player.ConnectionId);

                item.Container.IsOpen = false;

                
                foreach (var obj in room.Players)
                {
                    if (obj.Name == player.Name)
                    {
                        continue;
                    }
                    _writer.WriteLine($"<p>{player.Name} closes {item.Name.ToLower()}</p>", obj.ConnectionId);
                }

                room.Clean = false;
        }

        public void Give(string itemName, string targetName, Room room, Player player)
        {

            if (string.IsNullOrEmpty(itemName))
            {
                _writer.WriteLine("<p>Give what to whom?</p>", player.ConnectionId);
                return;
            }

            if (string.IsNullOrEmpty(targetName))
            {
                _writer.WriteLine("<p>Give to whom?</p>", player.ConnectionId);
                return;
            }

            var target =
                room.Players.FirstOrDefault(x =>
                    x.Name.Contains(targetName, StringComparison.CurrentCultureIgnoreCase)) ??
                room.Mobs.FirstOrDefault(x => x.Name.Contains(targetName, StringComparison.CurrentCultureIgnoreCase));

            if (target == null)
            {
                _writer.WriteLine("<p>They aren't here.</p>", player.ConnectionId);
                return;
            }
           
    
            //Check room first
            var item = player.Inventory.FirstOrDefault(x => x.Name.Contains(itemName, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                _writer.WriteLine("<p>You do not have that item.</p>", player.ConnectionId);
                return;
            }

            player.Inventory.Remove(item);

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    _writer.WriteLine($"<p>You give {item.Name.ToLower()} to {target.Name.ToLower()}.</p>", pc.ConnectionId);
                    continue;
                }

                if (pc.Name == target.Name)
                {
                    continue;
                }

                _writer.WriteLine($"<p>{player.Name} gives {item.Name.ToLower()} to {target.Name.ToLower()}.</p>", pc.ConnectionId);
            }

            target.Inventory.Add(item);
            _writer.WriteLine($"<p>{player.Name} gives you {item.Name.ToLower()}.</p>", target .ConnectionId);
            _updateUi.UpdateInventory(player);

            if (!string.IsNullOrEmpty(target.Events.Give))
            {
                UserData.RegisterType<MobScripts>();

                Script script = new Script();

                DynValue obj = UserData.Create(_mobScripts);
                script.Globals.Set("obj", obj);
                UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
                UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(player));


                script.Globals["room"] = room;

                script.Globals["player"] = player;
                script.Globals["mob"] = target;

                DynValue res = script.DoString(target.Events.Give);
            }


            // TODO: You are over encumbered 

        }

        public void Delete(string target, Room room, Player player)
        {
            throw new NotImplementedException();
        }

    
    }
}
