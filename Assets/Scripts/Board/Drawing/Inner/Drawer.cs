using System.Linq;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Diagnostics;
using Ramsey.Utilities;

namespace Ramsey.Drawing
{
    internal class Drawer
    {

        //
        Camera camera;

        //
        DrawingStorage presentStorage;
        DrawingStorage currentStorage;
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
            UnityReferences.Initialize();

            //
            this.camera = camera;

            //
            this.presentStorage = storage;
            this.currentStorage = storage;
            this.preferences = preferences;

            //
            argsArrayEdge = new uint[5]
            {
                MeshUtils.QuadMesh.GetIndexCount(0),
                0,
                MeshUtils.QuadMesh.GetIndexStart(0),
                MeshUtils.QuadMesh.GetBaseVertex(0),
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
            UnityReferences.EdgeMaterial.SetBuffer(Shader.PropertyToID("Transforms"), edgeTransformBuffer);
            UnityReferences.EdgeMaterial.SetBuffer(Shader.PropertyToID("Colors"), edgeTypeBuffer);
            UnityReferences.EdgeMaterial.SetBuffer(Shader.PropertyToID("IsHighlighted"), edgeHighlightBuffer);

            UnityReferences.NodeMaterial.SetBuffer(Shader.PropertyToID("Positions"), nodePositionBuffer);
            UnityReferences.NodeMaterial.SetBuffer(Shader.PropertyToID("IsHighlighted"), nodeHighlightBuffer);

            //
            UpdateArgsBuffer();
            UpdateEdgeBuffer();
            UpdateNodeBuffer();

        }

        public void UpdateAll(DrawingStorage storage)
        { UpdateArgsBuffer(storage); UpdateEdgeBuffer(storage); UpdateNodeBuffer(storage); this.currentStorage = storage; }

        public void UpdateAll()
            => UpdateAll(presentStorage);

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

        public void UpdateArgsBuffer() => UpdateArgsBuffer(presentStorage);
        public void UpdateEdgeBuffer() => UpdateEdgeBuffer(presentStorage);
        public void UpdateNodeBuffer() => UpdateNodeBuffer(presentStorage);
        

        public float RecordingScale 
        {
            get => UnityReferences.RecordingTransform.localScale.x;
            set => UnityReferences.RecordingTransform.localScale = new float3(value, ((float3)UnityReferences.RecordingTransform.localScale).yz);
        }
        
        public float2 Mouse { get; set; }

        public void Draw()
        {
            TextRenderer.Flush();

            if(presentStorage.ShouldUpdateEdgeBuffer)
            {
                UpdateEdgeBuffer();
                presentStorage.ShouldUpdateEdgeBuffer = false;
            }

            UnityReferences.NodeMaterial.SetVector("_Mouse", Mouse.xyzw());

            Graphics.DrawMeshInstancedIndirect(MeshUtils.QuadMesh, 0, UnityReferences.EdgeMaterial, UnityReferences.Bounds, argsBufferEdge, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, LayerMask.NameToLayer("Board"), camera);
            Graphics.DrawMeshInstancedIndirect(MeshUtils.QuadMesh, 0, UnityReferences.NodeMaterial, UnityReferences.Bounds, argsBufferNode, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, LayerMask.NameToLayer("Board"), camera);
            Graphics.DrawMesh(MeshUtils.QuadMesh, UnityReferences.RecordingTransform.WorldMatrix(), UnityReferences.RecorderMaterial, LayerMask.NameToLayer("Board"), camera);

            if (presentStorage.IsLoading)
            {
                Graphics.DrawMesh(MeshUtils.QuadMesh, UnityReferences.LoadingTransform.WorldMatrix(), UnityReferences.LoadingMaterial, LayerMask.NameToLayer("Board"), camera);
            }

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