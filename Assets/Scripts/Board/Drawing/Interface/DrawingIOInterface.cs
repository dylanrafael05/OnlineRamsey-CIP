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

        readonly DrawingData data;
        readonly DrawingPreferences preferences;
        readonly Drawer drawer;

        internal DrawingIOInterface(DrawingData data, DrawingPreferences preferences, Drawer drawer)
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

            drawer.UpdateEdgeBuffer();
            drawer.UpdateArgsBuffer();
        }
        public void AddNode(Node n)
        {
            Assert.AreEqual(data.NodePositions.Count, n.ID, "Nodes must be added to renderer upon creation!");

            data.NodePositions.Add(n.Position);
            data.NodeHighlights.Add(0);

            drawer.UpdateNodeBuffer();
            drawer.UpdateArgsBuffer();
        }

        public void UpdateNodePosition(Node n)
        {
            Assert.IsTrue(data.NodePositions.Count > n.ID, "Nodes must be added to renderer upon creation!");

            data.NodePositions[n.ID] = n.Position;
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

        public void SetHighlightedPath(Path path)
        {
            data.EdgeHighlights.Fill(0f);
            foreach(var e in path.Edges)
            {
                data.EdgeHighlights[e.ID] = 1.0f;
            }

            drawer.UpdateEdgeBuffer();
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
            Debug.Log(prickAmount);
            DrawingPreferences.RecorderMaterial.SetFloat("_PrickAmount", (float)prickAmount);
            DrawingPreferences.RecorderMaterial.SetFloat("_PrickSelectID", (float)selectedID);
        }

        public void Clear()
        {
            data.EdgeTransforms.Clear();
            data.EdgeColors.Clear();
            data.EdgeHighlights.Clear();

            data.NodePositions.Clear();
            data.NodeHighlights.Clear();
        }

        public DrawState CreateDrawState()
            => data.CreateState();
    }
}