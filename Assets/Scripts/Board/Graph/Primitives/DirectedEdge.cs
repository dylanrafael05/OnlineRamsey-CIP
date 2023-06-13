namespace Ramsey.Graph
{
    public readonly struct DirectedEdge 
    {
        public DirectedEdge(Edge internalEdge, bool reversed)
        {
            InternalEdge = internalEdge;
            Reversed = reversed;
        }

        public Edge InternalEdge { get; }
        public bool Reversed { get; }

        public Node Start => Reversed ? InternalEdge.End : InternalEdge.Start;
        public Node End => Reversed ? InternalEdge.Start : InternalEdge.End;
        public int ID => InternalEdge.ID;
    }
}