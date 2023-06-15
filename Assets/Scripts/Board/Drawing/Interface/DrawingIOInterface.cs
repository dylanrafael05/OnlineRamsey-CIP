using Ramsey.Graph;
using UnityEngine.Assertions;
using Unity.Mathematics;
using UnityEngine;
using Ramsey.Utilities;
using System.Data.SqlTypes;

namespace Ramsey.Drawing
{
    public class DrawingIOInterface
    {

        readonly DrawingStorage data;
        readonly DrawingPreferences preferences;
        readonly Drawer drawer;

        internal DrawingIOInterface(DrawingStorage data, DrawingPreferences preferences, Drawer drawer)
        {
            this.data = data;
            this.preferences = preferences;
            this.drawer = drawer;
        }

        public void AddEdge(Edge e)
        {
            Assert.AreEqual(data.EdgeTransforms.Count, e.ID, "Edges must be added to renderer upon creation!");

            data.EdgeTransforms.Add(DrawingTransformGenerator.GenerateEdgeTransform(e.Start, e.End, e.Type, preferences.edgeThickness));
            data.EdgeColors.Add(preferences.TypeToColor(e.Type));
            data.EdgeHighlights.Add(0);
            data.EdgeReversal.Add(0);

            drawer.UpdateEdgeBuffer();
        }
        public void AddNode(Node n)
        {
            Assert.AreEqual(data.NodePositions.Count, n.ID, "Nodes must be added to renderer upon creation!");

            data.NodeTransforms.Add(Matrix4x4.Translate(n.Position.xyz()));
            data.NodePositions.Add(n.Position);
            data.NodeHighlights.Add(0);

            drawer.UpdateNodeBuffer();
        }

        public void UpdateNodePosition(Node n)
        {
            Assert.IsTrue(data.NodePositions.Count > n.ID, "Nodes must be added to renderer upon creation!");

            data.NodePositions[n.ID] = n.Position;
            data.NodeTransforms[n.ID] = Matrix4x4.Translate(n.Position.xyz());

            foreach(var e in n.ConnectedEdges)
            {
                UpdateEdgeTransform(e);
            }

            drawer.UpdateNodeBuffer();
            drawer.UpdateEdgeBuffer();
        }

        public void HighlightNode(Node n)
        {
            Assert.IsTrue(data.NodePositions.Count >= n.ID, "Nodes must be added to renderer before they are highlighted!");

            data.NodeHighlights[n.ID] = 1;
            drawer.UpdateNodeBuffer();
        }
        public void UnhighlightNode(Node n)
        {
            Assert.IsTrue(data.NodePositions.Count >= n.ID, "Nodes must be added to renderer before they are highlighted!");

            data.NodeHighlights[n.ID] = 0;
            drawer.UpdateNodeBuffer();
        }

        internal void UpdateEdgeTransform(Edge e)
        {
            Assert.IsTrue(e.ID <= data.EdgeTransforms.Count, "Cannot update an edge transform for an edge which has not yet been added.");

            data.EdgeTransforms[e.ID] = DrawingTransformGenerator.GenerateEdgeTransform(e.Start, e.End, e.Type, preferences.edgeThickness);
        }

        public void UpdateEdgeType(Edge e)
        {
            data.EdgeColors[e.ID] = preferences.TypeToColor(e.Type);
            drawer.UpdateEdgeBuffer();
        }

        public void SetLoading(bool isLoading)
            => data.IsLoading = isLoading;

        public void SetHighlightedPathAsync(IPath path)
        {
            for(int i = 0; i < data.EdgeHighlights.Count; i++)
            {
                data.EdgeHighlights[i] = 0.0f;
            }

            foreach(var e in path.DirectedEdges)
            {
                data.EdgeHighlights[e.ID] = 1.0f;
                data.EdgeReversal[e.ID] = e.Reversed.ToInt();
            }

            data.ShouldUpdateEdgeBuffer = true;
        }

        public void SetMousePosition(float2 position)
        {
            drawer.Mouse = position;
        }

        public void LoadDrawState(DrawState drawState)
            => drawer.UpdateAll(drawState.Data);

        public void LoadDrawState()
            => drawer.UpdateAll();

        public void UpdateRecorder(int prickAmount, int selectedID)
        {
            UnityReferences.RecorderMaterial.SetFloat("_PrickAmount", (float)prickAmount);
            UnityReferences.RecorderMaterial.SetFloat("_PrickSelectID", (float)selectedID);

            float scale = 2.0f + 25.0f * math.max(0, 1-math.exp(-0.04f*(prickAmount - 6)));
            drawer.RecordingScaleX = scale;

            UnityReferences.RecorderMaterial.SetFloat("_xScale", scale);
            
        }

        public void Clear()
        {
            data.EdgeTransforms.Clear();
            data.EdgeColors.Clear();
            data.EdgeHighlights.Clear();

            data.NodePositions.Clear();
            data.NodeHighlights.Clear();
        }

        public void ClearScreen()
        {
            TextRenderer.ClearAll();
        }

        public DrawState CreateDrawState()
            => data.CreateState();
    }
}