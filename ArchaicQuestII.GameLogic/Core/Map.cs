using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;

namespace ArchaicQuestII.GameLogic.Core
{
    public class SigmaMapNode
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; } = 2;
        [JsonProperty("defaultLabelSize")]
        public int DefaultLabelSize { get; set; } = 12;
        [JsonProperty("color")]
        public string Color { get; set; } = "#ccc";
        [JsonProperty("type")]
        public Room.RoomType Type { get; set; } = Room.RoomType.Standard;
    }

    public class SigmaMapEdge
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("target")]
        public string Target { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; } = "#ccc";
        [JsonProperty("type")]
        public string Type { get; set; } = "line";
    }

    public class SigmaMapJSON
    {
        [JsonProperty("nodes")]
        public List<SigmaMapNode> Nodes { get; set; }
        [JsonProperty("edges")]
        public List<SigmaMapEdge> Edges { get; set; }

    }

    public static class Map
    {

        public static string DrawMap(List<Room> rooms)
        {

            var nodes = new List<SigmaMapNode>();
            var edges = new List<SigmaMapEdge>();

            var list = rooms;

            foreach (var node in list)
            {

                var y = 0;


                if (node.Coords.Y == 0)
                {
                    y = 0;
                }
                else if (node.Coords.Y > 0)
                {
                    y = (node.Coords.Y * -1);
                }
                else if (node.Coords.Y < 0)
                {
                    y = Math.Abs(node.Coords.Y);
                }

                var mapNode = new SigmaMapNode()
                {
                    Id = "node" + node.Id,
                    Label = node.Title,
                    X = node.Coords.X,
                    Y = y,

                };

                
                if (node.Type == Room.RoomType.Water || node.Type == Room.RoomType.River || node.Type == Room.RoomType.Sea)
                {
                    mapNode.Color = "#22A7F0";
                }
      
                nodes.Add(mapNode);

                if (node.Exits.NorthWest != null)
                {
                    var mapEdge = new SigmaMapEdge()
                    {
                        Id = "edge" + node.Id + node.Exits.NorthWest.RoomId,
                        Source = "node" + node.Id,
                        Target = "node" + node.Exits.NorthWest.RoomId,

                    };

                    if (edges.FirstOrDefault(x => x.Source == mapEdge.Source && x.Target == mapEdge.Target) == null)
                    {
                        edges.Add(mapEdge);
                    }
                }

                if (node.Exits.North != null)
                {
                    var mapEdge = new SigmaMapEdge()
                    {
                        Id = "edge" + node.Id + node.Exits.North.RoomId,
                        Source = "node" + node.Id,
                        Target = "node" + node.Exits.North.RoomId,

                    };

                    if (edges.FirstOrDefault(x => x.Source == mapEdge.Source && x.Target == mapEdge.Target) == null)
                    {
                        edges.Add(mapEdge);
                    }
                }

                if (node.Exits.NorthEast != null)
                {
                    var mapEdge = new SigmaMapEdge()
                    {
                        Id = "edge" + node.Id + node.Exits.NorthEast.RoomId,
                        Source = "node" + node.Id,
                        Target = "node" + node.Exits.NorthEast.RoomId,

                    };

                    if (edges.FirstOrDefault(x => x.Source == mapEdge.Source && x.Target == mapEdge.Target) == null)
                    {
                        edges.Add(mapEdge);
                    }
                }

                if (node.Exits.East != null)
                {
                    var mapEdge = new SigmaMapEdge()
                    {
                        Id = "edge" + node.Id + node.Exits.East.RoomId,
                        Source = "node" + node.Id,
                        Target = "node" + node.Exits.East.RoomId,

                    };

                     if (edges.FirstOrDefault(x => x.Source == mapEdge.Source && x.Target == mapEdge.Target) == null)
                    {
                        edges.Add(mapEdge);
                    }
                }

                if (node.Exits.South != null)
                {
                    var mapEdge = new SigmaMapEdge()
                    {
                        Id = "edge" + node.Id + node.Exits.South.RoomId,
                        Source = "node" + node.Id,
                        Target = "node" + node.Exits.South.RoomId,

                    };

                     if (edges.FirstOrDefault(x => x.Source == mapEdge.Source && x.Target == mapEdge.Target) == null)
                    {
                        edges.Add(mapEdge);
                    }
                }

                if (node.Exits.SouthEast != null)
                {
                    var mapEdge = new SigmaMapEdge()
                    {
                        Id = "edge" + node.Id + node.Exits.SouthEast.RoomId,
                        Source = "node" + node.Id,
                        Target = "node" + node.Exits.SouthEast.RoomId,

                    };

                    if (edges.FirstOrDefault(x => x.Source == mapEdge.Source && x.Target == mapEdge.Target) == null)
                    {
                        edges.Add(mapEdge);
                    }
                }

                if (node.Exits.SouthWest != null)
                {
                    var mapEdge = new SigmaMapEdge()
                    {
                        Id = "edge" + node.Id + node.Exits.SouthWest.RoomId,
                        Source = "node" + node.Id,
                        Target = "node" + node.Exits.SouthWest.RoomId,

                    };

                    if (edges.FirstOrDefault(x => x.Source == mapEdge.Source && x.Target == mapEdge.Target) == null)
                    {
                        edges.Add(mapEdge);
                    }
                }

                if (node.Exits.West != null)
                {
                    var mapEdge = new SigmaMapEdge()
                    {
                        Id = "edge" + node.Id + node.Exits.West.RoomId,
                        Source = "node" + node.Id,
                        Target = "node" + node.Exits.West.RoomId,

                    };

                     if (edges.FirstOrDefault(x => x.Source == mapEdge.Source && x.Target == mapEdge.Target) == null)
                    {
                        edges.Add(mapEdge);
                    }
                }

            }
 

            var json = new SigmaMapJSON()
            {
                Edges = edges,
                Nodes = nodes
            };

            var brokenEdges = new List<string>();

            foreach (var e in edges.ToList())
            {


                if (nodes.FirstOrDefault(x => x.Id == e.Target) == null)
                {
                    brokenEdges.Add(e.Id);
                    edges.Remove(e);
                }

            }

           
          return  JsonConvert.SerializeObject(json);
                
        }



    }
}
