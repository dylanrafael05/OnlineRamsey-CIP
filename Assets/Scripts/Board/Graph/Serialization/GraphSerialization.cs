// using System.IO;
// using Unity.Plastic.Newtonsoft.Json;
// using UnityEngine;

// namespace Ramsey.Graph
// {
//     public static class GraphSerialization 
//     {
//         public static string SaveGraphToString(Graph graph)
//         {
//             return JsonConvert.SerializeObject(GraphSerializationAlgorithms.Serialize(graph));
//         }
//         public static Graph LoadGraphFromString(string json)
//         {
//             var ser = JsonConvert.DeserializeObject<SerializedGraph>(json);
//             return GraphSerializationAlgorithms.DeserializeGraph(ser);
//         }

//         // public static string SavePathFinderToString(IncrementalPathFinder pathFinder)
//         // {
//         //     return JsonConvert.SerializeObject(GraphSerializationAlgorithms.Serialize(pathFinder));
//         // }
//         // public static IncrementalPathFinder LoadGraphFinderFromString(string json, Graph associatedGraph)
//         // {
//         //     var ser = JsonConvert.DeserializeObject<SerializedPathFinder>(json);

//         //     return GraphSerializationAlgorithms.DeserializePathFinder(ser, associatedGraph);
//         // }

//         // public static string SaveManagerToString(GraphManager manager)
//         // {
//         //     return JsonConvert.SerializeObject(GraphSerializationAlgorithms.Serialize(manager));
//         // }

//         // public static GraphManager LoadManagerFromString(string json)
//         // {
//         //     var ser = JsonConvert.DeserializeObject<SerializedGraphManager>(json);

//         //     var graph = GraphSerializationAlgorithms.DeserializeGraph(ser.Graph);
//         //     var ipf = GraphSerializationAlgorithms.DeserializePathFinder(ser.PathFinder, graph);

//         //     return new GraphManager(graph, ipf);
//         // }
//     }
// }