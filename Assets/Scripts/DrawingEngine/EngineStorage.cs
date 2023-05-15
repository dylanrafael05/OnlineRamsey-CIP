using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class EngineStorage
{

    //
    public static Mesh QuadMesh => quadMesh;
    static Mesh quadMesh = new Mesh()
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
            0, 2, 3
        },
        uv = new Vector2[]
        {
            new (-1f, -1f),
            new (-1f, 1f),
            new (1f, -1f),
            new (1f, 1f)
        }
    };

}