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

            drawer.UpdateNodeBuffer();
            drawer.UpdateArgsBuffers();
        }

        public void UpdateNodePosition(Node n)
        {
            Assert.IsTrue(data.NodePositions.Count > n.ID, "Nodes must be added to renderer upon creation!");

            data.NodePositions[n.ID] = n.Position;

            drawer.UpdateNodeBuffer();
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