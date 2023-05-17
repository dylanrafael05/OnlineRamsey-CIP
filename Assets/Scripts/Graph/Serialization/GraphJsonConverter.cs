using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace Ramsey.Core
{
    internal class GraphJsonConverter : JsonConverter<Graph>
    {
        public const string NodeCount = "nodecount";
        public const string Edges = "edges";
        public const string Start = "start";
        public const string End = "end";
        public const string Type = "type";

        public override Graph ReadJson(JsonReader reader, Type objectType, Graph existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            existingValue = new Graph();

            var token = JToken.ReadFrom(reader);

            if(token is not JObject obj) 
                throw new JsonSerializationException("Graphs cannot be read from a non-object!");

            if (!obj.ContainsKey(NodeCount))
                throw new JsonSerializationException("Graph jsons must contain a " + NodeCount + " property!");
            
            var nodeCount = obj[NodeCount].ToObject<float>(serializer);

            var nodes = new List<Node>();
            for(var id = 0; id < nodeCount; id++)
            {
                var node = new Node(id);
                nodes.Add(node);

                existingValue.AddExistingNode(node);
            }

            if(!obj.ContainsKey(Edges))
                throw new JsonSerializationException("Graph jsons must contain a " + Edges + " property!");

            if(obj[Edges] is not JArray edges)
                throw new JsonSerializationException(Edges + " must be a list!");

            foreach(var edgeRaw in edges)
            {
                if(edgeRaw is not JObject edge)
                    throw new JsonSerializationException(Edges + "[...] must be an object!");

                if(!edge.ContainsKey(Start))
                    throw new JsonSerializationException("Edge jsons must contain a " + Start + " property!");
                if(!edge.ContainsKey(End))
                    throw new JsonSerializationException("Edge jsons must contain a " + End + " property!");
                if(!edge.ContainsKey(Type))
                    throw new JsonSerializationException("Edge jsons must contain a " + Type + " property!");

                var start = edge[Start].ToObject<int>(serializer); 
                var end = edge[End].ToObject<int>(serializer); 
                var type = edge[Type].ToObject<int>(serializer);

                if(start >= nodes.Count || start < 0)
                    throw new JsonSerializationException(Start + " must less within the interval [0, " + NodeCount + "]");
                if(end >= nodes.Count || end < 0)
                    throw new JsonSerializationException(End + " must less within the interval [0, " + NodeCount + "]");
                
                existingValue.CreateEdge(nodes[start], nodes[end], type);
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, Graph value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(NodeCount);
            writer.WriteValue(value.Nodes.Count);

            writer.WritePropertyName(Edges);
            writer.WriteStartArray();

            foreach(var edge in value.Edges)
            {
                writer.WriteStartObject();

                writer.WritePropertyName(Start);
                writer.WriteValue(edge.Start.ID);

                writer.WritePropertyName(End);
                writer.WriteValue(edge.End.ID);

                writer.WritePropertyName(Type);
                writer.WriteValue(edge.Type);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            
            writer.WriteEndObject();
        }
    }
}