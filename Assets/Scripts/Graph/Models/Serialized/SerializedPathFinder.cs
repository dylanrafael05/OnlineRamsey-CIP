namespace Ramsey.Core
{
    internal struct SerializedPathFinder
    {
        public SerializedPath[] Paths { get; set; }
        public SerializedPathFinderEntry[] NodesByTerminatingPaths { get; set; }
        public int MaxLengthPath { get; set; }
    }
}