using UnityEngine;

namespace Ramsey.Utilities
{
    public static class MeshUtils
    {

        public static readonly Mesh QuadMesh = new()
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

    }
}