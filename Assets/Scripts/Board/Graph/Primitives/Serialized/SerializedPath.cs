namespace Ramsey.Graph
{
    internal struct SerializedPath
    {
        public int[] Nodes { get; set; }
        public int End { get; set; }
        public int Type { get; set; }
        public int ID { get; set; }
    }
}