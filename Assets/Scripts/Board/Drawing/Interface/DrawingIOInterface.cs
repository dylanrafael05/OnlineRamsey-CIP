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
        #if HEADLESS
            public void AddEdge(Edge _) {}
            public void AddNode(Node _) {}
            public void UpdateNodePosition(Node _) {}
            public void HighlightNode(Node _) {}
            public void UnhighlightNode(Node _) {}
            public void UpdateEdgeTransform(Edge _) {}
            public void UpdateEdgeType(Edge _) {}
            public void SetLoading(bool _) {}
            public void SetHighlightedPathAsync(IPath _) {}
            public void SetMousePosition(float2 _) {}
            public void LoadDrawState(DrawState _) {}
            public void LoadDrawState() {}
            public void UpdateRecorder(int _, int _) {}
            public void Clear() {}
            public void ClearScreen() {}
            public DrawState CreateDrawState() => new();
        #else
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

                data.ShouldUpdateEdgeBuffer = true;
            }
            public void AddNode(Node n)
            {
                Assert.AreEqual(data.NodePositions.Count, n.ID, "Nodes must be added to renderer upon creation!");

                data.NodeTransforms.Add(Matrix4x4.Translate(n.Position.xyz()));
                data.NodePositions.Add(n.Position);
                data.NodeHighlights.Add(0);

                data.ShouldUpdateNodeBuffer = true;
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

                data.ShouldUpdateNodeBuffer = true;
                data.ShouldUpdateEdgeBuffer = true;
            }

            public void HighlightNode(Node n)
            {
                Assert.IsTrue(data.NodePositions.Count >= n.ID, "Nodes must be added to renderer before they are highlighted!");

                data.NodeHighlights[n.ID] = 1;
                data.ShouldUpdateNodeBuffer = true;
            }
            public void UnhighlightNode(Node n)
            {
                Assert.IsTrue(data.NodePositions.Count >= n.ID, "Nodes must be added to renderer before they are highlighted!");

                data.NodeHighlights[n.ID] = 0;
                data.ShouldUpdateNodeBuffer = true;
            }

            internal void UpdateEdgeTransform(Edge e)
            {
                Assert.IsTrue(e.ID <= data.EdgeTransforms.Count, "Cannot update an edge transform for an edge which has not yet been added.");

                data.EdgeTransforms[e.ID] = DrawingTransformGenerator.GenerateEdgeTransform(e.Start, e.End, e.Type, preferences.edgeThickness);
            }

            public void UpdateEdgeType(Edge e)
            {
                data.EdgeColors[e.ID] = preferences.TypeToColor(e.Type);
                data.ShouldUpdateEdgeBuffer = true;
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
                data.EdgeReversal.Clear();

                data.NodePositions.Clear();
                data.NodeHighlights.Clear();
                data.NodeTransforms.Clear();
            }

            public void ClearScreen()
            {
                TextRenderer.ClearAll();
            }

            public DrawState CreateDrawState()
                => data.CreateState();
        #endif
    }
}