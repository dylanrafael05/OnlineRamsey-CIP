﻿using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Ramsey.Utilities;

namespace Ramsey.Drawing
{
    internal class DrawingStorage
    {
        public DrawingStorage(List<Matrix4x4> EdgeTransforms = null, List<Vector4> EdgeColors = null, List<float> EdgeHighlights = null, List<Vector4> NodePositions = null, List<float> NodeHighlights = null)
        {
            this.EdgeTransforms = EdgeTransforms ?? new();
            this.EdgeColors = EdgeColors ?? new();
            this.EdgeHighlights = EdgeHighlights ?? new();

            this.NodePositions = NodePositions ?? new();
            this.NodeHighlights = NodeHighlights ?? new();
        }

        public List<Matrix4x4> EdgeTransforms { get; private set; }
        public List<Vector4> EdgeColors { get; private set; }
        public List<float> EdgeHighlights { get; private set; }

        public List<Vector4> NodePositions { get; private set; }
        public List<float> NodeHighlights { get; private set; }

        public bool ShouldUpdateEdgeBuffer { get; set; }

        public bool IsLoading { get; set; }

        public int EdgeCount => EdgeTransforms.Count;
        public int NodeCount => NodePositions.Count;

        public DrawState CreateState() => new(new (EdgeTransforms.Copy(), EdgeColors.Copy(), EdgeHighlights.Copy(), NodePositions.Copy(), NodeHighlights.Copy()));

    }

    public struct DrawState
    {

        internal DrawState(DrawingStorage data) => this.Data = data;
        internal DrawingStorage Data { get; private set; }

    }
}