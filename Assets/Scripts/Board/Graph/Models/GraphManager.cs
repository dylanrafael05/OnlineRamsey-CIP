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
    public class GraphManager : IGraphManager
    {
        public IReadOnlyGraph Graph => graph;

        public IReadOnlyList<Node> Nodes => graph.Nodes;
        public IReadOnlyList<Edge> Edges => graph.Edges;
        public int? MaxNodeCount => pathFinder.MaxSupportedNodeCount;

        public event Action OnFinishPathCalculation;

        public Node NodeFromID(int id) 
            => graph.NodeFromID(id);
        public Edge EdgeFromID(int id) 
            => graph.EdgeFromID(id);

        public void LoadGraph(IGraph graph)
        {
            this.graph = graph;
            pathFinder.HandleFullGraph(graph);
        }

        Task currentPathTask = null;

        /// <summary>
        /// Check if the graph manager is waiting for path finding
        /// to complete.
        /// </summary>
        public bool IsAwaitingPathTask => currentPathTask != null;
        /// <summary>
        /// Wait for path finding to complete.
        /// </summary>
        public async Task AwaitPathTask()
        {
            if(IsAwaitingPathTask)
                await currentPathTask;
        }

        public IReadOnlyList<IPath> MaxPathsByType => pathFinder.MaxPathsByType;

        internal IGraph graph;
        internal readonly IIncrementalPathFinder pathFinder;

        internal GraphManager(IGraph graph, IIncrementalPathFinder pathFinder)
        {
            this.graph = graph;
            this.pathFinder = pathFinder;
        }

        public GraphManager(IIncrementalPathFinder pathFinder) : this(new Graph(), pathFinder)
        {}

        public static GraphManager UsingAlgorithm<TAlgo>() where TAlgo: IIncrementalPathFinder, new()
            => new(new TAlgo());

        /// <inheritdoc cref="Graph.CreateNode(float2)"/>
        public Node CreateNode(float2 position = default)
        {
            if(pathFinder.MaxSupportedNodeCount is int max && Nodes.Count > max)
            {
                throw new GraphTooComplexException(max);
            }

            var n = graph.CreateNode(position);
            pathFinder.HandleNodeAddition(n);

            return n;
        }

        /// <inheritdoc cref="Graph.CreateEdge(Node, Node)"/>
        public Edge CreateEdge(Node start, Node end)
        {
            var e = graph.CreateEdge(start, end);

            return e;
        }

        /// <summary>
        /// Paint the given edge with the given type, updating
        /// path finding either in the background (synchronous = false)
        /// or immediately (synchronous = true).
        /// </summary>
        public void PaintEdge(Edge e, int type, bool synchronous = false) //Will start background task
        {   
            graph.PaintEdge(e, type);

            currentPathTask = Utils.Run(synchronous, delegate
            {
                pathFinder.HandlePaintedEdge(e, graph); 
                currentPathTask = null; 

                OnFinishPathCalculation?.Invoke();
            });
        }

        /// <inheritdoc cref="Graph.MoveNode(Node, float2)"/>
        public void MoveNode(Node n, float2 position)
        {
            graph.MoveNode(n, position);
        }

        /// <inheritdoc cref="Graph.Clear()"/>
        public void Clear()
        {
            graph.Clear();
            
            pathFinder.Clear();
        }

        /// <inheritdoc cref="Graph.IsValidEdge(Node, Node)"/>
        public bool IsValidEdge(Node start, Node end)
        {
            return graph.IsValidEdge(start, end);
        }
    }
}