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
        DrawingStorage storage;
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

        public Drawer(DrawingStorage storage, DrawingPreferences preferences, Camera camera)
        {
            Values.Initialize();

            //
            this.camera = camera;

            //
            this.storage = storage;
            this.preferences = preferences;

            //
            argsArrayEdge = new uint[5]
            {
                Values.QuadMesh.GetIndexCount(0),
                0,
                Values.QuadMesh.GetIndexStart(0),
                Values.QuadMesh.GetBaseVertex(0),
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
            Values.EdgeMaterial.SetBuffer(Shader.PropertyToID("Transforms"), edgeTransformBuffer);
            Values.EdgeMaterial.SetBuffer(Shader.PropertyToID("Colors"), edgeTypeBuffer);
            Values.EdgeMaterial.SetBuffer(Shader.PropertyToID("IsHighlighted"), edgeHighlightBuffer);

            Values.NodeMaterial.SetBuffer(Shader.PropertyToID("Positions"), nodePositionBuffer);
            Values.NodeMaterial.SetBuffer(Shader.PropertyToID("IsHighlighted"), nodeHighlightBuffer);

            //
            UpdateArgsBuffer();
            UpdateEdgeBuffer();
            UpdateNodeBuffer();

        }

        public void UpdateAll(DrawingStorage storage)
        { UpdateArgsBuffer(storage); UpdateEdgeBuffer(storage); UpdateNodeBuffer(storage); }

        public void UpdateAll()
            => UpdateAll(storage);

        public void UpdateArgsBuffer(DrawingStorage storage)
        {

            //
            argsArrayEdge[1] = (uint)storage.EdgeTransforms.Count;
            argsArrayNode[1] = (uint)storage.NodePositions.Count;

            argsBufferEdge.SetData(argsArrayEdge);
            argsBufferNode.SetData(argsArrayNode);

        }
        public void UpdateEdgeBuffer(DrawingStorage storage) 
        { 
            lock(storage.EdgeHighlights)
            {
                edgeTransformBuffer.SetData(storage.EdgeTransforms); 
                edgeTypeBuffer.SetData(storage.EdgeColors);
                edgeHighlightBuffer.SetData(storage.EdgeHighlights);
            }
        }
        public void UpdateNodeBuffer(DrawingStorage storage) { nodePositionBuffer.SetData(storage.NodePositions); nodeHighlightBuffer.SetData(storage.NodeHighlights); }

        public void UpdateArgsBuffer() => UpdateArgsBuffer(storage);
        public void UpdateEdgeBuffer() => UpdateEdgeBuffer(storage);
        public void UpdateNodeBuffer() => UpdateNodeBuffer(storage);

        public float RecordingScale 
        {
            get => Values.RecordingTransform.localScale.x;
            set => Values.RecordingTransform.localScale = new float3(value, ((float3)Values.RecordingTransform.localScale).yz);
        }
        
        public float2 Mouse { get; set; }

        public void Draw()
        {
            TextRenderer.Flush();

            if(storage.ShouldUpdateEdgeBuffer)
            {
                UpdateEdgeBuffer();
                storage.ShouldUpdateEdgeBuffer = false;
            }

            Values.NodeMaterial.SetVector("_Mouse", Mouse.xyzw());

            Graphics.DrawMeshInstancedIndirect(Values.QuadMesh, 0, Values.EdgeMaterial, Values.Bounds, argsBufferEdge, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, LayerMask.NameToLayer("Board"), camera);
            Graphics.DrawMeshInstancedIndirect(Values.QuadMesh, 0, Values.NodeMaterial, Values.Bounds, argsBufferNode, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, LayerMask.NameToLayer("Board"), camera);
            Graphics.DrawMesh(Values.QuadMesh, Values.RecordingTransform.WorldMatrix(), Values.RecorderMaterial, LayerMask.NameToLayer("Board"), camera);

            if (storage.IsLoading)
            {
                Graphics.DrawMesh(Values.QuadMesh, Values.LoadingTransform.WorldMatrix(), Values.LoadingMaterial, LayerMask.NameToLayer("Board"), camera);
            }

            for(var i = 0; i < storage.NodePositions.Count; i++) 
            {
                TextRenderer.Draw(storage.NodePositions[i], i.ToString());
            }
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