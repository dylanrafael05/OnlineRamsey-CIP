namespace Ramsey.Graph
{
    internal struct SerializedPathFinderEntry
    {
        public int Node { get; set; }
        public int[] TerminatingPaths { get; set; }
    }
}