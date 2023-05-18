using Ramsey.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Ramsey.Graph
{
    internal static class GraphSerializationAlgorithms
    {
        public static SerializedNode Serialize(Node node) 
        {
            return new() 
            {
                Position = node.Position
            };
        }
        public static SerializedEdge Serialize(Edge edge)
        {
            return new() 
            {
                Start = edge.Start.ID,
                End   = edge.End.ID,
                Type  = edge.Type
            };
        }
        public static SerializedGraph Serialize(IReadOnlyGraph graph)
        {
            return new() 
            {
                Nodes = graph.Nodes.Select(Serialize).ToArray(),
                Edges = graph.Edges.Select(Serialize).ToArray()
            };  
        }
        public static SerializedPath Serialize(Path path, int id)
        {
            return new()
            {
                Nodes = path.Nodes.Select(n => n.ID).ToArray(),
                End   = path.End.ID,
                Type  = path.Type,
                ID    = id
            };
        }
        public static SerializedPathFinder Serialize(IncrementalPathFinder finder) 
        {
            var ids = finder.AllPaths.AssignUniqueIDs();

            return new()
            {
                Paths = finder.AllPaths.Select(path => Serialize(path, ids[path])).ToArray(),

                NodesByTerminatingPaths = finder.NodesByTerminatingPaths
                    .Select(k => new SerializedPathFinderEntry {
                        Node = k.Key.ID, 
                        TerminatingPaths = k.Value.Select(p => ids[p]).ToArray()
                    }).ToArray(),
                
                MaxLengthPath = ids[finder.MaxLengthPath]
            };
        }
        public static SerializedGraphManager Serialize(GraphManager manager)
        {
            return new() 
            {
                Graph = Serialize(manager.graph),
                PathFinder = Serialize(manager.pathFinder)
            };
        }

        public static Path DeserializePath(SerializedPath path, Graph associatedGraph)
        {
            var nodes = path.Nodes.Select(i => associatedGraph.Nodes[i]).ToHashSet();
            var end = associatedGraph.Nodes[path.End];

            return new Path(nodes, end, path.Type);
        }
        public static Graph DeserializeGraph(SerializedGraph ser)
        {
            var graph = new Graph();

            foreach(var node in ser.Nodes)
            {
                graph.CreateNode(node.Position);
            }

            foreach(var edge in ser.Edges)
            {
                graph.CreateEdge(graph.Nodes[edge.Start], graph.Nodes[edge.End], edge.Type);
            }

            return graph;
        }
        public static IncrementalPathFinder DeserializePathFinder(SerializedPathFinder ser, Graph associatedGraph)
        {
            var paths = new List<Path>();
            foreach(var path in ser.Paths)
            {
                paths.Add(DeserializePath(path, associatedGraph));
            }

            var nbtp = ser.NodesByTerminatingPaths.ToDictionary(
                k => associatedGraph.Nodes[k.Node], 
                k => k.TerminatingPaths.Select(i => paths[i]).ToList()
            );
            var max = paths[ser.MaxLengthPath];
            
            return new IncrementalPathFinder(nbtp, max);
        }
    }
}