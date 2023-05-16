using System.Linq;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Diagnostics;

public class EngineDrawer
{

    //
    EngineData storage;

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

    public EngineDrawer(EngineData storage, EnginePreferences preferences, Camera camera)
    {

        //
        this.storage = storage;

        //
        argsArrayEdge = new uint[5]
        {
            EngineData.QuadMesh.GetIndexCount(0),
            0,
            EngineData.QuadMesh.GetIndexStart(0),
            EngineData.QuadMesh.GetBaseVertex(0),
            0
        };
        argsArrayNode = new uint[5]; argsArrayEdge.CopyTo(argsArrayNode, 0);

        argsBufferEdge = new(MAXMESHCOUNT, sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBufferNode = new(MAXMESHCOUNT, sizeof(uint), ComputeBufferType.IndirectArguments);

        edgeTransformBuffer = new (MAXMESHCOUNT, Marshal.SizeOf<Matrix4x4>());
        edgeTypeBuffer = new(MAXMESHCOUNT, sizeof(int));

        nodePositionBuffer = new (MAXMESHCOUNT, Marshal.SizeOf<float2>());

        //Uniforms Prefs
        preferences.UniformPreferences();

        //Link To Shader
        EnginePreferences.EdgeMaterial.SetBuffer(Shader.PropertyToID("Transforms"), edgeTransformBuffer);
        EnginePreferences.EdgeMaterial.SetBuffer(Shader.PropertyToID("Types"), edgeTypeBuffer);

        EnginePreferences.NodeMaterial.SetBuffer(Shader.PropertyToID("Positions"), nodePositionBuffer);

        //
        UpdateArgsBuffers();
        UpdateEdgeBuffer();
        UpdateNodeBuffer();

    }

    public void UpdateArgsBuffers()
    {

        //
        argsArrayEdge[1] = (uint) storage.EdgeTransforms.Count;
        argsArrayNode[1] = (uint)storage.NodePositions.Count;

        argsBufferEdge.SetData(argsArrayEdge);
        argsBufferNode.SetData(argsArrayNode);

    }
    public void UpdateEdgeBuffer() { edgeTransformBuffer.SetData(storage.EdgeTransforms); edgeTypeBuffer.SetData(storage.EdgeTypes); Debug.Log(string.Join(", ", storage.EdgeTypes.Select(x => x.ToString()))); }
    public void UpdateNodeBuffer() => nodePositionBuffer.SetData(storage.NodePositions);

    public void Draw()
    {
        Graphics.DrawMeshInstancedIndirect(EngineData.QuadMesh, 0, EnginePreferences.EdgeMaterial, EngineData.Bounds, argsBufferEdge, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, LayerMask.NameToLayer("Default"));
        Graphics.DrawMeshInstancedIndirect(EngineData.QuadMesh, 0, EnginePreferences.NodeMaterial, EngineData.Bounds, argsBufferNode, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, LayerMask.NameToLayer("Default"));
    }

    public void Cleanup()
    {
        argsBufferEdge.Dispose();
        argsBufferNode.Dispose();
        edgeTransformBuffer.Dispose();
        nodePositionBuffer.Dispose();
    }

}