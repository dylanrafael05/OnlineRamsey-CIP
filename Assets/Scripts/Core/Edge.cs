namespace Ramsey.Core
{
    public class Edge 
    {
        internal Edge(Node start, Node end, int type)
        {
            Start = start;
            End = end;
            Type = type;
        }

        public Node Start { get; }
        public Node End { get; }
        public int Type { get; }
    }
}