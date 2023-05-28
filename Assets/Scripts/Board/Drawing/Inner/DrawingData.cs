using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Ramsey.Utilities;

namespace Ramsey.Drawing
{
    internal class DrawingData
    {

        //
        public static Mesh QuadMesh => quadMesh;
        public static readonly Bounds Bounds = new Bounds(Vector3.zero, Vector3.one * 100f); //temp
        static readonly Mesh quadMesh = new Mesh()
        {
            vertices = new Vector3[]
            {
            new (-1f, -1f, 0f),
            new (-1f, 1f, 0f),
            new (1f, -1f, 0f),
            new (1f, 1f, 0f)
            },
            triangles = new int[]
            {
            0, 1, 3,
            0, 3, 2
            },
            uv = new Vector2[]
            {
            new (-1f, -1f),
            new (-1f, 1f),
            new (1f, -1f),
            new (1f, 1f)
            }
        };

        public DrawingData(List<Matrix4x4> EdgeTransforms = null, List<Color> EdgeColors = null, List<float> EdgeHighlights = null, List<float2> NodePositions = null, List<float> NodeHighlights = null)
        {
            this.EdgeTransforms = EdgeTransforms ?? new();
            this.EdgeColors = EdgeColors ?? new();
            this.EdgeHighlights = EdgeHighlights ?? new();

            this.NodePositions = NodePositions ?? new();
            this.NodeHighlights = NodeHighlights ?? new();
        }

        public List<Matrix4x4> EdgeTransforms { get; private set; }
        public List<Color> EdgeColors { get; private set; }
        public List<float> EdgeHighlights { get; private set; }

        public List<float2> NodePositions { get; private set; }
        public List<float> NodeHighlights { get; private set; }

        public bool ShouldUpdateEdgeBuffer { get; set; }
        
        public bool IsLoading { get; set; }

        public DrawState CreateState() => new(new (EdgeTransforms.Copy(), EdgeColors.Copy(), EdgeHighlights.Copy(), NodePositions.Copy(), NodeHighlights.Copy()));

    }

    public struct DrawState
    {

        internal DrawState(DrawingData data) => this.Data = data;
        internal DrawingData Data { get; private set; }

    }
}