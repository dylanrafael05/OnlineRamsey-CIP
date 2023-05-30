using System.Linq;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Diagnostics;
using Ramsey.Screen;
using Ramsey.Utilities;

namespace Ramsey.Drawing
{
    internal class Drawer
    {

        //
        Camera camera;

        //
        DrawingData presentStorage;
        DrawingData currentStorage;
        DrawingPreferences preferences;

        //
        const int MAXMESHCOUNT = 4096;

        //
        uint[] argsArrayEdge;
        ComputeBuffer argsBufferEdge;

        uint[] argsArrayNode;
        ComputeBuffer argsBufferNode;

        //
        ComputeBuffer edgeTransformBuffer;
        ComputeBuffer edgeTypeBuffer;
        ComputeBuffer edgeHighlightBuffer;

        //
        ComputeBuffer nodePositionBuffer;
        ComputeBuffer nodeHighlightBuffer;

        public Drawer(DrawingData storage, DrawingPreferences preferences, Camera camera)
        {

            //
            this.camera = camera;

            //
            this.presentStorage = storage;
            this.currentStorage = storage;
            this.preferences = preferences;

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
            edgeHighlightBuffer = new(MAXMESHCOUNT, sizeof(float));

            nodePositionBuffer = new(MAXMESHCOUNT, Marshal.SizeOf<float2>());
            nodeHighlightBuffer = new(MAXMESHCOUNT, Marshal.SizeOf<float>());

            //Uniforms Prefs
            preferences.UniformPreferences();

            //Link To Shader
            DrawingPreferences.EdgeMaterial.SetBuffer(Shader.PropertyToID("Transforms"), edgeTransformBuffer);
            DrawingPreferences.EdgeMaterial.SetBuffer(Shader.PropertyToID("Colors"), edgeTypeBuffer);
            DrawingPreferences.EdgeMaterial.SetBuffer(Shader.PropertyToID("IsHighlighted"), edgeHighlightBuffer);

            DrawingPreferences.NodeMaterial.SetBuffer(Shader.PropertyToID("Positions"), nodePositionBuffer);
            DrawingPreferences.NodeMaterial.SetBuffer(Shader.PropertyToID("IsHighlighted"), nodeHighlightBuffer);

            //
            UpdateArgsBuffer();
            UpdateEdgeBuffer();
            UpdateNodeBuffer();

        }

        public void UpdateAll(DrawingData storage)
        { UpdateArgsBuffer(storage); UpdateEdgeBuffer(storage); UpdateNodeBuffer(storage); this.currentStorage = storage; }

        public void UpdateAll()
            => UpdateAll(presentStorage);

        public void UpdateArgsBuffer(DrawingData storage)
        {

            //
            argsArrayEdge[1] = (uint)storage.EdgeTransforms.Count;
            argsArrayNode[1] = (uint)storage.NodePositions.Count;

            argsBufferEdge.SetData(argsArrayEdge);
            argsBufferNode.SetData(argsArrayNode);

        }
        public void UpdateEdgeBuffer(DrawingData storage) 
        { 
            lock(storage.EdgeHighlights)
            {
                edgeTransformBuffer.SetData(storage.EdgeTransforms); 
                edgeTypeBuffer.SetData(storage.EdgeColors);
                edgeHighlightBuffer.SetData(storage.EdgeHighlights);
            }
        }
        public void UpdateNodeBuffer(DrawingData storage) { nodePositionBuffer.SetData(storage.NodePositions); nodeHighlightBuffer.SetData(storage.NodeHighlights); }

        public void UpdateArgsBuffer() => UpdateArgsBuffer(presentStorage);
        public void UpdateEdgeBuffer() => UpdateEdgeBuffer(presentStorage);
        public void UpdateNodeBuffer() => UpdateNodeBuffer(presentStorage);
        

        public float2 Mouse { get; set; }

        public void Draw()
        {
            TextRenderer.Flush();

            if(presentStorage.ShouldUpdateEdgeBuffer)
            {
                UpdateEdgeBuffer();
                presentStorage.ShouldUpdateEdgeBuffer = false;
            }

            DrawingPreferences.NodeMaterial.SetVector(Shader.PropertyToID("_Mouse"), Mouse.xyzw());

            Graphics.DrawMeshInstancedIndirect(DrawingData.QuadMesh, 0, DrawingPreferences.EdgeMaterial, DrawingData.Bounds, argsBufferEdge, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, LayerMask.NameToLayer("Board"), camera);
            Graphics.DrawMeshInstancedIndirect(DrawingData.QuadMesh, 0, DrawingPreferences.NodeMaterial, DrawingData.Bounds, argsBufferNode, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, LayerMask.NameToLayer("Board"), camera);
            Graphics.DrawMesh(DrawingData.QuadMesh, DrawingPreferences.RecorderTransform, DrawingPreferences.RecorderMaterial, LayerMask.NameToLayer("Board"), camera); //Mabe make expand as we add more pricks and maybe shrinkX pricks when we add more interpolate to low const approach -> inf

            if(presentStorage.IsLoading)
                Graphics.DrawMesh(DrawingData.QuadMesh, preferences.LoadingCircleTransform, DrawingPreferences.LoadingMaterial, LayerMask.NameToLayer("Board"), camera);

            for(var i = 0; i < currentStorage.NodePositions.Count; i++) 
                TextRenderer.Draw(currentStorage.NodePositions[i], i.ToString());
        }

        public void Cleanup()
        {
            argsBufferEdge.Dispose();
            argsBufferNode.Dispose();
            
            edgeTypeBuffer.Dispose();
            edgeTransformBuffer.Dispose();
            edgeHighlightBuffer.Dispose();

            nodePositionBuffer.Dispose();
            nodeHighlightBuffer.Dispose();
        }

    }
}