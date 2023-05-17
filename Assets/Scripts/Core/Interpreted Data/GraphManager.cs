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

        private Graph graph;
        private IncrementalPathFinder pathFinder;
        private GameState gameState;

        public GraphManager()
        {
            graph = new Graph();
            pathFinder = new IncrementalPathFinder();
            gameState = new GameState
            {
                Graph = graph
            };
        }

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
            throw new NotImplementedException();
        }

        public bool IsValidEdge(Node start, Node end)
        {
            throw new NotImplementedException();
        }
    }

}
