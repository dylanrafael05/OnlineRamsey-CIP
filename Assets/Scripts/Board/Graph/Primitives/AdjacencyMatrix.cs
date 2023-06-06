using System;
using System.Linq;
using Ramsey.Graph.Experimental;
using Ramsey.Utilities;
using UnityEngine;

namespace Ramsey.Graph
{
    public class AdjacencyMatrix : IAdjacencyMatrix
    {
        private BitSet bits = new();
        private int nodeCount = 0;

        public int Size => nodeCount;

        public void Expand(int newNodeCount)
        {
            var newBits = new BitSet();

            for(int i = 0; i < nodeCount; i++)
            {
                for(int j = 0; j < nodeCount; j++)
                {
                    if(bits.IsSet(i * nodeCount + j)) 
                        newBits.Set(i * newNodeCount + j);
                }
            }

            bits = newBits;
            nodeCount = newNodeCount;
        }

        public void AddAdjacency(Node start, Node end) 
        {
            bits.Set(nodeCount * start.ID + end.ID);
            bits.Set(nodeCount * end.ID + start.ID);

            Debug.Log($"Adding adjacency from {start.ID} -> {end.ID}");
        }

        public bool AreAdjacent(Node start, Node end)
        {
            return bits.IsSet(nodeCount * start.ID + end.ID);
        }
        public bool AreAdjacent(int start, int end) 
        {
            return bits.IsSet(nodeCount * start + end);
        }

        public override string ToString()
        {
            var s = ". | " + string.Join(' ', Enumerable.Range(0, Size).Select(i => i % 10)) + "\n--+" + new string('-', Size * 2) + "\n";

            for(int j = 0; j < Size; j++)
            {
                s += j.ToString().PadRight(Mathf.FloorToInt(Mathf.Log10(Size+0.5f))) + " | ";
                for(int i = 0; i < Size; i++)
                {
                    s += i == j ? "\\" : (AreAdjacent(i, j) ? "X" : "-");
                    s += " ";
                }
                s += "\n";
            }

            return s;
        }
    }
}