using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ramsey.Utilities;
using Unity.Mathematics;
using UnityEngine;

namespace Ramsey.Graph
{
    
    public class GraphManager
    {
        public IReadOnlyGraph Graph => graph;

        public IReadOnlyList<Node> Nodes => graph.Nodes;
        public IReadOnlyList<Edge> Edges => graph.Edges;
        public IEnumerable<IPath> Paths => pathFinder.AllPaths;

        public event Action OnFinishPathCalculation;

        public Node NodeFromID(int id) 
            => graph.NodeFromID(id);
        public Edge EdgeFromID(int id) 
            => graph.EdgeFromID(id);

        Task currentPathTask = null;

        public bool IsAwaitingPathTask => currentPathTask != null;
        public async Task AwaitPathTask()
        {
            if(IsAwaitingPathTask)
                await currentPathTask;
        }

        public IReadOnlyList<IPath> MaxPathsByType => pathFinder.MaxPathsByType;

        internal readonly Graph graph;
        internal readonly IIncrementalPathFinder pathFinder;

        internal GraphManager(Graph graph, IIncrementalPathFinder pathFinder)
        {
            this.graph = graph;
            this.pathFinder = pathFinder;
        }

        public GraphManager(IIncrementalPathFinder pathFinder) : this(new(), pathFinder)
        {}

        public static GraphManager UsingAlgorithm<TAlgo>() where TAlgo: IIncrementalPathFinder, new()
            => new(new TAlgo());

        public Node CreateNode(float2 position = default)
        {
            var n = graph.CreateNode(position);
            pathFinder.HandleNodeAddition(n);

            return n;
        }

        public Edge CreateEdge(Node start, Node end)
        {
            var e = graph.CreateEdge(start, end);

            return e;
        }

        public void PaintEdge(Edge e, int type) //Will start background task
        {   
            graph.PaintEdge(e, type);

            currentPathTask = Task.Run(async () => 
            {
                await pathFinder.HandlePaintedEdge(e, graph).UnityReport(); 
                currentPathTask = null; 

                OnFinishPathCalculation?.Invoke();
            }).UnityReport();
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
