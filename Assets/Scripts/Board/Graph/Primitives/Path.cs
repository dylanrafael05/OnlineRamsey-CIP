using System.Collections.Generic;

namespace Ramsey.Graph
{
    public class Path 
    {
        internal Path(HashSet<Node> nodes, Node end, int type)
        {
            this.nodes = nodes;
            
            End = end;
            Type = type;
        }

        private Path(Path path) 
        {
            nodes = new HashSet<Node>(path.nodes);
            End = path.End;
            Type = path.Type;
        }

        public Path(Node node, int type) 
        {
            nodes = new() {node};
            End = node;
            Type = type;
        }

        private HashSet<Node> nodes;

        public int Type { get; }

        public IReadOnlyCollection<Node> Nodes => nodes;
        public int Length => nodes.Count;

        public Node End { get; private set; }

        public bool Contains(Node node) 
        {
            return nodes.Contains(node);
        }
        public bool Add(Node node) 
        {
            if(Contains(node)) return false;

            nodes.Add(node);
            End = node;

            return true;
        }

        public Path Append(Node node) 
        {
            var other = new Path(this);
            other.Add(node);

            return other;
        }
    }
}