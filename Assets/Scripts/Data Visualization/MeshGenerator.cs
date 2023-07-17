using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Ramsey.Utilities;

using static Unity.Mathematics.math;
using System.Linq;
using System.Collections;

namespace Ramsey.Visualization
{
    public class MatchupData : IList<MatchupResult>
    {

        int4 metaParams;
        public MatchupData(int startTarget, int endTarget, int step, int attemptsPer, string label = null)
        {
            Label = label;
            metaParams = new(startTarget, endTarget, step, attemptsPer);
        }

        public string Label { get; }

        private readonly List<MatchupResult> results = new();

        public MatchupResult this[int index] { get => ((IList<MatchupResult>)results)[index]; set => ((IList<MatchupResult>)results)[index] = value; }

        public int Count => ((ICollection<MatchupResult>)results).Count;

        public bool IsReadOnly => ((ICollection<MatchupResult>)results).IsReadOnly;

        public void Add(MatchupResult item)
        {
            ((ICollection<MatchupResult>)results).Add(item);
        }

        public void Clear()
        {
            ((ICollection<MatchupResult>)results).Clear();
        }

        public bool Contains(MatchupResult item)
        {
            return ((ICollection<MatchupResult>)results).Contains(item);
        }

        public void CopyTo(MatchupResult[] array, int arrayIndex)
        {
            ((ICollection<MatchupResult>)results).CopyTo(array, arrayIndex);
        }

        public IEnumerator<MatchupResult> GetEnumerator()
        {
            return ((IEnumerable<MatchupResult>)results).GetEnumerator();
        }

        public int IndexOf(MatchupResult item)
        {
            return ((IList<MatchupResult>)results).IndexOf(item);
        }

        public void Insert(int index, MatchupResult item)
        {
            ((IList<MatchupResult>)results).Insert(index, item);
        }

        public bool Remove(MatchupResult item)
        {
            return ((ICollection<MatchupResult>)results).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<MatchupResult>)results).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)results).GetEnumerator();
        }

        public string ToString(bool compact = false)
        {
            //jank
            string s = "Matchup Data - "
                + results.Count + " Game" + (results.Count != 1 ? "s" : "") + " Run.  "
                + "Step Size of " + metaParams.y + ".  "
                + "Start Target Path of " + metaParams.x + " End Target Path of " + metaParams.y + ".  "
                + metaParams.w + " Attempts per Target Path."
                + "\n \n";

            results.ForEach(m => s += m.ToString(compact) + "\n");
            s += "\n\n";

            return s;
        }
    }

    public readonly struct MatchupResult
    {
        public int PathSize { get; }
        public float AverageGameLength { get; }
        public int SampleSize { get; }
        public string BuilderName { get; }
        public string PainterName { get; }

        public MatchupResult(int pathsize, float gamelen, string builderName = "UNKNOWN Builder", string painterName = "UNKNOWN Painter", int samplesize = 1)
        {
            PathSize = pathsize;
            AverageGameLength = gamelen;
            SampleSize = samplesize;
            BuilderName = builderName;
            PainterName = painterName;
        }

        public static MatchupResult Average(int pathsize, string builderName, string painterName, params float[] games)
            => new(pathsize, (float)games.Average(), builderName, painterName, games.Length);

        public float2 Datapoint => float2(PathSize, AverageGameLength);

        public string ToString(bool compact = false)
            => compact ? $"{BuilderName} and {PainterName} - ({PathSize}, {AverageGameLength}, {SampleSize})"
                       : $"Using {BuilderName} and {PainterName}, Path Size of {PathSize}, Game Length of {AverageGameLength}, Sample Size of {SampleSize}";
    }

    public static class MeshGenerator
    {

        static List<float2> GetPoints(MatchupData matchupData, float2 scale)
        {
            List<float2> points = new()
            {
                float2(0f)
            };
            foreach (var p in matchupData)
                points.Add(p.Datapoint * scale);
            return new() { float2(0f), float2(1f), float2(2f) };
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
                    p = p2 + to * (v * 1f / vertCount);

                    yBa = GetPointOnLine(p1, p2, p.x);
                    yOn = GetPointOnLine(p2, p3, p.x);
                    yFo = GetPointOnLine(p3, p4, p.x);

                    float interpolate = (p.x - p2.x) / (p3.x - p2.x);

                    smoothedPoints.Add(float2(p.x, Smooth(yOn, yBa, k) * (1f - interpolate) + Smooth(yOn, yFo, k) * interpolate)); //causing the NaNs p sure (debug for.. div k 0?)
                }

            }
            smoothedPoints = points;

            float2 normal;
            List<Vector3> vertices = new();
            List<Vector2> uvs = new();

            smoothedPoints.ForEachIndex((point, i) =>
            {
                float2 normal1 = i - 1 >= 0 ? normalize((smoothedPoints[i] - smoothedPoints[i - 1]).perp()) : float2(0f); //perp method being weird problem?
                float2 normal2 = i + 1 < smoothedPoints.Count ? normalize((smoothedPoints[i + 1] - smoothedPoints[i]).perp()) : float2(0f);

                normal = normalize(normal1 + normal2);

                Debug.Log(.1f * (point - normal * 1f).xyzV());
                vertices.Add(.1f*(point - normal*1f).xyzV()); uvs.Add(new(0f, -1));
                vertices.Add(.1f*(point + normal*1f).xyzV()); uvs.Add(new(0f,  1));
            });

            List<int> triangles = new();

            for (int i = 0; i < vertices.Count-2; i += 2)
            {
                triangles.Add(i);
                triangles.Add(i + 1);
                triangles.Add(i + 2);

                triangles.Add(i + 2);
                triangles.Add(i + 1);
                triangles.Add(i + 3);
            }

            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);

            Debug.Log("The vertices: ");
            vertices.Print();

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