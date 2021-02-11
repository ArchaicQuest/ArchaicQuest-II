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
        [JsonProperty("z")]
        public int Z{ get; set; }
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
                    Id = $"node{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}",
                    Label = node.Title,
                    X = node.Coords.X,
                    Y = y,
                    Z = node.Coords.Z

                };

                
                if (node.Type == Room.RoomType.Water || node.Type == Room.RoomType.River || node.Type == Room.RoomType.Sea)
                {
                    mapNode.Color = "#22A7F0";
                }

                if (node.Type == Room.RoomType.Field)
                {
                    mapNode.Color = "#20bf6b";
                }

                nodes.Add(mapNode);

                if (node.Exits.NorthWest != null)
                {
                    var mapEdge = new SigmaMapEdge()
                    {
                        Id = $"edge{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}{node.Exits.NorthWest.Coords.X}{node.Exits.NorthWest.Coords.Y}{node.Exits.NorthWest.Coords.Z}",
                        Source = $"node{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}",
                        Target = $"node{node.Exits.NorthWest.AreaId}{node.Exits.NorthWest.Coords.X}{node.Exits.NorthWest.Coords.Y}{node.Exits.NorthWest.Coords.Z}",

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
                        Id = $"edge{node.AreaId}{node.Exits.North.Coords.X}{node.Exits.North.Coords.Y}{node.Exits.North.Coords.Z}",
                        Source = $"node{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}",
                        Target = $"node{node.AreaId}{node.Exits.North.Coords.X}{node.Exits.North.Coords.Y}{node.Exits.North.Coords.Z}",

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
                        Id = $"edge{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}{node.Exits.NorthEast.Coords.X}{node.Exits.NorthEast.Coords.Y}{node.Exits.NorthEast.Coords.Z}",
                        Source = $"node{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}",
                        Target = $"node{node.Exits.NorthEast.AreaId}{node.Exits.NorthEast.Coords.X}{node.Exits.NorthEast.Coords.Y}{node.Exits.NorthEast.Coords.Z}",

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
                        Id = $"edge{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}{node.Exits.East.Coords.X}{node.Exits.East.Coords.Y}{node.Exits.East.Coords.Z}",
                        Source = $"node{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}",
                        Target = $"node{node.Exits.East.AreaId}{node.Exits.East.Coords.X}{node.Exits.East.Coords.Y}{node.Exits.East.Coords.Z}",

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
                        Id = $"edge{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}{node.Exits.South.Coords.X}{node.Exits.South.Coords.Y}{node.Exits.South.Coords.Z}",
                        Source = $"node{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}",
                        Target = $"node{node.Exits.South.AreaId}{node.Exits.South.Coords.X}{node.Exits.South.Coords.Y}{node.Exits.South.Coords.Z}",

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
                        Id = $"edge{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}{node.Exits.SouthEast.Coords.X}{node.Exits.SouthEast.Coords.Y}{node.Exits.SouthEast.Coords.Z}",
                        Source = $"node{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}",
                        Target = $"node{node.Exits.SouthEast.AreaId}{node.Exits.SouthEast.Coords.X}{node.Exits.SouthEast.Coords.Y}{node.Exits.SouthEast.Coords.Z}",

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
                        Id = $"edge{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}{node.Exits.SouthWest.Coords.X}{node.Exits.SouthWest.Coords.Y}{node.Exits.SouthWest.Coords.Z}",
                        Source = $"node{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}",
                        Target = $"node{node.Exits.SouthWest.AreaId}{node.Exits.SouthWest.Coords.X}{node.Exits.SouthWest.Coords.Y}{node.Exits.SouthWest.Coords.Z}",

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
                        Id = $"edge{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}{node.Exits.West.Coords.X}{node.Exits.West.Coords.Y}{node.Exits.West.Coords.Z}",
                        Source = $"node{node.AreaId}{node.Coords.X}{node.Coords.Y}{node.Coords.Z}",
                        Target = $"node{node.Exits.West.AreaId}{node.Exits.West.Coords.X}{node.Exits.West.Coords.Y}{node.Exits.West.Coords.Z}",

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

            foreach (var e in edges.Distinct().ToList())
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
