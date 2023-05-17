using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Ramsey.Core
{
    public static class GraphSerialization
    {
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