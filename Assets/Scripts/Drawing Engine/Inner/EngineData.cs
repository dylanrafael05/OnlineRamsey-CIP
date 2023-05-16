using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class EngineData
{

    //
    public static Mesh QuadMesh => quadMesh;
    public static readonly Bounds Bounds = new Bounds(Vector3.zero, Vector3.one * 100f); //temp
    static readonly Mesh quadMesh = new Mesh()
    {
        vertices = new Vector3[]
        {
            new (-1f, -1f, 0f),
            new (-1f, 1f, 0f),
            new (1f, -1f, 0f),
            new (1f, 1f, 0f)
        },
        triangles = new int[]
        {
            0, 1, 3,
            0, 3, 2
        },
        uv = new Vector2[]
        {
            new (-1f, -1f),
            new (-1f, 1f),
            new (1f, -1f),
            new (1f, 1f)
        }
    };

    public EngineData()
    {
        EdgeTransforms = new();
        NodePositions = new();
    }

    public List<Matrix4x4> EdgeTransforms { get; private set; }
    public List<float2> NodePositions { get; set; } //UNEXPOSE SET

}