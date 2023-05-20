using Ramsey.Graph;
using UnityEngine.Assertions;
using Unity.Mathematics;
using UnityEngine;

namespace Ramsey.Drawing
{
    public class DrawingWritingInterface
    {

        DrawingData data;
        DrawingPreferences preferences;
        Drawer drawer;

        internal DrawingWritingInterface(DrawingData data, DrawingPreferences preferences, Drawer drawer)
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
            drawer.UpdateArgsBuffers();
        }
        public void AddNode(Node n)
        {
            Assert.AreEqual(data.NodePositions.Count, n.ID, "Nodes must be added to renderer upon creation!");

            data.NodePositions.Add(n.Position);
            data.NodeHighlights.Add(0);

            drawer.UpdateNodeBuffer();
            drawer.UpdateArgsBuffers();
        }

        public void UpdateNodePosition(Node n)
        {
            Assert.IsTrue(data.NodePositions.Count > n.ID, "Nodes must be added to renderer upon creation!");

            data.NodePositions[n.ID] = n.Position;
            foreach(var e in n.Edges)
            {
                UpdateEdgeTransform(e);
            }

            drawer.UpdateNodeBuffer();
        }

        public void HighlightNode(Node n)
        {
            Assert.IsTrue(data.NodePositions.Count >= n.ID, "Nodes must be added to renderer before they are highlighted!");

            data.NodeHighlights[n.ID] = 1;
        }
        public void UnhighlightNode(Node n)
        {
            Assert.IsTrue(data.NodePositions.Count >= n.ID, "Nodes must be added to renderer before they are highlighted!");

            data.NodeHighlights[n.ID] = 0;
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

        public void Clear()
        {
            data.NodePositions.Clear();
            data.EdgeTransforms.Clear();
            data.EdgeColors.Clear();
        }
    }
}