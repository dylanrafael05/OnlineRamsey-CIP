using System.Linq;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Diagnostics;
using Ramsey.Utilities;
using UnityEngine.Rendering;

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
        InfinitePropertyBlock edgeProps = new();
        InfinitePropertyBlock nodeProps = new();

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
            // argsArrayEdge = new uint[5]
            // {
            //     MeshUtils.QuadMesh.GetIndexCount(0),
            //     0,
            //     MeshUtils.QuadMesh.GetIndexStart(0),
            //     MeshUtils.QuadMesh.GetBaseVertex(0),
            //     0
            // };
            // argsArrayNode = new uint[5]; argsArrayEdge.CopyTo(argsArrayNode, 0);

            // argsBufferEdge = new(MAXMESHCOUNT, sizeof(uint), ComputeBufferType.IndirectArguments);
            // argsBufferNode = new(MAXMESHCOUNT, sizeof(uint), ComputeBufferType.IndirectArguments);

            // edgeTransformBuffer = new(MAXMESHCOUNT, Marshal.SizeOf<Matrix4x4>());
            // edgeTypeBuffer = new(MAXMESHCOUNT, Marshal.SizeOf<float4>());
            // edgeHighlightBuffer = new(MAXMESHCOUNT, sizeof(float));

            // nodePositionBuffer = new(MAXMESHCOUNT, Marshal.SizeOf<float2>());
            // nodeHighlightBuffer = new(MAXMESHCOUNT, Marshal.SizeOf<float>());

            //Uniforms Prefs
            preferences.UniformPreferences();

            //
            UpdateEdgeBuffer();
            UpdateNodeBuffer();

            // Setup canvas rendering
            Canvas.preWillRenderCanvases += DrawUI;

        }

        public void UpdateAll(DrawingStorage storage)
        { UpdateEdgeBuffer(storage); UpdateNodeBuffer(storage); this.currentStorage = storage; }

        public void UpdateAll()
            => UpdateAll(presentStorage);
        
        public void UpdateEdgeBuffer(DrawingStorage storage) 
        {
            if(storage.EdgeCount == 0) return;

            edgeProps.SetVectorArray("_Colors", storage.EdgeColors);
            edgeProps.SetFloatArray("_IsHighlighted", storage.EdgeHighlights);
        }
        public void UpdateNodeBuffer(DrawingStorage storage) 
        { 
            if(storage.NodeCount == 0) return;

            nodeProps.SetFloatArray("_IsHighlighted", storage.NodeHighlights);
        }

        public void UpdateEdgeBuffer() => UpdateEdgeBuffer(presentStorage);
        public void UpdateNodeBuffer() => UpdateNodeBuffer(presentStorage);
        

        public float RecordingScaleX 
        {
            get => UnityReferences.RecordingTransform.localScale.x;
            set => UnityReferences.RecordingTransform.localScale = new float3(value, ((float3)UnityReferences.RecordingTransform.localScale).yz);
        }
        
        public float2 Mouse { get; set; }

        public void DrawBoard()
        {
            TextRenderer.Begin();

            if(presentStorage.ShouldUpdateEdgeBuffer)
            {
                UpdateEdgeBuffer();
                presentStorage.ShouldUpdateEdgeBuffer = false;
            }

            UnityReferences.NodeMaterial.SetVector("_Mouse", Mouse.xyzw());

            foreach(var (count, block) in edgeProps.GetRenderBlocks(currentStorage.EdgeCount)) 
                Graphics.DrawMeshInstanced(
                    MeshUtils.QuadMesh, 0, 
                    UnityReferences.EdgeMaterial, 
                    currentStorage.EdgeTransforms.ToArray(), 
                    count, block, 
                    ShadowCastingMode.Off, false, 
                    LayerMask.NameToLayer("Board"), 
                    camera
                );

            foreach(var (count, block) in nodeProps.GetRenderBlocks(currentStorage.NodeCount)) 
                Graphics.DrawMeshInstanced(
                    MeshUtils.QuadMesh, 0, 
                    UnityReferences.NodeMaterial, 
                    currentStorage.NodeTransforms.ToArray(), 
                    count, block, 
                    ShadowCastingMode.Off, false, 
                    LayerMask.NameToLayer("Board"), 
                    camera
                );

            for(var i = 0; i < currentStorage.NodePositions.Count; i++) 
                TextRenderer.Draw(currentStorage.NodePositions[i], i.ToString());
            
            TextRenderer.End();
        }

        public void DrawUI()
        {
            Graphics.DrawMesh(MeshUtils.QuadMesh, UnityReferences.RecordingTransform.WorldMatrix(), UnityReferences.RecorderMaterial, LayerMask.NameToLayer("Board"), camera);

            if (presentStorage.IsLoading)
            {
                Graphics.DrawMesh(MeshUtils.QuadMesh, UnityReferences.LoadingTransform.WorldMatrix(), UnityReferences.LoadingMaterial, LayerMask.NameToLayer("Board"), camera);
            }
        }

        public void Cleanup()
        {
            // argsBufferEdge.Dispose();
            // argsBufferNode.Dispose();
            
            // edgeTypeBuffer.Dispose();
            // edgeTransformBuffer.Dispose();
            // edgeHighlightBuffer.Dispose();

            // nodePositionBuffer.Dispose();
            // nodeHighlightBuffer.Dispose();

            Canvas.preWillRenderCanvases -= DrawUI;
        }

    }
}