using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Ramsey.Utilities;

using static Unity.Mathematics.math;

namespace Ramsey.Visualization
{
    public struct MatchupData
    {

        public List<int2> data; // <Turns to Win, Game Length> - <i, data[i]>

    }

    public static class MeshGenerator
    {

        static List<float2> GetPoints(MatchupData matchupData, float2 scale)
        {
            List<float2> points = new();
            foreach (var p in matchupData.data)
                points.Add(p * scale);
            return points;
        }

        static float GetPointOnLine(float2 p1, float2 p2, float x)
        {

            float m = (p2.y - p1.y) / (p2.x - p1.x);
            return m * (x - p1.x) + p1.y;

        }

        static float Smooth(float v1, float v2, float k)
        {
            float d = abs(v1 - v2);
            float h = d * .5f * pow(max(0f, 1f - d / k), 2.0f);
            return (v1 > v2 ? max(v1, v2) : min(v1, v2)) + h * sign(v2 - v1);
        }

        public static Mesh GenerateCurveMesh(MatchupData matchupData, float k=1f)
        {
            List<float2> smoothedPoints = new();

            float vertDistDensity = 1f + k*10f;

            var points = GetPoints(matchupData, new float2(1f));
            float2 p1, p2, p3, p4, p;
            float yOn, yBa, yFo;
            int vertCount;
            float2 to;

            for (int i = 0; i < points.Count; i++)
            {
                p1 = points[max(0, i - 1)] + new float2(i - 1 < 0 ? -1 : 0, 0f);
                p2 = points[i];
                p3 = points[min(points.Count - 1, i + 1)] + new float2(i + 1 >= points.Count ? 1 : 0, 0f);
                p4 = points[min(points.Count - 1, i + 2)] + new float2(i + 2 >= points.Count ? 1 : 0, 0f);

                to = p3 - p2;
                vertCount = (int)ceil(vertDistDensity * length(to));

                for (int v = 0; v < vertCount; v++)
                {
                    p = p2 + to * ((float)v / (float)vertCount);

                    yBa = GetPointOnLine(p1, p2, p.x);
                    yOn = GetPointOnLine(p2, p3, p.x);
                    yFo = GetPointOnLine(p3, p4, p.x);

                    float interpolate = (p.x - p2.x) / (p3.x - p2.x);

                    smoothedPoints.Add(float2(p.x, Smooth(yOn, yBa, k) * (1f - interpolate) + Smooth(yOn, yFo, k) * interpolate));
                }

            }
            //smoothedPoints = points;

            float2 normal;
            List<Vector3> vertices = new();
            List<Vector2> uvs = new();

            smoothedPoints.ForEachIndex((point, i) =>
            {
                float2 normal1 = i - 1 >= 0 ? normalize((smoothedPoints[i] - smoothedPoints[i - 1]).perp()) : float2(0f);
                float2 normal2 = i + 1 < smoothedPoints.Count ? normalize((smoothedPoints[i + 1] - smoothedPoints[i]).perp()) : float2(0f);

                normal = normalize(normal1 + normal2);

                vertices.Add((point - normal*.1f).xyzV()); uvs.Add(new(0f, -1));
                vertices.Add((point + normal*.1f).xyzV()); uvs.Add(new(0f,  1));
            });

            List<int> triangles = new();

            for (int i = 0; i < vertices.Count-2; i += 2)
            {
                triangles.Add(i);
                triangles.Add(i + 1);
                triangles.Add(i + 2);
                triangles.Add(i + 3);
                triangles.Add(i + 1);
                triangles.Add(i + 2);
            }

            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);

            return mesh;

            /*return new()
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                uv = uvs.ToArray()
            };*/

        }
    }

}