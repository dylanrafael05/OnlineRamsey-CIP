using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Ramsey.Core
{
    internal struct SerializedPath
    {
        public int[] Nodes { get; set; }
        public int LastNode { get; set; }
        public int Type { get; set; }
        public int ID { get; set; }
    }
    internal struct SerializedEdge
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Type { get; set; }
    }
    internal struct SerializedGraph
    {
        public int NodeCount { get; set; }
        public float2[] Positions { get; set; }
        public SerializedEdge[] Edges { get; set; }
    }
    internal struct SerializedPathFinder
    {
        public SerializedPath[] Paths { get; set; }
        public Dictionary<int, int[]> NodesByTerminatingPaths { get; set; }
        public int MaxLengthPath { get; set; }
    }
    internal struct SerializedGraphManager
    {
        public SerializedGraph Graph { get; set; }
        public SerializedPathFinder PathFinder { get; set; }
    }

    public static class GraphSerialization
    {
        private static SerializedGraph Serialize(Graph graph)
        {
            SerializedGraph output = default;
        }


        private static JsonSerializerSettings settings = new() 
        {
            Converters = new[] {new GraphJsonConverter()}
        };

        public static string SaveToString(Graph graph)
        {
            return JsonConvert.SerializeObject(graph, settings);
        }

        public static Graph LoadFromString(string contents)
        {
            return JsonConvert.DeserializeObject<Graph>(contents, settings);
        }
    }
}