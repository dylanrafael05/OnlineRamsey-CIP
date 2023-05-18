namespace Ramsey.Graph
{
    internal struct SerializedGraph
    {
        public SerializedNode[] Nodes { get; set; }
        public SerializedEdge[] Edges { get; set; }
    }
}