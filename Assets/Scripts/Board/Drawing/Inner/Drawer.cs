using System.Linq;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Diagnostics;
using Ramsey.Screen;

namespace Ramsey.Drawing
{
    internal class Drawer
    {

        //
        Camera camera;

        //
        DrawingData storage;

        //
        const int MAXMESHCOUNT = 4096;

        //
        uint[] argsArrayEdge;
        ComputeBuffer argsBufferEdge;

        uint[] argsArrayNode;
        ComputeBuffer argsBufferNode;

        //
        int edgeCount;
        ComputeBuffer edgeTransformBuffer;
        ComputeBuffer edgeTypeBuffer;

        //
        int nodeCount;
        ComputeBuffer nodePositionBuffer;

        public Drawer(DrawingData storage, DrawingPreferences preferences, Camera camera)
        {

            //
            this.camera = camera;

            //
            this.storage = storage;

            //
            argsArrayEdge = new uint[5]
            {
            DrawingData.QuadMesh.GetIndexCount(0),
            0,
            DrawingData.QuadMesh.GetIndexStart(0),
            DrawingData.QuadMesh.GetBaseVertex(0),
            0
            };
            argsArrayNode = new uint[5]; argsArrayEdge.CopyTo(argsArrayNode, 0);

            argsBufferEdge = new(MAXMESHCOUNT, sizeof(uint), ComputeBufferType.IndirectArguments);
            argsBufferNode = new(MAXMESHCOUNT, sizeof(uint), ComputeBufferType.IndirectArguments);

            edgeTransformBuffer = new(MAXMESHCOUNT, Marshal.SizeOf<Matrix4x4>());
            edgeTypeBuffer = new(MAXMESHCOUNT, Marshal.SizeOf<float4>());

            nodePositionBuffer = new(MAXMESHCOUNT, Marshal.SizeOf<float2>());

            //Uniforms Prefs
            preferences.UniformPreferences();

            //Link To Shader
            DrawingPreferences.EdgeMaterial.SetBuffer(Shader.PropertyToID("Transforms"), edgeTransformBuffer);
            DrawingPreferences.EdgeMaterial.SetBuffer(Shader.PropertyToID("Colors"), edgeTypeBuffer);

            DrawingPreferences.NodeMaterial.SetBuffer(Shader.PropertyToID("Positions"), nodePositionBuffer);

            //
            UpdateArgsBuffers();
            UpdateEdgeBuffer();
            UpdateNodeBuffer();

        }

        public void UpdateArgsBuffers()
        {

            //
            argsArrayEdge[1] = (uint)storage.EdgeTransforms.Count;
            argsArrayNode[1] = (uint)storage.NodePositions.Count;

            argsBufferEdge.SetData(argsArrayEdge);
            argsBufferNode.SetData(argsArrayNode);

        }
        public void UpdateEdgeBuffer() { edgeTransformBuffer.SetData(storage.EdgeTransforms); edgeTypeBuffer.SetData(storage.EdgeColors); }
        public void UpdateNodeBuffer() => nodePositionBuffer.SetData(storage.NodePositions);

        public void Draw()
        {
            Graphics.DrawMeshInstancedIndirect(DrawingData.QuadMesh, 0, DrawingPreferences.EdgeMaterial, DrawingData.Bounds, argsBufferEdge, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, LayerMask.NameToLayer("Board"), camera);
            Graphics.DrawMeshInstancedIndirect(DrawingData.QuadMesh, 0, DrawingPreferences.NodeMaterial, DrawingData.Bounds, argsBufferNode, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, LayerMask.NameToLayer("Board"), camera);
        }

        public void Cleanup()
        {
            argsBufferEdge.Dispose();
            argsBufferNode.Dispose();
            edgeTypeBuffer.Dispose();
            edgeTransformBuffer.Dispose();
            nodePositionBuffer.Dispose();
        }

    }
}