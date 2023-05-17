using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

namespace Ramsey.Core
{
    
    public class GraphManager : IGraph
    {
        public IReadOnlyGraph Graph => graph;
        public GameState State => gameState;

        public IReadOnlyList<Node> Nodes => graph.Nodes;
        public IReadOnlyList<Edge> Edges => graph.Edges;

        internal readonly Graph graph;
        internal readonly IncrementalPathFinder pathFinder;
        private readonly GameState gameState;

        internal GraphManager(Graph graph, IncrementalPathFinder pathFinder)
        {
            this.graph = graph;
            this.pathFinder = pathFinder;

            gameState = new()
            {
                Graph = graph,
                MaxLengthPath = pathFinder.MaxLengthPath
            };
        }

        public GraphManager() : this(new(), new())
        {}

        public Node CreateNode(float2 position = default)
        {
            var n = graph.CreateNode(position);
            pathFinder.HandleNodeAddition(n);

            return n;
        }

        public Edge CreateEdge(Node start, Node end, int type = Edge.NullType)
        {
            var e = graph.CreateEdge(start, end, type);

            if(e.Type != Edge.NullType)
            {
                pathFinder.HandlePaintedEdge(e);
                gameState.MaxLengthPath = pathFinder.MaxLengthPath;
            }

            return e;
        }

        public void PaintEdge(Edge e, int type)
        {
            graph.PaintEdge(e, type);
            pathFinder.HandlePaintedEdge(e);
            
            gameState.MaxLengthPath = pathFinder.MaxLengthPath;
        }

        public void MoveNode(Node n, float2 position)
        {
            graph.MoveNode(n, position);
        }

        public void Clear()
        {
            graph.Clear();
        }

        public bool IsValidEdge(Node start, Node end)
        {
            return graph.IsValidEdge(start, end);
        }
    }

}
