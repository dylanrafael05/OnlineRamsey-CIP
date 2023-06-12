using System.Collections.Generic;
using Ramsey.Utilities;
using UnityEngine;

namespace Ramsey.Drawing 
{
    public class InfinitePropertyBlock
    {
        public const int MAX_PER_BLOCK = 1023;

        private readonly List<MaterialPropertyBlock> blocks = new();
        private readonly HashSet<string> propertiesSeen = new();

        public IEnumerable<(int count, MaterialPropertyBlock block)> GetRenderBlocks(int count)
        {
            foreach(var (b, i) in blocks.Indexed())
            {
                var thiscount = Mathf.Min(MAX_PER_BLOCK, count - MAX_PER_BLOCK * i);

                if(thiscount < 0) yield break;

                yield return (thiscount, b);
            }
        }

        private void EnsureBlocks(int count)
        {
            while(blocks.Count * MAX_PER_BLOCK < count)
            {
                var block = new MaterialPropertyBlock();
                blocks.Add(block);
            }
        }

        private int MaxIndex(int count)
            => Mathf.CeilToInt(count * 1f / MAX_PER_BLOCK);

        public void SetVectorArray(string name, Vector4[] vecs)
        {
            if(vecs.Length == 0) return;
            EnsureBlocks(vecs.Length);

            for(int i = 0; i < MaxIndex(vecs.Length); i++)
            {
                if(!propertiesSeen.Contains(name)) blocks[i].SetVectorArray(name, new Vector4[MAX_PER_BLOCK]);
                blocks[i].SetVectorArray(name, vecs[(i*MAX_PER_BLOCK)..Mathf.Min(vecs.Length, i*MAX_PER_BLOCK+MAX_PER_BLOCK)]);
            }

            propertiesSeen.Add(name);
        }
        public void SetVectorArray(string name, List<Vector4> vecs) => SetVectorArray(name, vecs.ToArray());

        public void SetFloatArray(string name, float[] floats)
        {
            if(floats.Length == 0) return;
            EnsureBlocks(floats.Length);

            for(int i = 0; i < MaxIndex(floats.Length); i++)
            {
                if(!propertiesSeen.Contains(name)) blocks[i].SetFloatArray(name, new float[MAX_PER_BLOCK]);
                blocks[i].SetFloatArray(name, floats[(i*MAX_PER_BLOCK)..Mathf.Min(floats.Length, i*MAX_PER_BLOCK+MAX_PER_BLOCK)]);
            }
            
            propertiesSeen.Add(name);
        }
        public void SetFloatArray(string name, List<float> floats) => SetFloatArray(name, floats.ToArray());

        public void SetMatrixArray(string name, Matrix4x4[] mats)
        {
            if(mats.Length == 0) return;
            EnsureBlocks(mats.Length);

            for(int i = 0; i < MaxIndex(mats.Length); i++)
            {
                if(!propertiesSeen.Contains(name)) blocks[i].SetMatrixArray(name, new Matrix4x4[MAX_PER_BLOCK]);
                blocks[i].SetMatrixArray(name, mats[(i*MAX_PER_BLOCK)..Mathf.Min(mats.Length, i*MAX_PER_BLOCK+MAX_PER_BLOCK)]);
            }
            
            propertiesSeen.Add(name);
        }
        public void SetMatrixArray(string name, List<Matrix4x4> mats) => SetMatrixArray(name, mats.ToArray());
    }
}