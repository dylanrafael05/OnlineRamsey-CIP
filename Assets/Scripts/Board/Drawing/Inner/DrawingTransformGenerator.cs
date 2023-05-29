using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Ramsey.Graph;

namespace Ramsey.Drawing
{
    internal static class DrawingTransformGenerator
    {

        public static Matrix4x4 GenerateEdgeTransform(Node startNode, Node endNode, int type, float thickness)
        {
            float2 start = startNode.Position; float2 end = endNode.Position;
            float2 pos = (start + end) * .5f;
            float2 scale = new float2(length(start - end) * .5f, 1.0f);
            float theta = atan2((end - start).y, (end - start).x);

            return Matrix4x4.TRS(new Vector3(pos.x, pos.y, type == 0 ? 1f : 2f), Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * theta), new Vector3(scale.x, scale.y, 1f));

        }

    }
}