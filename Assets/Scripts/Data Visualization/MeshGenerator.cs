using log4net.DateFormatter;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Ramsey.Visualization
{
    public struct MatchupData
    {

        public List<int2> data; //<Turns to Win, Game Length> - <i, data[i]>

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

        static float2 GetPointOnLine(float2 p1, float2 p2, float x)
        {

            float m = (p2.y - p1.y) / (p2.x - p1.x);
            return m * (x - p1.x) + p1.y;

        }

        static float Smooth(float v1, float v2, float k)
        {
            float d = math.abs(v1 - v2);
            float h = d * .5f * math.pow(math.max(0f, 1f - d / k), 2.0f);
            return v1 > v2 ? math.max(v1, v2) : math.min(v1, v2) + h * math.sign(v2 - v1);
        }

        public static Mesh GenerateMesh(MatchupData matchupData, float xScale, float yScale, float k, float vertDistDensity)
        {

            var points = GetPoints(matchupData, new float2(xScale, yScale));
            float2 p1, p2, p3, p4;

            for (int i = 0; i < points.Count; i++)
            {
                p1 = points[math.max(0, i - 1)];
                p2 = points[i];
                p3 = points[math.min(points.Count - 1, i + 1)];
                p4 = points[math.min(points.Count - 1, i + 2)];
            }

            throw new System.NotImplementedException();

        }
    }

}