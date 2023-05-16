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

        public static string SaveToString(GraphManager graph)
        {
            return JsonConvert.SerializeObject(graph, settings);
        }

        public static GraphManager LoadFromString(string contents)
        {
            return JsonConvert.DeserializeObject<GraphManager>(contents, settings);
        }
    }
}